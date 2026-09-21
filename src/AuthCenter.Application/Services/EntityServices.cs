using AuthCenter.Application.DTOs;
using AuthCenter.Domain.Entities;
using AuthCenter.Domain.Repositories;
using AuthCenter.Domain.Services;

namespace AuthCenter.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IUserOrganizationRepository _userOrgRepo;
    private readonly IUserMainOrganizationRepository _mainOrgRepo;
    private readonly IUnitRepository _unitRepo;
    private readonly IDepartmentRepository _deptRepo;
    private readonly IPositionRepository _positionRepo;
    private readonly IPasswordHistoryRepository _pwdHistoryRepo;
    private readonly ICryptoProvider _crypto;
    private readonly IIdGenerator _idGenerator;

    public UserService(
        IUserRepository userRepo,
        IUserOrganizationRepository userOrgRepo,
        IUserMainOrganizationRepository mainOrgRepo,
        IUnitRepository unitRepo,
        IDepartmentRepository deptRepo,
        IPositionRepository positionRepo,
        IPasswordHistoryRepository pwdHistoryRepo,
        ICryptoProvider crypto,
        IIdGenerator idGenerator)
    {
        _userRepo = userRepo;
        _userOrgRepo = userOrgRepo;
        _mainOrgRepo = mainOrgRepo;
        _unitRepo = unitRepo;
        _deptRepo = deptRepo;
        _positionRepo = positionRepo;
        _pwdHistoryRepo = pwdHistoryRepo;
        _crypto = crypto;
        _idGenerator = idGenerator;
    }

    public async Task<UserDetailDto> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"User '{id}' not found.");
        return await MapToDetailDtoAsync(user, ct);
    }

    public async Task<PageResult<UserDto>> SearchAsync(string? keyword, int? status, PageRequest page, CancellationToken ct = default)
    {
        var items = await _userRepo.SearchAsync(keyword, status, page.PageIndex, page.PageSize, ct);
        var total = await _userRepo.CountAsync(keyword, status, ct);
        var dtos = items.Select(MapToListDto).ToList();
        return new PageResult<UserDto>(dtos, total);
    }

    public async Task<UserDetailDto> CreateAsync(CreateUserRequest request, long operatorId, CancellationToken ct = default)
    {
        if (await _userRepo.ExistsByLoginNameAsync(request.LoginName, ct))
            throw new InvalidOperationException($"Login name '{request.LoginName}' already exists.");

        var now = DateTime.Now;
        var user = new SysUser
        {
            Id = _idGenerator.NewId(),
            LoginName = request.LoginName,
            PasswordHash = _crypto.HashPassword(request.Password),
            RealName = request.RealName,
            Mobile = request.Mobile,
            Email = request.Email,
            Status = 0,
            MustModifyPwd = 1,
            TokenVersion = 1,
            CreateTime = now,
            UpdateTime = now
        };

        if (!string.IsNullOrEmpty(request.IdCardNo))
        {
            user.IdCardCiphertext = _crypto.Encrypt(request.IdCardNo);
            user.IdCardHash = _crypto.ComputeHmac(request.IdCardNo);
        }

        await _userRepo.AddAsync(user, ct);

        var userOrg = new SysUserOrganization
        {
            Id = _idGenerator.NewId(),
            UserId = user.Id,
            UnitId = request.UnitId,
            DepartmentId = request.DepartmentId,
            PositionId = request.PositionId,
            RegionId = request.RegionId,
            StartTime = now,
            Status = 0,
            CreateTime = now,
            UpdateTime = now
        };
        await _userOrgRepo.AddAsync(userOrg, ct);

        var mainOrg = new SysUserMainOrganization
        {
            Id = _idGenerator.NewId(),
            UserId = user.Id,
            UserOrganizationId = userOrg.Id,
            CreateTime = now,
            UpdateTime = now
        };
        await _mainOrgRepo.AddAsync(mainOrg, ct);

        return await MapToDetailDtoAsync(user, ct);
    }

    public async Task UpdateAsync(long id, UpdateUserRequest request, long operatorId, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"User '{id}' not found.");

        if (request.RealName != null) user.RealName = request.RealName;
        if (request.Mobile != null) user.Mobile = request.Mobile;
        if (request.Email != null) user.Email = request.Email;
        user.UpdateTime = DateTime.Now;

        await _userRepo.UpdateAsync(user, ct);
    }

    public async Task DisableAsync(long id, long operatorId, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"User '{id}' not found.");
        user.Status = 1;
        user.UpdateTime = DateTime.Now;
        await _userRepo.UpdateAsync(user, ct);
    }

    public async Task EnableAsync(long id, long operatorId, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"User '{id}' not found.");
        user.Status = 0;
        user.LoginFailCount = 0;
        user.UpdateTime = DateTime.Now;
        await _userRepo.UpdateAsync(user, ct);
    }

    public async Task DeleteAsync(long id, long operatorId, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"User '{id}' not found.");
        user.IsDeleted = 1;
        user.UpdateTime = DateTime.Now;
        await _userRepo.UpdateAsync(user, ct);
    }

    public async Task ResetPasswordAsync(long id, ResetPasswordRequest request, long operatorId, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"User '{id}' not found.");

        var newHash = _crypto.HashPassword(request.NewPassword);
        await _pwdHistoryRepo.AddAsync(new SysUserPasswordHistory
        {
            Id = _idGenerator.NewId(),
            UserId = id,
            PasswordHash = user.PasswordHash,
            CreateTime = DateTime.Now
        }, ct);

        user.PasswordHash = newHash;
        user.TokenVersion++;
        user.SecurityStamp = Guid.NewGuid().ToString("N");
        user.MustModifyPwd = 0;
        user.UpdateTime = DateTime.Now;
        await _userRepo.UpdateAsync(user, ct);
    }

    public async Task ChangeMainOrganizationAsync(long userId, ChangeMainOrganizationRequest request, long operatorId, CancellationToken ct = default)
    {
        if (!await _mainOrgRepo.ExistsByUserOrganizationIdAsync(request.UserOrganizationId, ct))
            throw new InvalidOperationException($"User organization '{request.UserOrganizationId}' not found.");

        await _mainOrgRepo.DeleteAsync(userId, ct);
        await _mainOrgRepo.AddAsync(new SysUserMainOrganization
        {
            Id = _idGenerator.NewId(),
            UserId = userId,
            UserOrganizationId = request.UserOrganizationId,
            CreateTime = DateTime.Now,
            UpdateTime = DateTime.Now
        }, ct);
    }

    private async Task<UserDetailDto> MapToDetailDtoAsync(SysUser user, CancellationToken ct)
    {
        var orgs = await _userOrgRepo.GetByUserIdAsync(user.Id, ct);
        var orgDtos = new List<UserOrganizationDto>();
        foreach (var o in orgs)
        {
            var unit = await _unitRepo.GetByIdAsync(o.UnitId, ct);
            string? deptName = null;
            if (o.DepartmentId.HasValue)
            {
                var dept = await _deptRepo.GetByIdAsync(o.DepartmentId.Value, ct);
                deptName = dept?.DeptName;
            }
            string? posName = null;
            if (o.PositionId.HasValue)
            {
                var pos = await _positionRepo.GetByIdAsync(o.PositionId.Value, ct);
                posName = pos?.PositionName;
            }
            orgDtos.Add(new UserOrganizationDto(
                o.Id, o.UnitId, unit?.UnitName,
                o.DepartmentId, deptName,
                o.PositionId, posName,
                o.RegionId, o.StartTime, o.EndTime, o.Status));
        }

        UserMainOrganizationDto? mainDto = null;
        var mainOrg = await _mainOrgRepo.GetByUserIdAsync(user.Id, ct);
        if (mainOrg != null)
        {
            var mainUserOrg = await _userOrgRepo.GetByIdAsync(mainOrg.UserOrganizationId, ct);
            if (mainUserOrg != null)
            {
                var unit = await _unitRepo.GetByIdAsync(mainUserOrg.UnitId, ct);
                string? deptName = null;
                if (mainUserOrg.DepartmentId.HasValue)
                {
                    var dept = await _deptRepo.GetByIdAsync(mainUserOrg.DepartmentId.Value, ct);
                    deptName = dept?.DeptName;
                }
                mainDto = new UserMainOrganizationDto(
                    mainUserOrg.Id, mainUserOrg.UnitId, unit?.UnitName,
                    mainUserOrg.DepartmentId, deptName);
            }
        }

        return new UserDetailDto(
            user.Id, user.LoginName, user.RealName, user.Mobile, user.Email,
            user.Status, user.MustModifyPwd, user.PasswordExpireTime,
            user.LastLoginTime, user.LastLoginIp, user.CreateTime,
            orgDtos, mainDto);
    }

    private static UserDto MapToListDto(SysUser user) => new(
        user.Id, user.LoginName, user.RealName, user.Mobile, user.Email,
        user.Status, user.MustModifyPwd, user.LastLoginTime, user.CreateTime);
}

