using AuthCenter.Application.DTOs;
using AuthCenter.Domain.Entities;
using AuthCenter.Domain.Repositories;
using AuthCenter.Domain.Services;

namespace AuthCenter.Application.Services;

internal static class PermissionMapper
{
    public static AdminPermissionDto MapToDto(SysAdminPermission e) => new(
        e.Id, e.PermissionCode, e.PermissionName, e.Description);
}

public class AdminRoleService : IAdminRoleService
{
    private readonly IAdminRoleRepository _roleRepo;
    private readonly IAdminPermissionRepository _permRepo;
    private readonly IAdminRolePermissionRepository _rolePermRepo;
    private readonly IIdGenerator _idGenerator;

    public AdminRoleService(
        IAdminRoleRepository roleRepo,
        IAdminPermissionRepository permRepo,
        IAdminRolePermissionRepository rolePermRepo,
        IIdGenerator idGenerator)
    {
        _roleRepo = roleRepo;
        _permRepo = permRepo;
        _rolePermRepo = rolePermRepo;
        _idGenerator = idGenerator;
    }

    public async Task<AdminRoleDto> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await _roleRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Role '{id}' not found.");
        return MapToDto(entity);
    }

    public async Task<IReadOnlyList<AdminRoleDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _roleRepo.GetAllAsync(ct);
        return items.Select(MapToDto).ToList();
    }

    public async Task<AdminRoleDto> CreateAsync(CreateAdminRoleRequest request, CancellationToken ct = default)
    {
        var now = DateTime.Now;
        var entity = new SysAdminRole
        {
            Id = _idGenerator.NewId(),
            RoleCode = request.RoleCode,
            RoleName = request.RoleName,
            Description = request.Description,
            Status = 0,
            CreateTime = now,
            UpdateTime = now
        };
        await _roleRepo.AddAsync(entity, ct);
        return MapToDto(entity);
    }

    public async Task UpdateAsync(long id, UpdateAdminRoleRequest request, CancellationToken ct = default)
    {
        var entity = await _roleRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Role '{id}' not found.");
        if (request.RoleName != null) entity.RoleName = request.RoleName;
        if (request.Status.HasValue) entity.Status = request.Status.Value;
        if (request.Description != null) entity.Description = request.Description;
        entity.UpdateTime = DateTime.Now;
        await _roleRepo.UpdateAsync(entity, ct);
    }

    public async Task AssignPermissionsAsync(long roleId, AssignPermissionsRequest request, CancellationToken ct = default)
    {
        await _rolePermRepo.DeleteByRoleIdAsync(roleId, ct);
        foreach (var permId in request.PermissionIds)
        {
            await _rolePermRepo.AddAsync(new SysAdminRolePermission
            {
                Id = _idGenerator.NewId(),
                RoleId = roleId,
                PermissionId = permId,
                CreateTime = DateTime.Now
            }, ct);
        }
    }

    public async Task<IReadOnlyList<AdminPermissionDto>> GetRolePermissionsAsync(long roleId, CancellationToken ct = default)
    {
        var rolePerms = await _rolePermRepo.GetByRoleIdAsync(roleId, ct);
        var result = new List<AdminPermissionDto>();
        foreach (var rp in rolePerms)
        {
            var perm = await _permRepo.GetByIdAsync(rp.PermissionId, ct);
            if (perm != null) result.Add(PermissionMapper.MapToDto(perm));
        }
        return result;
    }

    private static AdminRoleDto MapToDto(SysAdminRole e) => new(
        e.Id, e.RoleCode, e.RoleName, e.Description, e.Status);
}

public class AdminPermissionService : IAdminPermissionService
{
    private readonly IAdminPermissionRepository _permRepo;

    public AdminPermissionService(IAdminPermissionRepository permRepo)
    {
        _permRepo = permRepo;
    }

    public async Task<IReadOnlyList<AdminPermissionDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _permRepo.GetAllAsync(ct);
        return items.Select(PermissionMapper.MapToDto).ToList();
    }
}

public class UserAdminService : IUserAdminService
{
    private readonly IAdminUserRoleRepository _userRoleRepo;
    private readonly IAdminRoleRepository _roleRepo;
    private readonly IAdminRolePermissionRepository _rolePermRepo;
    private readonly IAdminPermissionRepository _permRepo;
    private readonly IIdGenerator _idGenerator;

