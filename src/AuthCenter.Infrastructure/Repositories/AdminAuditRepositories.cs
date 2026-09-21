using AuthCenter.Domain.Entities;
using AuthCenter.Domain.Repositories;
using AuthCenter.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthCenter.Infrastructure.Repositories;

public class AdminRoleRepository : IAdminRoleRepository
{
    private readonly AuthCenterDbContext _db;
    public AdminRoleRepository(AuthCenterDbContext db) => _db = db;

    public async Task<SysAdminRole?> GetByIdAsync(long id, CancellationToken ct = default)
        => await _db.AdminRoles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == 0, ct);

    public async Task<SysAdminRole?> GetByCodeAsync(string roleCode, CancellationToken ct = default)
        => await _db.AdminRoles.AsNoTracking().FirstOrDefaultAsync(x => x.RoleCode == roleCode && x.IsDeleted == 0, ct);

    public async Task<IReadOnlyList<SysAdminRole>> GetAllAsync(CancellationToken ct = default)
        => await _db.AdminRoles.AsNoTracking().Where(x => x.IsDeleted == 0).OrderBy(x => x.RoleCode).ToListAsync(ct);

    public async Task AddAsync(SysAdminRole role, CancellationToken ct = default)
        => await _db.AdminRoles.AddAsync(role, ct);

    public Task UpdateAsync(SysAdminRole role, CancellationToken ct = default)
    {
        _db.AdminRoles.Update(role);
        return Task.CompletedTask;
    }
}

public class AdminPermissionRepository : IAdminPermissionRepository
{
    private readonly AuthCenterDbContext _db;
    public AdminPermissionRepository(AuthCenterDbContext db) => _db = db;

    public async Task<SysAdminPermission?> GetByIdAsync(long id, CancellationToken ct = default)
        => await _db.AdminPermissions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<SysAdminPermission?> GetByCodeAsync(string permissionCode, CancellationToken ct = default)
        => await _db.AdminPermissions.AsNoTracking().FirstOrDefaultAsync(x => x.PermissionCode == permissionCode, ct);

    public async Task<IReadOnlyList<SysAdminPermission>> GetAllAsync(CancellationToken ct = default)
        => await _db.AdminPermissions.AsNoTracking().OrderBy(x => x.PermissionCode).ToListAsync(ct);
}

public class AdminUserRoleRepository : IAdminUserRoleRepository
{
    private readonly AuthCenterDbContext _db;
    public AdminUserRoleRepository(AuthCenterDbContext db) => _db = db;

    public async Task<IReadOnlyList<SysAdminUserRole>> GetByUserIdAsync(long userId, CancellationToken ct = default)
        => await _db.AdminUserRoles.AsNoTracking().Where(x => x.UserId == userId).ToListAsync(ct);

    public async Task AddAsync(SysAdminUserRole userRole, CancellationToken ct = default)
        => await _db.AdminUserRoles.AddAsync(userRole, ct);

    public Task DeleteAsync(long userId, long roleId, CancellationToken ct = default)
    {
        var entity = _db.AdminUserRoles.FirstOrDefault(x => x.UserId == userId && x.RoleId == roleId);
        if (entity != null) _db.AdminUserRoles.Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteByUserIdAsync(long userId, CancellationToken ct = default)
    {
        var entities = _db.AdminUserRoles.Where(x => x.UserId == userId);
        _db.AdminUserRoles.RemoveRange(entities);
        return Task.CompletedTask;
    }
}

public class AdminRolePermissionRepository : IAdminRolePermissionRepository
{
    private readonly AuthCenterDbContext _db;
    public AdminRolePermissionRepository(AuthCenterDbContext db) => _db = db;

    public async Task<IReadOnlyList<SysAdminRolePermission>> GetByRoleIdAsync(long roleId, CancellationToken ct = default)
        => await _db.AdminRolePermissions.AsNoTracking().Where(x => x.RoleId == roleId).ToListAsync(ct);

    public async Task AddAsync(SysAdminRolePermission rolePermission, CancellationToken ct = default)
        => await _db.AdminRolePermissions.AddAsync(rolePermission, ct);