public class RegionService : IRegionService
{
    private readonly IRegionRepository _regionRepo;
    private readonly IIdGenerator _idGenerator;

    public RegionService(IRegionRepository regionRepo, IIdGenerator idGenerator)
    {
        _regionRepo = regionRepo;
        _idGenerator = idGenerator;
    }

    public async Task<RegionDto> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await _regionRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Region '{id}' not found.");
        return MapToDto(entity);
    }

    public async Task<IReadOnlyList<RegionDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _regionRepo.GetAllAsync(ct);
        return items.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<RegionDto>> GetChildrenAsync(long parentId, CancellationToken ct = default)
    {
        var items = await _regionRepo.GetChildrenAsync(parentId, ct);
        return items.Select(MapToDto).ToList();
    }

    public async Task<RegionDto> CreateAsync(CreateRegionRequest request, CancellationToken ct = default)
    {
        var now = DateTime.Now;
        var entity = new SysRegion
        {
            Id = _idGenerator.NewId(),
            ParentId = request.ParentId,
            RegionCode = request.RegionCode,
            RegionName = request.RegionName,
            RegionLevel = request.RegionLevel,
            Sort = request.Sort,
            Status = 0,
            CreateTime = now,
            UpdateTime = now
        };
        await _regionRepo.AddAsync(entity, ct);
        return MapToDto(entity);
    }

    public async Task UpdateAsync(long id, UpdateRegionRequest request, CancellationToken ct = default)
    {
        var entity = await _regionRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Region '{id}' not found.");
        if (request.RegionName != null) entity.RegionName = request.RegionName;
        if (request.Status.HasValue) entity.Status = request.Status.Value;
        if (request.Sort.HasValue) entity.Sort = request.Sort.Value;
        entity.UpdateTime = DateTime.Now;
        await _regionRepo.UpdateAsync(entity, ct);
    }

    private static RegionDto MapToDto(SysRegion e) => new(
        e.Id, e.ParentId, e.RegionCode, e.RegionName, e.RegionLevel, e.Status, e.Sort);
}

