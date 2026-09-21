using AuthCenter.Domain.Entities;
using AuthCenter.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace AuthCenter.Infrastructure.Persistence;

public class AuthCenterDbContext : DbContext, IUnitOfWork
{
    public DbSet<SysUser> Users => Set<SysUser>();
    public DbSet<SysRegion> Regions => Set<SysRegion>();
    public DbSet<SysUnit> Units => Set<SysUnit>();
    public DbSet<SysDepartment> Departments => Set<SysDepartment>();
    public DbSet<SysPosition> Positions => Set<SysPosition>();
    public DbSet<SysUserOrganization> UserOrganizations => Set<SysUserOrganization>();
    public DbSet<SysUserMainOrganization> UserMainOrganizations => Set<SysUserMainOrganization>();
    public DbSet<SysUserPasswordHistory> PasswordHistories => Set<SysUserPasswordHistory>();
    public DbSet<SysAdminRole> AdminRoles => Set<SysAdminRole>();
    public DbSet<SysAdminPermission> AdminPermissions => Set<SysAdminPermission>();
    public DbSet<SysAdminUserRole> AdminUserRoles => Set<SysAdminUserRole>();
    public DbSet<SysAdminRolePermission> AdminRolePermissions => Set<SysAdminRolePermission>();
    public DbSet<SysLoginLog> LoginLogs => Set<SysLoginLog>();
    public DbSet<SysAuditLog> AuditLogs => Set<SysAuditLog>();

    public AuthCenterDbContext(DbContextOptions<AuthCenterDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthCenterDbContext).Assembly);
    }
}
