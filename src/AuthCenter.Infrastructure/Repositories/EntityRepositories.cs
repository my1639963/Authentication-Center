using AuthCenter.Domain.Entities;
using AuthCenter.Domain.Repositories;
using AuthCenter.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthCenter.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthCenterDbContext _db;

    public UserRepository(AuthCenterDbContext db) => _db = db;

    public async Task<SysUser?> GetByIdAsync(long id, CancellationToken ct = default)
        => await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == 0, ct);

    public async Task<SysUser?> GetByLoginNameAsync(string loginName, CancellationToken ct = default)
        => await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.LoginName == loginName && x.IsDeleted == 0, ct);

    public async Task<SysUser?> GetByIdCardHashAsync(string idCardHash, CancellationToken ct = default)
        => await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.IdCardHash == idCardHash && x.IsDeleted == 0, ct);

    public async Task<bool> ExistsByLoginNameAsync(string loginName, CancellationToken ct = default)
        => await _db.Users.AnyAsync(x => x.LoginName == loginName && x.IsDeleted == 0, ct);

    public async Task<bool> ExistsByMobileAsync(string mobile, CancellationToken ct = default)
        => await _db.Users.AnyAsync(x => x.Mobile == mobile && x.IsDeleted == 0, ct);

    public async Task AddAsync(SysUser user, CancellationToken ct = default)
        => await _db.Users.AddAsync(user, ct);

    public Task UpdateAsync(SysUser user, CancellationToken ct = default)
    {
        _db.Users.Update(user);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<SysUser>> SearchAsync(string? keyword, int? status, int pageIndex, int pageSize, CancellationToken ct = default)
    {
        var query = _db.Users.AsNoTracking().Where(x => x.IsDeleted == 0);
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(x => x.RealName.Contains(keyword) || x.LoginName.Contains(keyword) || (x.Mobile != null && x.Mobile.Contains(keyword)));
        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);
        return await query.OrderByDescending(x => x.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? keyword, int? status, CancellationToken ct = default)
    {
        var query = _db.Users.Where(x => x.IsDeleted == 0);
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(x => x.RealName.Contains(keyword) || x.LoginName.Contains(keyword) || (x.Mobile != null && x.Mobile.Contains(keyword)));
        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);
        return await query.CountAsync(ct);
    }
}

public class UnitRepository : IUnitRepository
{
    private readonly AuthCenterDbContext _db;

    public UnitRepository(AuthCenterDbContext db) => _db = db;

    public async Task<SysUnit?> GetByIdAsync(long id, CancellationToken ct = default)
        => await _db.Units.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == 0, ct);

    public async Task<SysUnit?> GetByCodeAsync(string unitCode, CancellationToken ct = default)
        => await _db.Units.AsNoTracking().FirstOrDefaultAsync(x => x.UnitCode == unitCode && x.IsDeleted == 0, ct);

    public async Task<bool> ExistsByCodeAsync(string unitCode, CancellationToken ct = default)
        => await _db.Units.AnyAsync(x => x.UnitCode == unitCode && x.IsDeleted == 0, ct);

    public async Task<IReadOnlyList<SysUnit>> GetChildrenAsync(long? parentId, CancellationToken ct = default)
        => await _db.Units.AsNoTracking().Where(x => x.ParentId == parentId && x.IsDeleted == 0).OrderBy(x => x.Sort).ToListAsync(ct);

    public async Task AddAsync(SysUnit unit, CancellationToken ct = default)
        => await _db.Units.AddAsync(unit, ct);

    public Task UpdateAsync(SysUnit unit, CancellationToken ct = default)
    {
        _db.Units.Update(unit);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<SysUnit>> SearchAsync(string? keyword, int? status, int pageIndex, int pageSize, CancellationToken ct = default)
    {
        var query = _db.Units.AsNoTracking().Where(x => x.IsDeleted == 0);
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(x => x.UnitName.Contains(keyword) || x.UnitCode.Contains(keyword));
        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);
        return await query.OrderByDescending(x => x.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? keyword, int? status, CancellationToken ct = default)
    {
        var query = _db.Units.Where(x => x.IsDeleted == 0);
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(x => x.UnitName.Contains(keyword) || x.UnitCode.Contains(keyword));
        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);
        return await query.CountAsync(ct);
    }
}

