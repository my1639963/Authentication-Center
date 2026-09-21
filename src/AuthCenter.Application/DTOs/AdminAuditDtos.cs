namespace AuthCenter.Application.DTOs;

public record CreateAdminRoleRequest(string RoleCode, string RoleName, string? Description);

public record UpdateAdminRoleRequest(string? RoleName, int? Status, string? Description);

public record AdminRoleDto(long Id, string RoleCode, string RoleName, string? Description, int Status);

public record AssignRolesRequest(IReadOnlyList<long> RoleIds);

public record AssignPermissionsRequest(IReadOnlyList<long> PermissionIds);

public record AdminPermissionDto(
    long Id,
    string PermissionCode,
    string PermissionName,
    string? Description
);

public record LoginLogDto(
    long Id,
    long? UserId,
    string? LoginName,
    string? ClientId,
    string LoginType,
    int LoginResult,
    string? FailReason,
    string? IpAddress,
    DateTime CreateTime
);

public record AuditLogDto(
    long Id,
    string EventType,
    string OperationType,
    long? OperateUserId,
    long? TargetUserId,
    string? ClientId,
    int EventResult,
    string? FailReason,
    string? Content,
    DateTime CreateTime
);

public record LoginLogQuery(
    long? UserId,
    string? ClientId,
    string? LoginType,
    int? Result,
    DateTime? StartTime,
    DateTime? EndTime,
    int PageIndex = 1,
    int PageSize = 20
);

public record AuditLogQuery(
    string? EventType,
    string? OperationType,
    long? OperateUserId,
    long? TargetUserId,
    int? Result,
    DateTime? StartTime,
    DateTime? EndTime,
    int PageIndex = 1,
    int PageSize = 20
);
