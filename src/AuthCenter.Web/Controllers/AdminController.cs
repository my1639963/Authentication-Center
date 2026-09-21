using AuthCenter.Application.DTOs;
using AuthCenter.Application.Services;
using AuthCenter.OpenIddict.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthCenter.Web.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IAdminRoleService _roleService;
    private readonly IAdminPermissionService _permService;
    private readonly IUserAdminService _userAdminService;

    public AdminController(
        IAdminRoleService roleService,
        IAdminPermissionService permService,
        IUserAdminService userAdminService)
    {
        _roleService = roleService;
        _permService = permService;
        _userAdminService = userAdminService;
    }

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpGet("roles")]
    public async Task<ActionResult<IReadOnlyList<AdminRoleDto>>> GetAllRoles()
        => Ok(await _roleService.GetAllAsync());

    [HttpGet("roles/{id:long}")]
    public async Task<ActionResult<AdminRoleDto>> GetRole(long id)
        => Ok(await _roleService.GetByIdAsync(id));

    [HttpPost("roles")]
    public async Task<ActionResult<AdminRoleDto>> CreateRole([FromBody] CreateAdminRoleRequest request)
        => Ok(await _roleService.CreateAsync(request));

    [HttpPut("roles/{id:long}")]
    public async Task<IActionResult> UpdateRole(long id, [FromBody] UpdateAdminRoleRequest request)
    {
        await _roleService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpPut("roles/{id:long}/permissions")]
    public async Task<IActionResult> AssignPermissions(long id, [FromBody] AssignPermissionsRequest request)
    {
        await _roleService.AssignPermissionsAsync(id, request);
        return NoContent();
    }

    [HttpGet("roles/{id:long}/permissions")]
    public async Task<ActionResult<IReadOnlyList<AdminPermissionDto>>> GetRolePermissions(long id)
        => Ok(await _roleService.GetRolePermissionsAsync(id));

    [HttpGet("permissions")]
    public async Task<ActionResult<IReadOnlyList<AdminPermissionDto>>> GetAllPermissions()
        => Ok(await _permService.GetAllAsync());

    [HttpGet("users/{userId:long}/roles")]
    public async Task<ActionResult<IReadOnlyList<AdminRoleDto>>> GetUserRoles(long userId)
        => Ok(await _userAdminService.GetUserRolesAsync(userId));

    [HttpPut("users/{userId:long}/roles")]
    public async Task<IActionResult> AssignRoles(long userId, [FromBody] AssignRolesRequest request)
    {
        await _userAdminService.AssignRolesAsync(userId, request, GetCurrentUserId());
        return NoContent();
    }

    [HttpGet("users/{userId:long}/permissions")]
    public async Task<ActionResult<IReadOnlyList<AdminPermissionDto>>> GetUserPermissions(long userId)
        => Ok(await _userAdminService.GetUserPermissionsAsync(userId));
}

[ApiController]
[Route("api/oauth-clients")]
[Authorize]
public class OAuthClientController : ControllerBase
{
    private readonly IOAuthClientService _clientService;

    public OAuthClientController(IOAuthClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OAuthClientDto>>> GetAll()
        => Ok(await _clientService.GetAllAsync());

    [HttpGet("{clientId}")]
    public async Task<ActionResult<OAuthClientDto>> GetByClientId(string clientId)
    {
        var result = await _clientService.GetByClientIdAsync(clientId);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<OAuthClientDto>> Create([FromBody] CreateOAuthClientRequest request)
    {
        var result = await _clientService.CreateAsync(request);
        return CreatedAtAction(nameof(GetByClientId), new { clientId = result.ClientId }, result);
    }

    [HttpPut("{clientId}")]
    public async Task<IActionResult> Update(string clientId, [FromBody] UpdateOAuthClientRequest request)
    {
        await _clientService.UpdateAsync(clientId, request);
        return NoContent();
    }

    [HttpDelete("{clientId}")]
    public async Task<IActionResult> Delete(string clientId)
    {
        await _clientService.DeleteAsync(clientId);
        return NoContent();
    }
}