public class RegionRepository : IRegionRepository
{
    private readonly AuthCenterDbContext _db;

    public RegionRepository(AuthCenterDbContext db) => _db = db;

    public async Task<SysRegion?> GetByIdAsync(long id, CancellationToken ct = default)
        => await _db.Regions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == 0, ct);

    public async Task<SysRegion?> GetByCodeAsync(string regionCode, CancellationToken ct = default)
        => await _db.Regions.AsNoTracking().FirstOrDefaultAsync(x => x.RegionCode == regionCode && x.IsDeleted == 0, ct);

    public async Task<IReadOnlyList<SysRegion>> GetChildrenAsync(long parentId, CancellationToken ct = default)
        => await _db.Regions.AsNoTracking().Where(x => x.ParentId == parentId && x.IsDeleted == 0).OrderBy(x => x.Sort).ToListAsync(ct);

    public async Task<IReadOnlyList<SysRegion>> GetAllAsync(CancellationToken ct = default)
        => await _db.Regions.AsNoTracking().Where(x => x.IsDeleted == 0).OrderBy(x => x.Sort).ToListAsync(ct);

    public async Task AddAsync(SysRegion region, CancellationToken ct = default)
        => await _db.Regions.AddAsync(region, ct);

    public Task UpdateAsync(SysRegion region, CancellationToken ct = default)
    {
        _db.Regions.Update(region);
        return Task.CompletedTask;
    }
}

public class DepartmentRepository : IDepartmentRepository
{
    private readonly AuthCenterDbContext _db;

    public DepartmentRepository(AuthCenterDbContext db) => _db = db;

    public async Task<SysDepartment?> GetByIdAsync(long id, CancellationToken ct = default)
        => await _db.Departments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == 0, ct);

    public async Task<IReadOnlyList<SysDepartment>> GetByUnitAsync(long unitId, CancellationToken ct = default)
        => await _db.Departments.AsNoTracking().Where(x => x.UnitId == unitId && x.IsDeleted == 0).OrderBy(x => x.Sort).ToListAsync(ct);

    public async Task<IReadOnlyList<SysDepartment>> GetChildrenAsync(long unitId, long? parentId, CancellationToken ct = default)
        => await _db.Departments.AsNoTracking().Where(x => x.UnitId == unitId && x.ParentId == parentId && x.IsDeleted == 0).OrderBy(x => x.Sort).ToListAsync(ct);

    public async Task<bool> ExistsCodeInUnitAsync(long unitId, string deptCode, CancellationToken ct = default)
        => await _db.Departments.AnyAsync(x => x.UnitId == unitId && x.DeptCode == deptCode && x.IsDeleted == 0, ct);

    public async Task AddAsync(SysDepartment department, CancellationToken ct = default)
        => await _db.Departments.AddAsync(department, ct);

    public Task UpdateAsync(SysDepartment department, CancellationToken ct = default)
    {
        _db.Departments.Update(department);
        return Task.CompletedTask;
    }
}

public class PositionRepository : IPositionRepository
{
    private readonly AuthCenterDbContext _db;

    public PositionRepository(AuthCenterDbContext db) => _db = db;

