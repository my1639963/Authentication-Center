using AuthCenter.Application.DTOs;
using AuthCenter.Application.Services;
using AuthCenter.Domain.Entities;
using AuthCenter.Domain.Repositories;
using AuthCenter.Domain.Services;
using FluentAssertions;
using Moq;

namespace AuthCenter.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepo;
    private readonly Mock<IUserOrganizationRepository> _userOrgRepo;
    private readonly Mock<IUserMainOrganizationRepository> _mainOrgRepo;
    private readonly Mock<IUnitRepository> _unitRepo;
    private readonly Mock<IDepartmentRepository> _deptRepo;
    private readonly Mock<IPositionRepository> _positionRepo;
    private readonly Mock<IPasswordHistoryRepository> _pwdHistoryRepo;
    private readonly Mock<ICryptoProvider> _crypto;
    private readonly Mock<IIdGenerator> _idGenerator;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _userRepo = new Mock<IUserRepository>();
        _userOrgRepo = new Mock<IUserOrganizationRepository>();
        _mainOrgRepo = new Mock<IUserMainOrganizationRepository>();
        _unitRepo = new Mock<IUnitRepository>();
        _deptRepo = new Mock<IDepartmentRepository>();
        _positionRepo = new Mock<IPositionRepository>();
        _pwdHistoryRepo = new Mock<IPasswordHistoryRepository>();
        _crypto = new Mock<ICryptoProvider>();
        _idGenerator = new Mock<IIdGenerator>();

        _idGenerator.Setup(x => x.NewId()).Returns(() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

        _sut = new UserService(
            _userRepo.Object, _userOrgRepo.Object, _mainOrgRepo.Object,
            _unitRepo.Object, _deptRepo.Object, _positionRepo.Object,
            _pwdHistoryRepo.Object, _crypto.Object, _idGenerator.Object);
    }

    private static SysUser CreateTestUser(long id = 1, string loginName = "testuser", string realName = "测试用户") => new()
    {
        Id = id,
        LoginName = loginName,
        RealName = realName,
        PasswordHash = "hashed",
        Status = 0,
        CreateTime = DateTime.Now,
        UpdateTime = DateTime.Now
    };

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ExistingUser_ReturnsDetailDto()
    {
        var user = CreateTestUser();
        _userRepo.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(user);
        _userOrgRepo.Setup(r => r.GetByUserIdAsync(1, default)).ReturnsAsync(new List<SysUserOrganization>());
        _mainOrgRepo.Setup(r => r.GetByUserIdAsync(1, default)).ReturnsAsync((SysUserMainOrganization?)null);

        var result = await _sut.GetByIdAsync(1);

        result.Id.Should().Be(1);
        result.LoginName.Should().Be("testuser");
        result.RealName.Should().Be("测试用户");
        result.Organizations.Should().BeEmpty();
        result.MainOrganization.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingUser_ThrowsException()
    {
        _userRepo.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((SysUser?)null);

        var act = () => _sut.GetByIdAsync(999);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*999*not found*");
    }

    [Fact]
    public async Task GetByIdAsync_WithOrganizations_MapsCorrectly()
    {
        var user = CreateTestUser();
        var unit = new SysUnit { Id = 10, UnitName = "测试单位", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
        var userOrg = new SysUserOrganization
        {
            Id = 100, UserId = 1, UnitId = 10, Status = 0,
            CreateTime = DateTime.Now, UpdateTime = DateTime.Now
        };

        _userRepo.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(user);
        _userOrgRepo.Setup(r => r.GetByUserIdAsync(1, default)).ReturnsAsync(new List<SysUserOrganization> { userOrg });
        _unitRepo.Setup(r => r.GetByIdAsync(10, default)).ReturnsAsync(unit);
        _deptRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>(), default)).ReturnsAsync((SysDepartment?)null);
        _positionRepo.Setup(r => r.GetByIdAsync(It.IsAny<long>(), default)).ReturnsAsync((SysPosition?)null);
        _mainOrgRepo.Setup(r => r.GetByUserIdAsync(1, default)).ReturnsAsync((SysUserMainOrganization?)null);

        var result = await _sut.GetByIdAsync(1);

        result.Organizations.Should().HaveCount(1);
        result.Organizations[0].UnitName.Should().Be("测试单位");
    }

    #endregion

    #region SearchAsync

    [Fact]
    public async Task SearchAsync_WithKeyword_ReturnsPagedResult()
    {
        var users = new List<SysUser> { CreateTestUser(1, "admin", "管理员"), CreateTestUser(2, "admin2", "管理员2") };
        _userRepo.Setup(r => r.SearchAsync("admin", null, 1, 20, default)).ReturnsAsync(users);
        _userRepo.Setup(r => r.CountAsync("admin", null, default)).ReturnsAsync(2);

        var result = await _sut.SearchAsync("admin", null, new PageRequest(1, 20));

        result.Total.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Items[0].LoginName.Should().Be("admin");
    }

    [Fact]
    public async Task SearchAsync_NoResults_ReturnsEmpty()
    {
        _userRepo.Setup(r => r.SearchAsync("nonexistent", null, 1, 20, default)).ReturnsAsync(new List<SysUser>());
        _userRepo.Setup(r => r.CountAsync("nonexistent", null, default)).ReturnsAsync(0);

        var result = await _sut.SearchAsync("nonexistent", null, new PageRequest(1, 20));

        result.Total.Should().Be(0);
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchAsync_WithStatusFilter_PassesToRepo()
    {
        _userRepo.Setup(r => r.SearchAsync(null, 0, 1, 10, default)).ReturnsAsync(new List<SysUser> { CreateTestUser() });
        _userRepo.Setup(r => r.CountAsync(null, 0, default)).ReturnsAsync(1);

        var result = await _sut.SearchAsync(null, 0, new PageRequest(1, 10));

        result.Total.Should().Be(1);
        _userRepo.Verify(r => r.SearchAsync(null, 0, 1, 10, default), Times.Once);
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_ValidRequest_CreatesUser()
    {
        var request = new CreateUserRequest("newuser", "P@ss123", "新用户", "13800000000", "test@test.com", null, 10, null, null, null);
        _userRepo.Setup(r => r.ExistsByLoginNameAsync("newuser", default)).ReturnsAsync(false);
        _crypto.Setup(c => c.HashPassword("P@ss123")).Returns("hashed_password");
        _userOrgRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<long>(), default)).ReturnsAsync(new List<SysUserOrganization>());
        _mainOrgRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<long>(), default)).ReturnsAsync((SysUserMainOrganization?)null);

        var result = await _sut.CreateAsync(request, 0);

        result.LoginName.Should().Be("newuser");
        result.RealName.Should().Be("新用户");
        _userRepo.Verify(r => r.AddAsync(It.Is<SysUser>(u =>
            u.LoginName == "newuser" &&
            u.PasswordHash == "hashed_password" &&
            u.Status == 0 &&
            u.MustModifyPwd == 1), default), Times.Once);
        _userOrgRepo.Verify(r => r.AddAsync(It.Is<SysUserOrganization>(o => o.UnitId == 10), default), Times.Once);
        _mainOrgRepo.Verify(r => r.AddAsync(It.IsAny<SysUserMainOrganization>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DuplicateLoginName_ThrowsException()
    {
        var request = new CreateUserRequest("existing", "P@ss123", "已存在", null, null, null, 10, null, null, null);
        _userRepo.Setup(r => r.ExistsByLoginNameAsync("existing", default)).ReturnsAsync(true);

        var act = () => _sut.CreateAsync(request, 0);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*existing*already exists*");
        _userRepo.Verify(r => r.AddAsync(It.IsAny<SysUser>(), default), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithIdCardNo_EncryptsIdCard()
    {
        var request = new CreateUserRequest("idcarduser", "P@ss123", "身份证用户", null, null, "110101199001011234", 10, null, null, null);
        _userRepo.Setup(r => r.ExistsByLoginNameAsync("idcarduser", default)).ReturnsAsync(false);
        _crypto.Setup(c => c.HashPassword("P@ss123")).Returns("hashed");
        _crypto.Setup(c => c.Encrypt("110101199001011234")).Returns("encrypted_idcard");
        _crypto.Setup(c => c.ComputeHmac("110101199001011234")).Returns("hmac_idcard");
        _userOrgRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<long>(), default)).ReturnsAsync(new List<SysUserOrganization>());
        _mainOrgRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<long>(), default)).ReturnsAsync((SysUserMainOrganization?)null);

        var result = await _sut.CreateAsync(request, 0);

        _userRepo.Verify(r => r.AddAsync(It.Is<SysUser>(u =>
            u.IdCardCiphertext == "encrypted_idcard" &&
            u.IdCardHash == "hmac_idcard"), default), Times.Once);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_ExistingUser_UpdatesFields()
    {
        var user = CreateTestUser();
        _userRepo.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(user);

        await _sut.UpdateAsync(1, new UpdateUserRequest("新姓名", "13900000000", "new@test.com"), 0);

        user.RealName.Should().Be("新姓名");
        user.Mobile.Should().Be("13900000000");
        user.Email.Should().Be("new@test.com");
        _userRepo.Verify(r => r.UpdateAsync(user, default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingUser_ThrowsException()
    {
        _userRepo.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((SysUser?)null);

        var act = () => _sut.UpdateAsync(999, new UpdateUserRequest("x", null, null), 0);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate_OnlyUpdatesProvidedFields()
    {
        var user = CreateTestUser();
        user.Mobile = "old_mobile";
        user.Email = "old@test.com";
        _userRepo.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(user);

        await _sut.UpdateAsync(1, new UpdateUserRequest("新姓名", null, null), 0);

        user.RealName.Should().Be("新姓名");
        user.Mobile.Should().Be("old_mobile");
        user.Email.Should().Be("old@test.com");
    }

    #endregion

    #region DisableAsync / EnableAsync

    [Fact]
    public async Task DisableAsync_ExistingUser_SetsStatusToDisabled()
    {
        var user = CreateTestUser();
        _userRepo.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(user);

        await _sut.DisableAsync(1, 0);

        user.Status.Should().Be(1);
        _userRepo.Verify(r => r.UpdateAsync(user, default), Times.Once);
    }

    [Fact]
    public async Task EnableAsync_DisabledUser_SetsStatusToNormal()
    {
        var user = CreateTestUser();
        user.Status = 1;
        user.LoginFailCount = 3;
        _userRepo.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(user);

        await _sut.EnableAsync(1, 0);

        user.Status.Should().Be(0);
        user.LoginFailCount.Should().Be(0);
    }

    [Fact]
    public async Task DisableAsync_NonExistingUser_ThrowsException()
    {
        _userRepo.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((SysUser?)null);

        var act = () => _sut.DisableAsync(999, 0);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_ExistingUser_SoftDeletes()
    {
        var user = CreateTestUser();
        _userRepo.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(user);

        await _sut.DeleteAsync(1, 0);

        user.IsDeleted.Should().Be(1);
        _userRepo.Verify(r => r.UpdateAsync(user, default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingUser_ThrowsException()
    {
        _userRepo.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((SysUser?)null);

        var act = () => _sut.DeleteAsync(999, 0);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    #endregion

    #region ResetPasswordAsync

    [Fact]
    public async Task ResetPasswordAsync_ExistingUser_ResetsPassword()
    {
        var user = CreateTestUser();
        user.PasswordHash = "old_hash";
        user.TokenVersion = 1;
        _userRepo.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(user);
        _crypto.Setup(c => c.HashPassword("NewP@ss123")).Returns("new_hash");

        await _sut.ResetPasswordAsync(1, new ResetPasswordRequest("NewP@ss123"), 0);

        user.PasswordHash.Should().Be("new_hash");
        user.TokenVersion.Should().Be(2);
        user.MustModifyPwd.Should().Be(0);
        user.SecurityStamp.Should().NotBeNullOrEmpty();
        _pwdHistoryRepo.Verify(r => r.AddAsync(It.Is<SysUserPasswordHistory>(h =>
            h.UserId == 1 && h.PasswordHash == "old_hash"), default), Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_NonExistingUser_ThrowsException()
    {
        _userRepo.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((SysUser?)null);

        var act = () => _sut.ResetPasswordAsync(999, new ResetPasswordRequest("x"), 0);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    #endregion

    #region ChangeMainOrganizationAsync

    [Fact]
    public async Task ChangeMainOrganizationAsync_ValidRequest_ChangesMainOrg()
    {
        _mainOrgRepo.Setup(r => r.ExistsByUserOrganizationIdAsync(100, default)).ReturnsAsync(true);

        await _sut.ChangeMainOrganizationAsync(1, new ChangeMainOrganizationRequest(100), 0);

        _mainOrgRepo.Verify(r => r.DeleteAsync(1, default), Times.Once);
        _mainOrgRepo.Verify(r => r.AddAsync(It.Is<SysUserMainOrganization>(m =>
            m.UserId == 1 && m.UserOrganizationId == 100), default), Times.Once);
    }

    [Fact]
    public async Task ChangeMainOrganizationAsync_InvalidUserOrgId_ThrowsException()
    {
        _mainOrgRepo.Setup(r => r.ExistsByUserOrganizationIdAsync(999, default)).ReturnsAsync(false);

        var act = () => _sut.ChangeMainOrganizationAsync(1, new ChangeMainOrganizationRequest(999), 0);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    #endregion
}
