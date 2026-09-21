using AuthCenter.Application.DTOs;
using AuthCenter.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthCenter.Web.Controllers;

[ApiController]
[Route("api/logs")]
[Authorize]
public class LogController : ControllerBase
{
    private readonly IAuditService _auditService;

    public LogController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpGet("login")]
    public async Task<ActionResult<PageResult<LoginLogDto>>> SearchLoginLogs(
        [FromQuery] long? userId, [FromQuery] string? clientId,
        [FromQuery] string? loginType, [FromQuery] int? result,
        [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime,
        [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var query = new LoginLogQuery(userId, clientId, loginType, result, startTime, endTime, pageIndex, pageSize);
        return Ok(await _auditService.SearchLoginLogsAsync(query));
    }

    [HttpGet("audit")]
    public async Task<ActionResult<PageResult<AuditLogDto>>> SearchAuditLogs(
        [FromQuery] string? eventType, [FromQuery] string? operationType,
        [FromQuery] long? operateUserId, [FromQuery] long? targetUserId,
        [FromQuery] int? result,
        [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime,
        [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var query = new AuditLogQuery(eventType, operationType, operateUserId, targetUserId, result, startTime, endTime, pageIndex, pageSize);
        return Ok(await _auditService.SearchAuditLogsAsync(query));
    }
}
