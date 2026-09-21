using AuthCenter.Domain.Entities;

namespace AuthCenter.Domain.Repositories;

public interface IAdminRoleRepository
{
    Task<SysAdminRole?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<SysAdminRole?> GetByCodeAsync(string roleCode, CancellationToken ct = default);
    Task<IReadOnlyList<SysAdminRole>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(SysAdminRole role, CancellationToken ct = default);
    Task UpdateAsync(SysAdminRole role, CancellationToken ct = default);
}

public interface IAdminPermissionRepository
{
    Task<SysAdminPermission?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<SysAdminPermission?> GetByCodeAsync(string permissionCode, CancellationToken ct = default);
    Task<IReadOnlyList<SysAdminPermission>> GetAllAsync(CancellationToken ct = default);
}

public interface IAdminUserRoleRepository
{
    Task<IReadOnlyList<SysAdminUserRole>> GetByUserIdAsync(long userId, CancellationToken ct = default);
    Task AddAsync(SysAdminUserRole userRole, CancellationToken ct = default);
    Task DeleteAsync(long userId, long roleId, CancellationToken ct = default);
    Task DeleteByUserIdAsync(long userId, CancellationToken ct = default);
}

public interface IAdminRolePermissionRepository
{
    Task<IReadOnlyList<SysAdminRolePermission>> GetByRoleIdAsync(long roleId, CancellationToken ct = default);
    Task AddAsync(SysAdminRolePermission rolePermission, CancellationToken ct = default);
    Task DeleteAsync(long roleId, long permissionId, CancellationToken ct = default);
    Task DeleteByRoleIdAsync(long roleId, CancellationToken ct = default);
}

public interface ILoginLogRepository
{
    Task AddAsync(SysLoginLog log, CancellationToken ct = default);
    Task<IReadOnlyList<SysLoginLog>> SearchAsync(long? userId, string? clientId, string? loginType, int? result, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, CancellationToken ct = default);
    Task<int> CountAsync(long? userId, string? clientId, string? loginType, int? result, DateTime? startTime, DateTime? endTime, CancellationToken ct = default);
}

public interface IAuditLogRepository
{
    Task AddAsync(SysAuditLog log, CancellationToken ct = default);
    Task<SysAuditLog?> GetLatestAsync(CancellationToken ct = default);
    Task<IReadOnlyList<SysAuditLog>> SearchAsync(string? eventType, string? operationType, long? operateUserId, long? targetUserId, int? result, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, CancellationToken ct = default);
    Task<int> CountAsync(string? eventType, string? operationType, long? operateUserId, long? targetUserId, int? result, DateTime? startTime, DateTime? endTime, CancellationToken ct = default);
}
