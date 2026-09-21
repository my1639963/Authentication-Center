using AuthCenter.Application.DTOs;

namespace AuthCenter.Application.Services;

public interface IUserService
{
    Task<UserDetailDto> GetByIdAsync(long id, CancellationToken ct = default);
    Task<PageResult<UserDto>> SearchAsync(string? keyword, int? status, PageRequest page, CancellationToken ct = default);
    Task<UserDetailDto> CreateAsync(CreateUserRequest request, long operatorId, CancellationToken ct = default);
    Task UpdateAsync(long id, UpdateUserRequest request, long operatorId, CancellationToken ct = default);
    Task DisableAsync(long id, long operatorId, CancellationToken ct = default);
    Task EnableAsync(long id, long operatorId, CancellationToken ct = default);
    Task DeleteAsync(long id, long operatorId, CancellationToken ct = default);
    Task ResetPasswordAsync(long id, ResetPasswordRequest request, long operatorId, CancellationToken ct = default);
    Task ChangeMainOrganizationAsync(long userId, ChangeMainOrganizationRequest request, long operatorId, CancellationToken ct = default);
}

public interface IRegionService
{
    Task<RegionDto> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<RegionDto>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<RegionDto>> GetChildrenAsync(long parentId, CancellationToken ct = default);
    Task<RegionDto> CreateAsync(CreateRegionRequest request, CancellationToken ct = default);
    Task UpdateAsync(long id, UpdateRegionRequest request, CancellationToken ct = default);
}

public interface IUnitService
{
    Task<UnitDto> GetByIdAsync(long id, CancellationToken ct = default);
    Task<PageResult<UnitDto>> SearchAsync(string? keyword, int? status, PageRequest page, CancellationToken ct = default);
    Task<IReadOnlyList<UnitDto>> GetChildrenAsync(long? parentId, CancellationToken ct = default);
    Task<UnitDto> CreateAsync(CreateUnitRequest request, CancellationToken ct = default);
    Task UpdateAsync(long id, UpdateUnitRequest request, CancellationToken ct = default);
    Task DisableAsync(long id, CancellationToken ct = default);
    Task EnableAsync(long id, CancellationToken ct = default);
}

public interface IDepartmentService
{
    Task<DepartmentDto> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<DepartmentDto>> GetByUnitAsync(long unitId, CancellationToken ct = default);
    Task<IReadOnlyList<DepartmentDto>> GetChildrenAsync(long unitId, long? parentId, CancellationToken ct = default);
    Task<DepartmentDto> CreateAsync(CreateDepartmentRequest request, CancellationToken ct = default);
    Task UpdateAsync(long id, UpdateDepartmentRequest request, CancellationToken ct = default);
    Task DisableAsync(long id, CancellationToken ct = default);
    Task EnableAsync(long id, CancellationToken ct = default);
}

public interface IPositionService
{
    Task<PositionDto> GetByIdAsync(long id, CancellationToken ct = default);
    Task<PageResult<PositionDto>> SearchAsync(string? keyword, int? status, PageRequest page, CancellationToken ct = default);
    Task<PositionDto> CreateAsync(CreatePositionRequest request, CancellationToken ct = default);
    Task UpdateAsync(long id, UpdatePositionRequest request, CancellationToken ct = default);
    Task DisableAsync(long id, CancellationToken ct = default);
    Task EnableAsync(long id, CancellationToken ct = default);
}
