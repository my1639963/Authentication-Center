namespace AuthCenter.Application.DTOs;

public record PageRequest(int PageIndex = 1, int PageSize = 20);

public record PageResult<T>(IReadOnlyList<T> Items, int Total);

public record CreateUserRequest(
    string LoginName,
    string Password,
    string RealName,
    string? Mobile,
    string? Email,
    string? IdCardNo,
    long UnitId,
    long? DepartmentId,
    long? PositionId,
    long? RegionId
);

public record UpdateUserRequest(
    string? RealName,
    string? Mobile,
    string? Email
);

public record UserDto(
    long Id,
    string LoginName,
    string RealName,
    string? Mobile,
    string? Email,
    int Status,
    int MustModifyPwd,
    DateTime? LastLoginTime,
    DateTime CreateTime
);

public record UserDetailDto(
    long Id,
    string LoginName,
    string RealName,
    string? Mobile,
    string? Email,
    int Status,
    int MustModifyPwd,
    DateTime? PasswordExpireTime,
    DateTime? LastLoginTime,
    string? LastLoginIp,
    DateTime CreateTime,
    IReadOnlyList<UserOrganizationDto> Organizations,
    UserMainOrganizationDto? MainOrganization
);

public record UserOrganizationDto(
    long Id,
    long UnitId,
    string? UnitName,
    long? DepartmentId,
    string? DepartmentName,
    long? PositionId,
    string? PositionName,
    long? RegionId,
    DateTime? StartTime,
    DateTime? EndTime,
    int Status
);

public record UserMainOrganizationDto(
    long UserOrganizationId,
    long UnitId,
    string? UnitName,
    long? DepartmentId,
    string? DepartmentName
);

public record ResetPasswordRequest(string NewPassword);

public record ChangeMainOrganizationRequest(long UserOrganizationId);

public record LoginRequest(string LoginName, string Password);
