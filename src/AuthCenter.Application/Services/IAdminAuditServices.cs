using AuthCenter.Application.DTOs;

namespace AuthCenter.Application.Services;

public interface IAdminRoleService
{
    Task<AdminRoleDto> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<AdminRoleDto>> GetAllAsync(CancellationToken ct = default);
    Task<AdminRoleDto> CreateAsync(CreateAdminRoleRequest request, CancellationToken ct = default);
    Task UpdateAsync(long id, UpdateAdminRoleRequest request, CancellationToken ct = default);
    Task AssignPermissionsAsync(long roleId, AssignPermissionsRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<AdminPermissionDto>> GetRolePermissionsAsync(long roleId, CancellationToken ct = default);
}

public interface IAdminPermissionService
{
    Task<IReadOnlyList<AdminPermissionDto>> GetAllAsync(CancellationToken ct = default);
}

public interface IUserAdminService
{
    Task AssignRolesAsync(long userId, AssignRolesRequest request, long operatorId, CancellationToken ct = default);
    Task<IReadOnlyList<AdminRoleDto>> GetUserRolesAsync(long userId, CancellationToken ct = default);
    Task<IReadOnlyList<AdminPermissionDto>> GetUserPermissionsAsync(long userId, CancellationToken ct = default);
}

public interface IAuditService
{
    Task<PageResult<LoginLogDto>> SearchLoginLogsAsync(LoginLogQuery query, CancellationToken ct = default);
    Task<PageResult<AuditLogDto>> SearchAuditLogsAsync(AuditLogQuery query, CancellationToken ct = default);
}
