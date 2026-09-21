using System.Security.Claims;
using AuthCenter.Application.DTOs;
using AuthCenter.Domain.Entities;
using AuthCenter.Domain.Enums;
using AuthCenter.Domain.Repositories;
using AuthCenter.Domain.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace AuthCenter.Web.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly ILoginLogRepository _loginLogRepo;
    private readonly ICryptoProvider _crypto;
    private readonly IIdGenerator _idGenerator;

    private const int MaxLoginFailCount = 5;

    public AuthController(
        IUserRepository userRepo,
        ILoginLogRepository loginLogRepo,
        ICryptoProvider crypto,
        IIdGenerator idGenerator)
    {
        _userRepo = userRepo;
        _loginLogRepo = loginLogRepo;
        _crypto = crypto;
        _idGenerator = idGenerator;
    }

    /// <summary>
    /// OAuth2 Token Endpoint - 处理密码登录并签发令牌
    /// </summary>
    [HttpPost("~/oauth2/token"), Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var now = DateTime.Now;

        if (!request.IsPasswordGrantType())
        {
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.UnsupportedGrantType,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "仅支持密码模式登录。"
                }));
        }

        var loginName = request.Username;
        var user = await _userRepo.GetByLoginNameAsync(loginName!);

        // 用户不存在
        if (user == null)
        {
            await LogLoginAsync(null, loginName, LoginResult.Failed, "用户不存在", ipAddress, now);
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "用户名或密码错误。"
                }));
        }

        // 账号已删除
        if (user.IsDeleted == 1)
        {
            await LogLoginAsync(user.Id, loginName, LoginResult.Failed, "账号已删除", ipAddress, now);
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "账号已删除。"
                }));
        }

        // 账号已禁用
        if (user.Status == (int)UserStatus.Disabled)
        {
            await LogLoginAsync(user.Id, loginName, LoginResult.Failed, "账号已禁用", ipAddress, now);
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "账号已禁用。"
                }));
        }

        // 账号已锁定且在锁定期内
        if (user.Status == (int)UserStatus.Locked && user.LockUntil.HasValue && user.LockUntil > now)
        {
            await LogLoginAsync(user.Id, loginName, LoginResult.Failed, "账号已锁定", ipAddress, now);
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = $"账号已锁定，请在 {user.LockUntil:yyyy-MM-dd HH:mm:ss} 后重试。"
                }));
        }

        // 锁定已过期，恢复正常状态
        if (user.Status == (int)UserStatus.Locked && user.LockUntil.HasValue && user.LockUntil <= now)
        {
            user.Status = (int)UserStatus.Normal;
            user.LoginFailCount = 0;
            user.LockUntil = null;
        }

        // 验证密码
        if (!_crypto.VerifyPassword(request.Password!, user.PasswordHash))
        {
            user.LoginFailCount++;
            if (user.LoginFailCount >= MaxLoginFailCount)
            {
                user.Status = (int)UserStatus.Locked;
                user.LockUntil = now.AddMinutes(30);
            }
            user.UpdateTime = now;
            await _userRepo.UpdateAsync(user);

            await LogLoginAsync(user.Id, loginName, LoginResult.Failed,
                $"密码错误（第{user.LoginFailCount}次）", ipAddress, now);

            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "用户名或密码错误。"
                }));
        }

        // 登录成功，重置失败计数
        user.LoginFailCount = 0;
        user.LastLoginTime = now;
        user.LastLoginIp = ipAddress;
        user.UpdateTime = now;
        await _userRepo.UpdateAsync(user);

        await LogLoginAsync(user.Id, loginName, LoginResult.Success, null, ipAddress, now);

        // 构建 ClaimsPrincipal，由 OpenIddict 签发令牌
        var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        identity.AddClaim(OpenIddictConstants.Claims.Subject, user.Id.ToString());
        identity.AddClaim(OpenIddictConstants.Claims.Name, user.LoginName);
        identity.AddClaim("real_name", user.RealName);

        var principal = new ClaimsPrincipal(identity);
        principal.SetScopes(new[]
        {
            OpenIddictConstants.Scopes.OfflineAccess
        });

        await HttpContext.SignInAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, principal);

        // OpenIddict passthrough 会自动处理响应，这里返回 EmptyResult 即可
        return new EmptyResult();
    }

    private async Task LogLoginAsync(long? userId, string? loginName, LoginResult result, string? failReason, string? ipAddress, DateTime createTime)
    {
        var log = new SysLoginLog
        {
            Id = _idGenerator.NewId(),
            UserId = userId,
            LoginName = loginName,
            LoginType = nameof(LoginType.Password),
            LoginResult = (int)result,
            FailReason = failReason,
            IpAddress = ipAddress,
            UserAgent = HttpContext.Request.Headers.UserAgent.ToString(),
            CreateTime = createTime
        };
        await _loginLogRepo.AddAsync(log);
    }
}