    public async Task<SysPosition?> GetByIdAsync(long id, CancellationToken ct = default)
        => await _db.Positions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == 0, ct);

    public async Task<SysPosition?> GetByCodeAsync(string positionCode, CancellationToken ct = default)
        => await _db.Positions.AsNoTracking().FirstOrDefaultAsync(x => x.PositionCode == positionCode && x.IsDeleted == 0, ct);

    public async Task<bool> ExistsByCodeAsync(string positionCode, CancellationToken ct = default)
        => await _db.Positions.AnyAsync(x => x.PositionCode == positionCode && x.IsDeleted == 0, ct);

    public async Task AddAsync(SysPosition position, CancellationToken ct = default)
        => await _db.Positions.AddAsync(position, ct);

    public Task UpdateAsync(SysPosition position, CancellationToken ct = default)
    {
        _db.Positions.Update(position);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<SysPosition>> SearchAsync(string? keyword, int? status, int pageIndex, int pageSize, CancellationToken ct = default)
    {
        var query = _db.Positions.AsNoTracking().Where(x => x.IsDeleted == 0);
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(x => x.PositionName.Contains(keyword) || x.PositionCode.Contains(keyword));
        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);
        return await query.OrderByDescending(x => x.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? keyword, int? status, CancellationToken ct = default)
    {
        var query = _db.Positions.Where(x => x.IsDeleted == 0);
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(x => x.PositionName.Contains(keyword) || x.PositionCode.Contains(keyword));
        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);
        return await query.CountAsync(ct);
    }
}

public class UserOrganizationRepository : IUserOrganizationRepository
{
    private readonly AuthCenterDbContext _db;

    public UserOrganizationRepository(AuthCenterDbContext db) => _db = db;

    public async Task<SysUserOrganization?> GetByIdAsync(long id, CancellationToken ct = default)
        => await _db.UserOrganizations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == 0, ct);

    public async Task<IReadOnlyList<SysUserOrganization>> GetByUserIdAsync(long userId, CancellationToken ct = default)
        => await _db.UserOrganizations.AsNoTracking().Where(x => x.UserId == userId && x.IsDeleted == 0).ToListAsync(ct);

    public async Task<IReadOnlyList<SysUserOrganization>> GetByUnitIdAsync(long unitId, CancellationToken ct = default)
        => await _db.UserOrganizations.AsNoTracking().Where(x => x.UnitId == unitId && x.IsDeleted == 0).ToListAsync(ct);

    public async Task AddAsync(SysUserOrganization userOrg, CancellationToken ct = default)
        => await _db.UserOrganizations.AddAsync(userOrg, ct);

    public Task UpdateAsync(SysUserOrganization userOrg, CancellationToken ct = default)
    {
        _db.UserOrganizations.Update(userOrg);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = new SysUserOrganization { Id = id, IsDeleted = 1, UpdateTime = DateTime.UtcNow };
        _db.UserOrganizations.Update(entity);
        return Task.CompletedTask;
    }
}

public class UserMainOrganizationRepository : IUserMainOrganizationRepository
{
    private readonly AuthCenterDbContext _db;

    public UserMainOrganizationRepository(AuthCenterDbContext db) => _db = db;

    public async Task<SysUserMainOrganization?> GetByUserIdAsync(long userId, CancellationToken ct = default)
        => await _db.UserMainOrganizations.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId, ct);

    public async Task AddAsync(SysUserMainOrganization mainOrg, CancellationToken ct = default)
        => await _db.UserMainOrganizations.AddAsync(mainOrg, ct);

    public Task DeleteAsync(long userId, CancellationToken ct = default)
    {
        var existing = _db.UserMainOrganizations.FirstOrDefault(x => x.UserId == userId);
        if (existing != null) _db.UserMainOrganizations.Remove(existing);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsByUserOrganizationIdAsync(long userOrganizationId, CancellationToken ct = default)
        => await _db.UserMainOrganizations.AnyAsync(x => x.UserOrganizationId == userOrganizationId, ct);
}

public class PasswordHistoryRepository : IPasswordHistoryRepository
{
    private readonly AuthCenterDbContext _db;

    public PasswordHistoryRepository(AuthCenterDbContext db) => _db = db;

    public async Task AddAsync(SysUserPasswordHistory history, CancellationToken ct = default)
        => await _db.PasswordHistories.AddAsync(history, ct);

    public async Task<IReadOnlyList<SysUserPasswordHistory>> GetRecentAsync(long userId, int count, CancellationToken ct = default)
        => await _db.PasswordHistories.AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreateTime)
            .Take(count)
            .ToListAsync(ct);
}