    public Task DeleteAsync(long roleId, long permissionId, CancellationToken ct = default)
    {
        var entity = _db.AdminRolePermissions.FirstOrDefault(x => x.RoleId == roleId && x.PermissionId == permissionId);
        if (entity != null) _db.AdminRolePermissions.Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteByRoleIdAsync(long roleId, CancellationToken ct = default)
    {
        var entities = _db.AdminRolePermissions.Where(x => x.RoleId == roleId);
        _db.AdminRolePermissions.RemoveRange(entities);
        return Task.CompletedTask;
    }
}

public class LoginLogRepository : ILoginLogRepository
{
    private readonly AuthCenterDbContext _db;
    public LoginLogRepository(AuthCenterDbContext db) => _db = db;

    public async Task AddAsync(SysLoginLog log, CancellationToken ct = default)
        => await _db.LoginLogs.AddAsync(log, ct);

    public async Task<IReadOnlyList<SysLoginLog>> SearchAsync(long? userId, string? clientId, string? loginType, int? result, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, CancellationToken ct = default)
    {
        var query = BuildQuery(userId, clientId, loginType, result, startTime, endTime);
        return await query.OrderByDescending(x => x.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
    }

    public async Task<int> CountAsync(long? userId, string? clientId, string? loginType, int? result, DateTime? startTime, DateTime? endTime, CancellationToken ct = default)
    {
        var query = BuildQuery(userId, clientId, loginType, result, startTime, endTime);
        return await query.CountAsync(ct);
    }

    private IQueryable<SysLoginLog> BuildQuery(long? userId, string? clientId, string? loginType, int? result, DateTime? startTime, DateTime? endTime)
    {
        var query = _db.LoginLogs.AsNoTracking();
        if (userId.HasValue) query = query.Where(x => x.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(clientId)) query = query.Where(x => x.ClientId == clientId);
        if (!string.IsNullOrWhiteSpace(loginType)) query = query.Where(x => x.LoginType == loginType);
        if (result.HasValue) query = query.Where(x => x.LoginResult == result.Value);
        if (startTime.HasValue) query = query.Where(x => x.CreateTime >= startTime.Value);
        if (endTime.HasValue) query = query.Where(x => x.CreateTime <= endTime.Value);
        return query;
    }
}

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AuthCenterDbContext _db;
    public AuditLogRepository(AuthCenterDbContext db) => _db = db;

    public async Task AddAsync(SysAuditLog log, CancellationToken ct = default)
        => await _db.AuditLogs.AddAsync(log, ct);

    public async Task<SysAuditLog?> GetLatestAsync(CancellationToken ct = default)
        => await _db.AuditLogs.AsNoTracking().OrderByDescending(x => x.CreateTime).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<SysAuditLog>> SearchAsync(string? eventType, string? operationType, long? operateUserId, long? targetUserId, int? result, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, CancellationToken ct = default)
    {
        var query = BuildQuery(eventType, operationType, operateUserId, targetUserId, result, startTime, endTime);
        return await query.OrderByDescending(x => x.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? eventType, string? operationType, long? operateUserId, long? targetUserId, int? result, DateTime? startTime, DateTime? endTime, CancellationToken ct = default)
    {
        var query = BuildQuery(eventType, operationType, operateUserId, targetUserId, result, startTime, endTime);
        return await query.CountAsync(ct);
    }

    private IQueryable<SysAuditLog> BuildQuery(string? eventType, string? operationType, long? operateUserId, long? targetUserId, int? result, DateTime? startTime, DateTime? endTime)
    {
        var query = _db.AuditLogs.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(eventType)) query = query.Where(x => x.EventType == eventType);
        if (!string.IsNullOrWhiteSpace(operationType)) query = query.Where(x => x.OperationType == operationType);
        if (operateUserId.HasValue) query = query.Where(x => x.OperateUserId == operateUserId.Value);
        if (targetUserId.HasValue) query = query.Where(x => x.TargetUserId == targetUserId.Value);
        if (result.HasValue) query = query.Where(x => x.EventResult == result.Value);
        if (startTime.HasValue) query = query.Where(x => x.CreateTime >= startTime.Value);
        if (endTime.HasValue) query = query.Where(x => x.CreateTime <= endTime.Value);
        return query;
    }
}