public class UnitService : IUnitService
{
    private readonly IUnitRepository _unitRepo;
    private readonly IIdGenerator _idGenerator;

    public UnitService(IUnitRepository unitRepo, IIdGenerator idGenerator)
    {
        _unitRepo = unitRepo;
        _idGenerator = idGenerator;
    }

    public async Task<UnitDto> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await _unitRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Unit '{id}' not found.");
        return MapToDto(entity);
    }

    public async Task<PageResult<UnitDto>> SearchAsync(string? keyword, int? status, PageRequest page, CancellationToken ct = default)
    {
        var items = await _unitRepo.SearchAsync(keyword, status, page.PageIndex, page.PageSize, ct);
        var total = await _unitRepo.CountAsync(keyword, status, ct);
        return new PageResult<UnitDto>(items.Select(MapToDto).ToList(), total);
    }

    public async Task<IReadOnlyList<UnitDto>> GetChildrenAsync(long? parentId, CancellationToken ct = default)
    {
        var items = await _unitRepo.GetChildrenAsync(parentId, ct);
        return items.Select(MapToDto).ToList();
    }

    public async Task<UnitDto> CreateAsync(CreateUnitRequest request, CancellationToken ct = default)
    {
        var now = DateTime.Now;
        var entity = new SysUnit
        {
            Id = _idGenerator.NewId(),
            ParentId = request.ParentId,
            UnitCode = request.UnitCode,
            UnitName = request.UnitName,
            UnitType = request.UnitType,
            UnitLevel = request.UnitLevel,
            RegionId = request.RegionId,
            Sort = request.Sort,
            Description = request.Description,
            Status = 0,
            CreateTime = now,
            UpdateTime = now
        };
        await _unitRepo.AddAsync(entity, ct);
        return MapToDto(entity);
    }

    public async Task UpdateAsync(long id, UpdateUnitRequest request, CancellationToken ct = default)
    {
        var entity = await _unitRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Unit '{id}' not found.");
        if (request.UnitName != null) entity.UnitName = request.UnitName;
        if (request.UnitType != null) entity.UnitType = request.UnitType;
        if (request.UnitLevel.HasValue) entity.UnitLevel = request.UnitLevel.Value;
        if (request.RegionId.HasValue) entity.RegionId = request.RegionId.Value;
        if (request.Status.HasValue) entity.Status = request.Status.Value;
        if (request.Sort.HasValue) entity.Sort = request.Sort.Value;
        if (request.Description != null) entity.Description = request.Description;
        entity.UpdateTime = DateTime.Now;
        await _unitRepo.UpdateAsync(entity, ct);
    }

    public async Task DisableAsync(long id, CancellationToken ct = default)
    {
        var entity = await _unitRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Unit '{id}' not found.");
        entity.Status = 1;
        entity.UpdateTime = DateTime.Now;
        await _unitRepo.UpdateAsync(entity, ct);
    }

    public async Task EnableAsync(long id, CancellationToken ct = default)
    {
        var entity = await _unitRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Unit '{id}' not found.");
        entity.Status = 0;
        entity.UpdateTime = DateTime.Now;
        await _unitRepo.UpdateAsync(entity, ct);
    }

    private static UnitDto MapToDto(SysUnit e) => new(
        e.Id, e.ParentId, e.UnitCode, e.UnitName, e.UnitType,
        e.UnitLevel, e.RegionId, e.Status, e.Sort, e.Description);
}

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _deptRepo;
    private readonly IIdGenerator _idGenerator;

    public DepartmentService(IDepartmentRepository deptRepo, IIdGenerator idGenerator)
    {
        _deptRepo = deptRepo;
        _idGenerator = idGenerator;
    }

    public async Task<DepartmentDto> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await _deptRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Department '{id}' not found.");
        return MapToDto(entity);
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetByUnitAsync(long unitId, CancellationToken ct = default)
    {
        var items = await _deptRepo.GetByUnitAsync(unitId, ct);
        return items.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetChildrenAsync(long unitId, long? parentId, CancellationToken ct = default)
    {
        var items = await _deptRepo.GetChildrenAsync(unitId, parentId, ct);
        return items.Select(MapToDto).ToList();
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentRequest request, CancellationToken ct = default)
    {
        var now = DateTime.Now;
        var entity = new SysDepartment
        {
            Id = _idGenerator.NewId(),
            UnitId = request.UnitId,
            ParentId = request.ParentId,
            DeptCode = request.DeptCode,
            DeptName = request.DeptName,
            LeaderUserId = request.LeaderUserId,
            Sort = request.Sort,
            Description = request.Description,
            Status = 0,
            CreateTime = now,
            UpdateTime = now
        };
        await _deptRepo.AddAsync(entity, ct);
        return MapToDto(entity);
    }

    public async Task UpdateAsync(long id, UpdateDepartmentRequest request, CancellationToken ct = default)
    {
        var entity = await _deptRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Department '{id}' not found.");
        if (request.DeptName != null) entity.DeptName = request.DeptName;
        if (request.LeaderUserId.HasValue) entity.LeaderUserId = request.LeaderUserId.Value;
        if (request.Status.HasValue) entity.Status = request.Status.Value;
        if (request.Sort.HasValue) entity.Sort = request.Sort.Value;
        if (request.Description != null) entity.Description = request.Description;
        entity.UpdateTime = DateTime.Now;
        await _deptRepo.UpdateAsync(entity, ct);
    }

    public async Task DisableAsync(long id, CancellationToken ct = default)
    {
        var entity = await _deptRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Department '{id}' not found.");
        entity.Status = 1;
        entity.UpdateTime = DateTime.Now;
        await _deptRepo.UpdateAsync(entity, ct);
    }

    public async Task EnableAsync(long id, CancellationToken ct = default)
    {
        var entity = await _deptRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Department '{id}' not found.");
        entity.Status = 0;
        entity.UpdateTime = DateTime.Now;
        await _deptRepo.UpdateAsync(entity, ct);
    }

    private static DepartmentDto MapToDto(SysDepartment e) => new(
        e.Id, e.UnitId, e.ParentId, e.DeptCode, e.DeptName,
        e.LeaderUserId, e.Status, e.Sort, e.Description);
}

