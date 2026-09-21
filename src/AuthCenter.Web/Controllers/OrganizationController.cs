using AuthCenter.Application.DTOs;
using AuthCenter.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthCenter.Web.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class OrganizationController : ControllerBase
{
    private readonly IRegionService _regionService;
    private readonly IUnitService _unitService;
    private readonly IDepartmentService _deptService;
    private readonly IPositionService _positionService;

    public OrganizationController(
        IRegionService regionService,
        IUnitService unitService,
        IDepartmentService deptService,
        IPositionService positionService)
    {
        _regionService = regionService;
        _unitService = unitService;
        _deptService = deptService;
        _positionService = positionService;
    }

    [HttpGet("regions")]
    public async Task<ActionResult<IReadOnlyList<RegionDto>>> GetAllRegions()
        => Ok(await _regionService.GetAllAsync());

    [HttpGet("regions/{id:long}")]
    public async Task<ActionResult<RegionDto>> GetRegion(long id)
        => Ok(await _regionService.GetByIdAsync(id));

    [HttpGet("regions/{parentId:long}/children")]
    public async Task<ActionResult<IReadOnlyList<RegionDto>>> GetRegionChildren(long parentId)
        => Ok(await _regionService.GetChildrenAsync(parentId));

    [HttpPost("regions")]
    public async Task<ActionResult<RegionDto>> CreateRegion([FromBody] CreateRegionRequest request)
        => Ok(await _regionService.CreateAsync(request));

    [HttpPut("regions/{id:long}")]
    public async Task<IActionResult> UpdateRegion(long id, [FromBody] UpdateRegionRequest request)
    {
        await _regionService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpGet("units")]
    public async Task<ActionResult<PageResult<UnitDto>>> SearchUnits(
        [FromQuery] string? keyword, [FromQuery] int? status,
        [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
        => Ok(await _unitService.SearchAsync(keyword, status, new PageRequest(pageIndex, pageSize)));

    [HttpGet("units/{id:long}")]
    public async Task<ActionResult<UnitDto>> GetUnit(long id)
        => Ok(await _unitService.GetByIdAsync(id));

    [HttpGet("units/children")]
    public async Task<ActionResult<IReadOnlyList<UnitDto>>> GetUnitChildren([FromQuery] long? parentId)
        => Ok(await _unitService.GetChildrenAsync(parentId));

    [HttpPost("units")]
    public async Task<ActionResult<UnitDto>> CreateUnit([FromBody] CreateUnitRequest request)
        => Ok(await _unitService.CreateAsync(request));

    [HttpPut("units/{id:long}")]
    public async Task<IActionResult> UpdateUnit(long id, [FromBody] UpdateUnitRequest request)
    {
        await _unitService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpPut("units/{id:long}/enable")]
    public async Task<IActionResult> EnableUnit(long id)
    {
        await _unitService.EnableAsync(id);
        return NoContent();
    }

    [HttpPut("units/{id:long}/disable")]
    public async Task<IActionResult> DisableUnit(long id)
    {
        await _unitService.DisableAsync(id);
        return NoContent();
    }

    [HttpGet("units/{unitId:long}/departments")]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetDepartmentsByUnit(long unitId)
        => Ok(await _deptService.GetByUnitAsync(unitId));

    [HttpGet("departments/{id:long}")]
    public async Task<ActionResult<DepartmentDto>> GetDepartment(long id)
        => Ok(await _deptService.GetByIdAsync(id));

    [HttpGet("departments/children")]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetDepartmentChildren(
        [FromQuery] long unitId, [FromQuery] long? parentId)
        => Ok(await _deptService.GetChildrenAsync(unitId, parentId));

    [HttpPost("departments")]
    public async Task<ActionResult<DepartmentDto>> CreateDepartment([FromBody] CreateDepartmentRequest request)
        => Ok(await _deptService.CreateAsync(request));

    [HttpPut("departments/{id:long}")]
    public async Task<IActionResult> UpdateDepartment(long id, [FromBody] UpdateDepartmentRequest request)
    {
        await _deptService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpPut("departments/{id:long}/enable")]
    public async Task<IActionResult> EnableDepartment(long id)
    {
        await _deptService.EnableAsync(id);
        return NoContent();
    }

    [HttpPut("departments/{id:long}/disable")]
    public async Task<IActionResult> DisableDepartment(long id)
    {
        await _deptService.DisableAsync(id);
        return NoContent();
    }

    [HttpGet("positions")]
    public async Task<ActionResult<PageResult<PositionDto>>> SearchPositions(
        [FromQuery] string? keyword, [FromQuery] int? status,
        [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
        => Ok(await _positionService.SearchAsync(keyword, status, new PageRequest(pageIndex, pageSize)));

    [HttpGet("positions/{id:long}")]
    public async Task<ActionResult<PositionDto>> GetPosition(long id)
        => Ok(await _positionService.GetByIdAsync(id));

    [HttpPost("positions")]
    public async Task<ActionResult<PositionDto>> CreatePosition([FromBody] CreatePositionRequest request)
        => Ok(await _positionService.CreateAsync(request));

    [HttpPut("positions/{id:long}")]
    public async Task<IActionResult> UpdatePosition(long id, [FromBody] UpdatePositionRequest request)
    {
        await _positionService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpPut("positions/{id:long}/enable")]
    public async Task<IActionResult> EnablePosition(long id)
    {
        await _positionService.EnableAsync(id);
        return NoContent();
    }

    [HttpPut("positions/{id:long}/disable")]
    public async Task<IActionResult> DisablePosition(long id)
    {
        await _positionService.DisableAsync(id);
        return NoContent();
    }
}
