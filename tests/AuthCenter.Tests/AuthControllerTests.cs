using AuthCenter.Domain.Entities;
using AuthCenter.Domain.Enums;
using AuthCenter.Domain.Repositories;
using AuthCenter.Domain.Services;
using AuthCenter.Web.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;

namespace AuthCenter.Tests;

/// <summary>
/// Minimal IServiceProvider that resolves IAuthenticationService from a mock.
/// </summary>
internal class MockServiceProvider : IServiceProvider
{
    private readonly IAuthenticationService _authService;
    public MockServiceProvider(IAuthenticationService authService) => _authService = authService;
    public object? GetService(Type serviceType)
    {
        if (serviceType == typeof(IAuthenticationService)) return _authService;
        return null;
    }
}

public class AuthControllerTests
{
    private readonly Mock<IUserRepository> _userRepo;
    private readonly Mock<ILoginLogRepository> _loginLogRepo;
    private readonly Mock<ICryptoProvider> _crypto;
    private readonly Mock<IIdGenerator> _idGenerator;
    private readonly AuthController _sut;
    private readonly DefaultHttpContext _httpContext;

    public AuthControllerTests()
    {
        _userRepo = new Mock<IUserRepository>();
        _loginLogRepo = new Mock<ILoginLogRepository>();
        _crypto = new Mock<ICryptoProvider>();
        _idGenerator = new Mock<IIdGenerator>();
        _idGenerator.Setup(x => x.NewId()).Returns(1);

        _sut = new AuthController(_userRepo.Object, _loginLogRepo.Object, _crypto.Object, _idGenerator.Object);

        _httpContext = new DefaultHttpContext
        {
            Connection = { RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1") }
        };
        _httpContext.Request.Headers.UserAgent = "TestAgent/1.0";

        var authMock = new Mock<IAuthenticationService>();
        authMock.Setup(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<System.Security.Claims.ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);
        _httpContext.RequestServices = new MockServiceProvider(authMock.Object);

        _sut.ControllerContext = new ControllerContext { HttpContext = _httpContext };
    }

    /// <summary>
    /// Sets up the OpenIddict server request on the HttpContext so that
    /// HttpContext.GetOpenIddictServerRequest() returns the specified request.
    /// </summary>
    private void SetOpenIddictRequest(OpenIddictRequest request)
    {
        var transaction = new OpenIddictServerTransaction { Request = request };
        var feature = new OpenIddictServerAspNetCoreFeature { Transaction = transaction };
        _httpContext.Features.Set(feature);
    }

    private static SysUser CreateActiveUser(string loginName = "testuser", string passwordHash = "correct_hash") => new()
    {
        Id = 1,
        LoginName = loginName,
        PasswordHash = passwordHash,
        RealName = "测试用户",
        Status = (int)UserStatus.Normal,
        IsDeleted = 0,
        LoginFailCount = 0,
        CreateTime = DateTime.Now,
        UpdateTime = DateTime.Now
    };

    #region Exchange (Login)

    [Fact]
    public async Task Exchange_ValidCredentials_ReturnsOkAndLogsSuccess()
    {
        var user = CreateActiveUser();
        _userRepo.Setup(r => r.GetByLoginNameAsync("testuser")).ReturnsAsync(user);
        _crypto.Setup(c => c.VerifyPassword("correct_password", "correct_hash")).Returns(true);
        SetOpenIddictRequest(new OpenIddictRequest
        {
            GrantType = OpenIddictConstants.GrantTypes.Password,
            Username = "testuser",
            Password = "correct_password"
        });

        var result = await _sut.Exchange();

        result.Should().BeOfType<EmptyResult>();
        user.LoginFailCount.Should().Be(0);
        _userRepo.Verify(r => r.UpdateAsync(user, default), Times.Once);
        _loginLogRepo.Verify(r => r.AddAsync(It.Is<SysLoginLog>(l =>
            l.LoginResult == (int)LoginResult.Success && l.LoginName == "testuser"), default), Times.Once);
    }

    [Fact]
    public async Task Exchange_UserNotFound_ReturnsForbid()
    {
        _userRepo.Setup(r => r.GetByLoginNameAsync("nonexistent")).ReturnsAsync((SysUser?)null);
        SetOpenIddictRequest(new OpenIddictRequest
        {
            GrantType = OpenIddictConstants.GrantTypes.Password,
            Username = "nonexistent",
            Password = "password"
        });

        var result = await _sut.Exchange();

        result.Should().BeOfType<ForbidResult>();
        _loginLogRepo.Verify(r => r.AddAsync(It.Is<SysLoginLog>(l =>
            l.LoginResult == (int)LoginResult.Failed && l.FailReason == "用户不存在"), default), Times.Once);
    }

    [Fact]
    public async Task Exchange_DeletedUser_ReturnsForbid()
    {
        var user = CreateActiveUser();
        user.IsDeleted = 1;
        _userRepo.Setup(r => r.GetByLoginNameAsync("testuser")).ReturnsAsync(user);
        SetOpenIddictRequest(new OpenIddictRequest
        {
            GrantType = OpenIddictConstants.GrantTypes.Password,
            Username = "testuser",
            Password = "password"
        });

        var result = await _sut.Exchange();

        result.Should().BeOfType<ForbidResult>();
        _loginLogRepo.Verify(r => r.AddAsync(It.Is<SysLoginLog>(l =>
            l.FailReason == "账号已删除"), default), Times.Once);
    }

    [Fact]
    public async Task Exchange_DisabledUser_ReturnsForbid()
    {
        var user = CreateActiveUser();
        user.Status = (int)UserStatus.Disabled;
        _userRepo.Setup(r => r.GetByLoginNameAsync("testuser")).ReturnsAsync(user);
        SetOpenIddictRequest(new OpenIddictRequest
        {
            GrantType = OpenIddictConstants.GrantTypes.Password,
            Username = "testuser",
            Password = "password"
        });

        var result = await _sut.Exchange();

        result.Should().BeOfType<ForbidResult>();
        _loginLogRepo.Verify(r => r.AddAsync(It.Is<SysLoginLog>(l =>
            l.FailReason == "账号已禁用"), default), Times.Once);
    }

    [Fact]
    public async Task Exchange_LockedUser_ReturnsForbid()
    {
        var user = CreateActiveUser();
        user.Status = (int)UserStatus.Locked;
        user.LockUntil = DateTime.Now.AddMinutes(30);
        _userRepo.Setup(r => r.GetByLoginNameAsync("testuser")).ReturnsAsync(user);
        SetOpenIddictRequest(new OpenIddictRequest
        {
            GrantType = OpenIddictConstants.GrantTypes.Password,
            Username = "testuser",
            Password = "password"
        });

        var result = await _sut.Exchange();

        result.Should().BeOfType<ForbidResult>();
        _loginLogRepo.Verify(r => r.AddAsync(It.Is<SysLoginLog>(l =>
            l.FailReason == "账号已锁定"), default), Times.Once);
    }

    [Fact]
    public async Task Exchange_ExpiredLock_ResetsStatus()
    {
        var user = CreateActiveUser();
        user.Status = (int)UserStatus.Locked;
        user.LockUntil = DateTime.Now.AddMinutes(-1);
        user.LoginFailCount = 5;
        _userRepo.Setup(r => r.GetByLoginNameAsync("testuser")).ReturnsAsync(user);
        _crypto.Setup(c => c.VerifyPassword("correct_password", "correct_hash")).Returns(true);
        SetOpenIddictRequest(new OpenIddictRequest
        {
            GrantType = OpenIddictConstants.GrantTypes.Password,
            Username = "testuser",
            Password = "correct_password"
        });

        var result = await _sut.Exchange();

        result.Should().BeOfType<EmptyResult>();
        user.Status.Should().Be((int)UserStatus.Normal);
        user.LoginFailCount.Should().Be(0);
        user.LockUntil.Should().BeNull();
    }

    [Fact]
    public async Task Exchange_WrongPassword_IncrementsFailCount()
    {
        var user = CreateActiveUser();
        user.LoginFailCount = 2;
        _userRepo.Setup(r => r.GetByLoginNameAsync("testuser")).ReturnsAsync(user);
        _crypto.Setup(c => c.VerifyPassword("wrong_password", "correct_hash")).Returns(false);
        SetOpenIddictRequest(new OpenIddictRequest
        {
            GrantType = OpenIddictConstants.GrantTypes.Password,
            Username = "testuser",
            Password = "wrong_password"
        });

        var result = await _sut.Exchange();

        result.Should().BeOfType<ForbidResult>();
        user.LoginFailCount.Should().Be(3);
        _loginLogRepo.Verify(r => r.AddAsync(It.Is<SysLoginLog>(l =>
            l.LoginResult == (int)LoginResult.Failed), default), Times.Once);
    }

    [Fact]
    public async Task Exchange_WrongPasswordExceedsLimit_LocksAccount()
    {
        var user = CreateActiveUser();
        user.LoginFailCount = 4;
        _userRepo.Setup(r => r.GetByLoginNameAsync("testuser")).ReturnsAsync(user);
        _crypto.Setup(c => c.VerifyPassword("wrong_password", "correct_hash")).Returns(false);
        SetOpenIddictRequest(new OpenIddictRequest
        {
            GrantType = OpenIddictConstants.GrantTypes.Password,
            Username = "testuser",
            Password = "wrong_password"
        });

        var result = await _sut.Exchange();

        result.Should().BeOfType<ForbidResult>();
        user.LoginFailCount.Should().Be(5);
        user.Status.Should().Be((int)UserStatus.Locked);
        user.LockUntil.Should().NotBeNull();
    }

    #endregion
}
