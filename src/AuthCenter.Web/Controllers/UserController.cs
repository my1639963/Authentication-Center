using AuthCenter.Application.DTOs;
using AuthCenter.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthCenter.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpGet("{id:long}")]
    public async Task<ActionResult<UserDetailDto>> GetById(long id)
    {
        var result = await _userService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<UserDto>>> Search(
        [FromQuery] string? keyword, [FromQuery] int? status,
        [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _userService.SearchAsync(keyword, status, new PageRequest(pageIndex, pageSize));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<UserDetailDto>> Create([FromBody] CreateUserRequest request)
    {
        var result = await _userService.CreateAsync(request, GetCurrentUserId());
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateUserRequest request)
    {
        await _userService.UpdateAsync(id, request, GetCurrentUserId());
        return NoContent();
    }

    [HttpPut("{id:long}/enable")]
    public async Task<IActionResult> Enable(long id)
    {
        await _userService.EnableAsync(id, GetCurrentUserId());
        return NoContent();
    }

    [HttpPut("{id:long}/disable")]
    public async Task<IActionResult> Disable(long id)
    {
        await _userService.DisableAsync(id, GetCurrentUserId());
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _userService.DeleteAsync(id, GetCurrentUserId());
        return NoContent();
    }

    [HttpPost("{id:long}/reset-password")]
    public async Task<IActionResult> ResetPassword(long id, [FromBody] ResetPasswordRequest request)
    {
        await _userService.ResetPasswordAsync(id, request, GetCurrentUserId());
        return NoContent();
    }

    [HttpPut("{userId:long}/main-organization")]
    public async Task<IActionResult> ChangeMainOrganization(long userId, [FromBody] ChangeMainOrganizationRequest request)
    {
        await _userService.ChangeMainOrganizationAsync(userId, request, GetCurrentUserId());
        return NoContent();
    }
}
