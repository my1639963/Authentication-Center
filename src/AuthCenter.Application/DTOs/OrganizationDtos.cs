namespace AuthCenter.Application.DTOs;

public record CreateRegionRequest(
    long? ParentId,
    string RegionCode,
    string RegionName,
    int RegionLevel,
    int Sort
);

public record UpdateRegionRequest(
    string? RegionName,
    int? Status,
    int? Sort
);

public record RegionDto(
    long Id,
    long? ParentId,
    string RegionCode,
    string RegionName,
    int RegionLevel,
    int Status,
    int Sort
);

public record CreateUnitRequest(
    long? ParentId,
    string UnitCode,
    string UnitName,
    string? UnitType,
    int? UnitLevel,
    long? RegionId,
    int Sort,
    string? Description
);

public record UpdateUnitRequest(
    string? UnitName,
    string? UnitType,
    int? UnitLevel,
    long? RegionId,
    int? Status,
    int? Sort,
    string? Description
);

public record UnitDto(
    long Id,
    long? ParentId,
    string UnitCode,
    string UnitName,
    string? UnitType,
    int? UnitLevel,
    long? RegionId,
    int Status,
    int Sort,
    string? Description
);

public record CreateDepartmentRequest(
    long UnitId,
    long? ParentId,
    string DeptCode,
    string DeptName,
    long? LeaderUserId,
    int Sort,
    string? Description
);

public record UpdateDepartmentRequest(
    string? DeptName,
    long? LeaderUserId,
    int? Status,
    int? Sort,
    string? Description
);

public record DepartmentDto(
    long Id,
    long UnitId,
    long? ParentId,
    string DeptCode,
    string DeptName,
    long? LeaderUserId,
    int Status,
    int Sort,
    string? Description
);

public record CreatePositionRequest(
    string PositionCode,
    string PositionName,
    string? PositionType,
    int? PositionLevel,
    int Sort,
    string? Description
);

public record UpdatePositionRequest(
    string? PositionName,
    string? PositionType,
    int? PositionLevel,
    int? Status,
    int? Sort,
    string? Description
);

public record PositionDto(
    long Id,
    string PositionCode,
    string PositionName,
    string? PositionType,
    int? PositionLevel,
    int Status,
    int Sort,
    string? Description
);
