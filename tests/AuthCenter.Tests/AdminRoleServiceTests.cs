using AuthCenter.Application.DTOs;
using AuthCenter.Application.Services;
using AuthCenter.Domain.Entities;
using AuthCenter.Domain.Repositories;
using AuthCenter.Domain.Services;
using FluentAssertions;
using Moq;

namespace AuthCenter.Tests;

public class AdminRoleServiceTests
{
    private readonly Mock<IAdminRoleRepository> _roleRepo;
    private readonly Mock<IAdminPermissionRepository> _permRepo;
    private readonly Mock<IAdminRolePermissionRepository> _rolePermRepo;
    private readonly Mock<IIdGenerator> _idGenerator;
    private readonly AdminRoleService _sut;

    public AdminRoleServiceTests()
    {
        _roleRepo = new Mock<IAdminRoleRepository>();
        _permRepo = new Mock<IAdminPermissionRepository>();
        _rolePermRepo = new Mock<IAdminRolePermissionRepository>();
        _idGenerator = new Mock<IIdGenerator>();
        _idGenerator.Setup(x => x.NewId()).Returns(() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

        _sut = new AdminRoleService(_roleRepo.Object, _permRepo.Object, _rolePermRepo.Object, _idGenerator.Object);
    }

    private static SysAdminRole CreateTestRole(long id = 1, string code = "admin", string name = "管理员") => new()
    {
        Id = id, RoleCode = code, RoleName = name, Status = 0,
        CreateTime = DateTime.Now, UpdateTime = DateTime.Now
    };

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnsAllRoles()
    {
        var roles = new List<SysAdminRole>
        {
            CreateTestRole(1, "admin", "管理员"),
            CreateTestRole(2, "user", "普通用户")
        };
        _roleRepo.Setup(r => r.GetAllAsync(default)).ReturnsAsync(roles);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
        result[0].RoleCode.Should().Be("admin");
        result[1].RoleCode.Should().Be("user");
    }

    [Fact]
    public async Task GetAllAsync_Empty_ReturnsEmptyList()
    {
        _roleRepo.Setup(r => r.GetAllAsync(default)).ReturnsAsync(new List<SysAdminRole>());

        var result = await _sut.GetAllAsync();

        result.Should().BeEmpty();
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ExistingRole_ReturnsDto()
    {
        var role = CreateTestRole();
        _roleRepo.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(role);

        var result = await _sut.GetByIdAsync(1);

        result.Id.Should().Be(1);
        result.RoleCode.Should().Be("admin");
        result.RoleName.Should().Be("管理员");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingRole_ThrowsException()
    {
        _roleRepo.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((SysAdminRole?)null);

        var act = () => _sut.GetByIdAsync(999);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_ValidRequest_CreatesRole()
    {
        var request = new CreateAdminRoleRequest("editor", "编辑", "负责内容编辑");

        var result = await _sut.CreateAsync(request);

        result.RoleCode.Should().Be("editor");
        result.RoleName.Should().Be("编辑");
        result.Description.Should().Be("负责内容编辑");
        result.Status.Should().Be(0);
        _roleRepo.Verify(r => r.AddAsync(It.Is<SysAdminRole>(e =>
            e.RoleCode == "editor" && e.RoleName == "编辑"), default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithoutDescription_CreatesRole()
    {
        var request = new CreateAdminRoleRequest("viewer", "查看者", null);

        var result = await _sut.CreateAsync(request);

        result.Description.Should().BeNull();
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_ExistingRole_UpdatesFields()
    {
        var role = CreateTestRole();
        _roleRepo.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(role);

        await _sut.UpdateAsync(1, new UpdateAdminRoleRequest("超级管理员", null, "新描述"));

        role.RoleName.Should().Be("超级管理员");
        role.Description.Should().Be("新描述");
        _roleRepo.Verify(r => r.UpdateAsync(role, default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate_OnlyUpdatesProvidedFields()
    {
        var role = CreateTestRole();
        role.Description = "old_desc";
        _roleRepo.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(role);

        await _sut.UpdateAsync(1, new UpdateAdminRoleRequest("新名称", null, null));

        role.RoleName.Should().Be("新名称");
        role.Description.Should().Be("old_desc");
    }

    [Fact]
    public async Task UpdateAsync_WithStatusChange_UpdatesStatus()
    {
        var role = CreateTestRole();
        _roleRepo.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(role);

        await _sut.UpdateAsync(1, new UpdateAdminRoleRequest(null, 1, null));

        role.Status.Should().Be(1);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingRole_ThrowsException()
    {
        _roleRepo.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((SysAdminRole?)null);

        var act = () => _sut.UpdateAsync(999, new UpdateAdminRoleRequest("x", null, null));

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    #endregion

    #region AssignPermissionsAsync

    [Fact]
    public async Task AssignPermissionsAsync_ClearsOldAndAddsNew()
    {
        var request = new AssignPermissionsRequest(new List<long> { 10, 20, 30 });

        await _sut.AssignPermissionsAsync(1, request);

        _rolePermRepo.Verify(r => r.DeleteByRoleIdAsync(1, default), Times.Once);
        _rolePermRepo.Verify(r => r.AddAsync(It.Is<SysAdminRolePermission>(rp =>
            rp.RoleId == 1 && rp.PermissionId == 10), default), Times.Once);
        _rolePermRepo.Verify(r => r.AddAsync(It.Is<SysAdminRolePermission>(rp =>
            rp.RoleId == 1 && rp.PermissionId == 20), default), Times.Once);
        _rolePermRepo.Verify(r => r.AddAsync(It.Is<SysAdminRolePermission>(rp =>
            rp.RoleId == 1 && rp.PermissionId == 30), default), Times.Once);
    }

    [Fact]
    public async Task AssignPermissionsAsync_EmptyList_OnlyClears()
    {
        var request = new AssignPermissionsRequest(new List<long>());

        await _sut.AssignPermissionsAsync(1, request);

        _rolePermRepo.Verify(r => r.DeleteByRoleIdAsync(1, default), Times.Once);
        _rolePermRepo.Verify(r => r.AddAsync(It.IsAny<SysAdminRolePermission>(), default), Times.Never);
    }

    #endregion

    #region GetRolePermissionsAsync

    [Fact]
    public async Task GetRolePermissionsAsync_ReturnsPermissions()
    {
        var rolePerms = new List<SysAdminRolePermission>
        {
            new() { Id = 1, RoleId = 1, PermissionId = 100 },
            new() { Id = 2, RoleId = 1, PermissionId = 200 }
        };
        var perm1 = new SysAdminPermission { Id = 100, PermissionCode = "user.read", PermissionName = "查看用户" };
        var perm2 = new SysAdminPermission { Id = 200, PermissionCode = "user.write", PermissionName = "编辑用户" };

        _rolePermRepo.Setup(r => r.GetByRoleIdAsync(1, default)).ReturnsAsync(rolePerms);
        _permRepo.Setup(r => r.GetByIdAsync(100, default)).ReturnsAsync(perm1);
        _permRepo.Setup(r => r.GetByIdAsync(200, default)).ReturnsAsync(perm2);

        var result = await _sut.GetRolePermissionsAsync(1);

        result.Should().HaveCount(2);
        result[0].PermissionCode.Should().Be("user.read");
        result[1].PermissionCode.Should().Be("user.write");
    }

    [Fact]
    public async Task GetRolePermissionsAsync_NoPermissions_ReturnsEmpty()
    {
        _rolePermRepo.Setup(r => r.GetByRoleIdAsync(1, default)).ReturnsAsync(new List<SysAdminRolePermission>());

        var result = await _sut.GetRolePermissionsAsync(1);

        result.Should().BeEmpty();
    }

    #endregion
}