public class PositionService : IPositionService
{
    private readonly IPositionRepository _positionRepo;
    private readonly IIdGenerator _idGenerator;

    public PositionService(IPositionRepository positionRepo, IIdGenerator idGenerator)
    {
        _positionRepo = positionRepo;
        _idGenerator = idGenerator;
    }

    public async Task<PositionDto> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await _positionRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Position '{id}' not found.");
        return MapToDto(entity);
    }

    public async Task<PageResult<PositionDto>> SearchAsync(string? keyword, int? status, PageRequest page, CancellationToken ct = default)
    {
        var items = await _positionRepo.SearchAsync(keyword, status, page.PageIndex, page.PageSize, ct);
        var total = await _positionRepo.CountAsync(keyword, status, ct);
        return new PageResult<PositionDto>(items.Select(MapToDto).ToList(), total);
    }

    public async Task<PositionDto> CreateAsync(CreatePositionRequest request, CancellationToken ct = default)
    {
        var now = DateTime.Now;
        var entity = new SysPosition
        {
            Id = _idGenerator.NewId(),
            PositionCode = request.PositionCode,
            PositionName = request.PositionName,
            PositionType = request.PositionType,
            PositionLevel = request.PositionLevel,
            Sort = request.Sort,
            Description = request.Description,
            Status = 0,
            CreateTime = now,
            UpdateTime = now
        };
        await _positionRepo.AddAsync(entity, ct);
        return MapToDto(entity);
    }

    public async Task UpdateAsync(long id, UpdatePositionRequest request, CancellationToken ct = default)
    {
        var entity = await _positionRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Position '{id}' not found.");
        if (request.PositionName != null) entity.PositionName = request.PositionName;
        if (request.PositionType != null) entity.PositionType = request.PositionType;
        if (request.PositionLevel.HasValue) entity.PositionLevel = request.PositionLevel.Value;
        if (request.Status.HasValue) entity.Status = request.Status.Value;
        if (request.Sort.HasValue) entity.Sort = request.Sort.Value;
        if (request.Description != null) entity.Description = request.Description;
        entity.UpdateTime = DateTime.Now;
        await _positionRepo.UpdateAsync(entity, ct);
    }

    public async Task DisableAsync(long id, CancellationToken ct = default)
    {
        var entity = await _positionRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Position '{id}' not found.");
        entity.Status = 1;
        entity.UpdateTime = DateTime.Now;
        await _positionRepo.UpdateAsync(entity, ct);
    }

    public async Task EnableAsync(long id, CancellationToken ct = default)
    {
        var entity = await _positionRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Position '{id}' not found.");
        entity.Status = 0;
        entity.UpdateTime = DateTime.Now;
        await _positionRepo.UpdateAsync(entity, ct);
    }

    private static PositionDto MapToDto(SysPosition e) => new(
        e.Id, e.PositionCode, e.PositionName, e.PositionType,
        e.PositionLevel, e.Status, e.Sort, e.Description);
}
