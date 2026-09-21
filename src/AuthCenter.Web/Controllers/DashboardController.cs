using AuthCenter.Application.DTOs;
using AuthCenter.Application.Services;
using AuthCenter.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthCenter.Web.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly IAdminRoleRepository _roleRepo;
    private readonly IAdminPermissionRepository _permRepo;
    private readonly IUnitRepository _unitRepo;
    private readonly IDepartmentRepository _deptRepo;
    private readonly IPositionRepository _posRepo;
    private readonly ILoginLogRepository _loginLogRepo;
    private readonly IAuditLogRepository _auditLogRepo;

    public DashboardController(
        IUserRepository userRepo,
        IAdminRoleRepository roleRepo,
        IAdminPermissionRepository permRepo,
        IUnitRepository unitRepo,
        IDepartmentRepository deptRepo,
        IPositionRepository posRepo,
        ILoginLogRepository loginLogRepo,
        IAuditLogRepository auditLogRepo)
    {
        _userRepo = userRepo;
        _roleRepo = roleRepo;
        _permRepo = permRepo;
        _unitRepo = unitRepo;
        _deptRepo = deptRepo;
        _posRepo = posRepo;
        _loginLogRepo = loginLogRepo;
        _auditLogRepo = auditLogRepo;
    }

    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetStats()
    {
        var userCount = await _userRepo.CountAsync(null, null);
        var roleList = await _roleRepo.GetAllAsync();
        var permList = await _permRepo.GetAllAsync();
        var unitCount = await _unitRepo.CountAsync(null, null);
        var posCount = await _posRepo.CountAsync(null, null);

        // 获取所有单位下的部门总数
        var units = await _unitRepo.GetChildrenAsync(null);
        var totalDepts = 0;
        foreach (var unit in units)
        {
            var depts = await _deptRepo.GetByUnitAsync(unit.Id);
            totalDepts += depts.Count;
        }

        // 今日登录统计
        var todayStart = DateTime.Today;
        var todayEnd = todayStart.AddDays(1);
        var todaySuccess = await _loginLogRepo.CountAsync(null, null, null, 0, todayStart, todayEnd);
        var todayFail = await _loginLogRepo.CountAsync(null, null, null, 1, todayStart, todayEnd);

        return Ok(new DashboardStatsDto(
            UserCount: userCount,
            RoleCount: roleList.Count,
            PermissionCount: permList.Count,
            UnitCount: unitCount,
            DepartmentCount: totalDepts,
            PositionCount: posCount,
            TodayLoginSuccess: todaySuccess,
            TodayLoginFail: todayFail
        ));
    }

    [HttpGet("recent-logins")]
    public async Task<ActionResult<IReadOnlyList<LoginLogDto>>> GetRecentLogins([FromQuery] int count = 5)
    {
        var logs = await _loginLogRepo.SearchAsync(null, null, null, null, null, null, 1, count);
        return Ok(logs.Select(l => new LoginLogDto(
            l.Id, l.UserId, l.LoginName, l.ClientId, l.LoginType,
            l.LoginResult, l.FailReason, l.IpAddress, l.CreateTime
        )).ToList());
    }

    [HttpGet("recent-audits")]
    public async Task<ActionResult<IReadOnlyList<AuditLogDto>>> GetRecentAudits([FromQuery] int count = 5)
    {
        var logs = await _auditLogRepo.SearchAsync(null, null, null, null, null, null, null, 1, count);
        return Ok(logs.Select(l => new AuditLogDto(
            l.Id, l.EventType, l.OperationType, l.OperateUserId, l.TargetUserId,
            l.ClientId, l.EventResult, l.FailReason, l.Content, l.CreateTime
        )).ToList());
    }
}

public record DashboardStatsDto(
    int UserCount,
    int RoleCount,
    int PermissionCount,
    int UnitCount,
    int DepartmentCount,
    int PositionCount,
    int TodayLoginSuccess,
    int TodayLoginFail
);
