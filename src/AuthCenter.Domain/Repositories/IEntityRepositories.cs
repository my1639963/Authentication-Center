using AuthCenter.Domain.Entities;

namespace AuthCenter.Domain.Repositories;

public interface IUserRepository
{
    Task<SysUser?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<SysUser?> GetByLoginNameAsync(string loginName, CancellationToken ct = default);
    Task<SysUser?> GetByIdCardHashAsync(string idCardHash, CancellationToken ct = default);
    Task<bool> ExistsByLoginNameAsync(string loginName, CancellationToken ct = default);
    Task<bool> ExistsByMobileAsync(string mobile, CancellationToken ct = default);
    Task AddAsync(SysUser user, CancellationToken ct = default);
    Task UpdateAsync(SysUser user, CancellationToken ct = default);
    Task<IReadOnlyList<SysUser>> SearchAsync(string? keyword, int? status, int pageIndex, int pageSize, CancellationToken ct = default);
    Task<int> CountAsync(string? keyword, int? status, CancellationToken ct = default);
}

public interface IRegionRepository
{
    Task<SysRegion?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<SysRegion?> GetByCodeAsync(string regionCode, CancellationToken ct = default);
    Task<IReadOnlyList<SysRegion>> GetChildrenAsync(long parentId, CancellationToken ct = default);
    Task<IReadOnlyList<SysRegion>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(SysRegion region, CancellationToken ct = default);
    Task UpdateAsync(SysRegion region, CancellationToken ct = default);
}

public interface IUnitRepository
{
    Task<SysUnit?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<SysUnit?> GetByCodeAsync(string unitCode, CancellationToken ct = default);
    Task<bool> ExistsByCodeAsync(string unitCode, CancellationToken ct = default);
    Task<IReadOnlyList<SysUnit>> GetChildrenAsync(long? parentId, CancellationToken ct = default);
    Task AddAsync(SysUnit unit, CancellationToken ct = default);
    Task UpdateAsync(SysUnit unit, CancellationToken ct = default);
    Task<IReadOnlyList<SysUnit>> SearchAsync(string? keyword, int? status, int pageIndex, int pageSize, CancellationToken ct = default);
    Task<int> CountAsync(string? keyword, int? status, CancellationToken ct = default);
}

public interface IDepartmentRepository
{
    Task<SysDepartment?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<SysDepartment>> GetByUnitAsync(long unitId, CancellationToken ct = default);
    Task<IReadOnlyList<SysDepartment>> GetChildrenAsync(long unitId, long? parentId, CancellationToken ct = default);
    Task<bool> ExistsCodeInUnitAsync(long unitId, string deptCode, CancellationToken ct = default);
    Task AddAsync(SysDepartment department, CancellationToken ct = default);
    Task UpdateAsync(SysDepartment department, CancellationToken ct = default);
}

public interface IPositionRepository
{
    Task<SysPosition?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<SysPosition?> GetByCodeAsync(string positionCode, CancellationToken ct = default);
    Task<bool> ExistsByCodeAsync(string positionCode, CancellationToken ct = default);
    Task AddAsync(SysPosition position, CancellationToken ct = default);
    Task UpdateAsync(SysPosition position, CancellationToken ct = default);
    Task<IReadOnlyList<SysPosition>> SearchAsync(string? keyword, int? status, int pageIndex, int pageSize, CancellationToken ct = default);
    Task<int> CountAsync(string? keyword, int? status, CancellationToken ct = default);
}

public interface IUserOrganizationRepository
{
    Task<SysUserOrganization?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<SysUserOrganization>> GetByUserIdAsync(long userId, CancellationToken ct = default);
    Task<IReadOnlyList<SysUserOrganization>> GetByUnitIdAsync(long unitId, CancellationToken ct = default);
    Task AddAsync(SysUserOrganization userOrg, CancellationToken ct = default);
    Task UpdateAsync(SysUserOrganization userOrg, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}

public interface IUserMainOrganizationRepository
{
    Task<SysUserMainOrganization?> GetByUserIdAsync(long userId, CancellationToken ct = default);
    Task AddAsync(SysUserMainOrganization mainOrg, CancellationToken ct = default);
    Task DeleteAsync(long userId, CancellationToken ct = default);
    Task<bool> ExistsByUserOrganizationIdAsync(long userOrganizationId, CancellationToken ct = default);
}

public interface IPasswordHistoryRepository
{
    Task AddAsync(SysUserPasswordHistory history, CancellationToken ct = default);
    Task<IReadOnlyList<SysUserPasswordHistory>> GetRecentAsync(long userId, int count, CancellationToken ct = default);
}