    public UserAdminService(
        IAdminUserRoleRepository userRoleRepo,
        IAdminRoleRepository roleRepo,
        IAdminRolePermissionRepository rolePermRepo,
        IAdminPermissionRepository permRepo,
        IIdGenerator idGenerator)
    {
        _userRoleRepo = userRoleRepo;
        _roleRepo = roleRepo;
        _rolePermRepo = rolePermRepo;
        _permRepo = permRepo;
        _idGenerator = idGenerator;
    }

    public async Task AssignRolesAsync(long userId, AssignRolesRequest request, long operatorId, CancellationToken ct = default)
    {
        await _userRoleRepo.DeleteByUserIdAsync(userId, ct);
        foreach (var roleId in request.RoleIds)
        {
            await _userRoleRepo.AddAsync(new SysAdminUserRole
            {
                Id = _idGenerator.NewId(),
                UserId = userId,
                RoleId = roleId,
                CreateTime = DateTime.Now
            }, ct);
        }
    }

    public async Task<IReadOnlyList<AdminRoleDto>> GetUserRolesAsync(long userId, CancellationToken ct = default)
    {
        var userRoles = await _userRoleRepo.GetByUserIdAsync(userId, ct);
        var result = new List<AdminRoleDto>();
        foreach (var ur in userRoles)
        {
            var role = await _roleRepo.GetByIdAsync(ur.RoleId, ct);
            if (role != null) result.Add(new AdminRoleDto(role.Id, role.RoleCode, role.RoleName, role.Description, role.Status));
        }
        return result;
    }

    public async Task<IReadOnlyList<AdminPermissionDto>> GetUserPermissionsAsync(long userId, CancellationToken ct = default)
    {
        var userRoles = await _userRoleRepo.GetByUserIdAsync(userId, ct);
        var permIds = new HashSet<long>();
        foreach (var ur in userRoles)
        {
            var rolePerms = await _rolePermRepo.GetByRoleIdAsync(ur.RoleId, ct);
            foreach (var rp in rolePerms) permIds.Add(rp.PermissionId);
        }

        var result = new List<AdminPermissionDto>();
        foreach (var pid in permIds)
        {
            var perm = await _permRepo.GetByIdAsync(pid, ct);
            if (perm != null) result.Add(PermissionMapper.MapToDto(perm));
        }
        return result;
    }
}

public class AuditService : IAuditService
{
    private readonly ILoginLogRepository _loginLogRepo;
    private readonly IAuditLogRepository _auditLogRepo;

    public AuditService(ILoginLogRepository loginLogRepo, IAuditLogRepository auditLogRepo)
    {
        _loginLogRepo = loginLogRepo;
        _auditLogRepo = auditLogRepo;
    }

    public async Task<PageResult<LoginLogDto>> SearchLoginLogsAsync(LoginLogQuery query, CancellationToken ct = default)
    {
        var items = await _loginLogRepo.SearchAsync(
            query.UserId, query.ClientId, query.LoginType, query.Result,
            query.StartTime, query.EndTime, query.PageIndex, query.PageSize, ct);
        var total = await _loginLogRepo.CountAsync(
            query.UserId, query.ClientId, query.LoginType, query.Result,
            query.StartTime, query.EndTime, ct);
        var dtos = items.Select(MapLoginLogToDto).ToList();
        return new PageResult<LoginLogDto>(dtos, total);
    }

    public async Task<PageResult<AuditLogDto>> SearchAuditLogsAsync(AuditLogQuery query, CancellationToken ct = default)
    {
        var items = await _auditLogRepo.SearchAsync(
            query.EventType, query.OperationType, query.OperateUserId,
            query.TargetUserId, query.Result,
            query.StartTime, query.EndTime, query.PageIndex, query.PageSize, ct);
        var total = await _auditLogRepo.CountAsync(
            query.EventType, query.OperationType, query.OperateUserId,
            query.TargetUserId, query.Result,
            query.StartTime, query.EndTime, ct);
        var dtos = items.Select(MapAuditLogToDto).ToList();
        return new PageResult<AuditLogDto>(dtos, total);
    }

    private static LoginLogDto MapLoginLogToDto(SysLoginLog e) => new(
        e.Id, e.UserId, e.LoginName, e.ClientId,
        e.LoginType, e.LoginResult, e.FailReason, e.IpAddress, e.CreateTime);

    private static AuditLogDto MapAuditLogToDto(SysAuditLog e) => new(
        e.Id, e.EventType, e.OperationType, e.OperateUserId,
        e.TargetUserId, e.ClientId, e.EventResult, e.FailReason, e.Content, e.CreateTime);
}
