using AuthCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthCenter.Infrastructure.Persistence.Configurations;

public class SysAdminRoleConfiguration : IEntityTypeConfiguration<SysAdminRole>
{
    public void Configure(EntityTypeBuilder<SysAdminRole> builder)
    {
        builder.ToTable("sys_admin_role");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateTime).HasColumnName("create_time").IsRequired();
        builder.Property(e => e.UpdateTime).HasColumnName("update_time").IsRequired();
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(0);

        builder.Property(e => e.RoleCode).HasColumnName("role_code").HasMaxLength(64).IsRequired();
        builder.Property(e => e.RoleName).HasColumnName("role_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(1000);
        builder.Property(e => e.Status).HasColumnName("status").IsRequired().HasDefaultValue(0);

        builder.HasIndex(e => e.RoleCode).IsUnique().HasDatabaseName("uk_sys_admin_role_code");
        builder.HasIndex(e => e.Status).HasDatabaseName("idx_sys_admin_role_status");
    }
}

public class SysAdminPermissionConfiguration : IEntityTypeConfiguration<SysAdminPermission>
{
    public void Configure(EntityTypeBuilder<SysAdminPermission> builder)
    {
        builder.ToTable("sys_admin_permission");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateTime).HasColumnName("create_time").IsRequired();
        builder.Property(e => e.UpdateTime).HasColumnName("update_time").IsRequired();

        builder.Property(e => e.PermissionCode).HasColumnName("permission_code").HasMaxLength(128).IsRequired();
        builder.Property(e => e.PermissionName).HasColumnName("permission_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(1000);

        builder.HasIndex(e => e.PermissionCode).IsUnique().HasDatabaseName("uk_sys_admin_permission_code");
    }
}

public class SysAdminUserRoleConfiguration : IEntityTypeConfiguration<SysAdminUserRole>
{
    public void Configure(EntityTypeBuilder<SysAdminUserRole> builder)
    {
        builder.ToTable("sys_admin_user_role");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateTime).HasColumnName("create_time").IsRequired();
        builder.Ignore(e => e.UpdateTime);

        builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(e => e.RoleId).HasColumnName("role_id").IsRequired();

        builder.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique().HasDatabaseName("uk_sys_admin_user_role");
        builder.HasIndex(e => e.UserId).HasDatabaseName("idx_sys_admin_user_role_user");
        builder.HasIndex(e => e.RoleId).HasDatabaseName("idx_sys_admin_user_role_role");
    }
}

public class SysAdminRolePermissionConfiguration : IEntityTypeConfiguration<SysAdminRolePermission>
{
    public void Configure(EntityTypeBuilder<SysAdminRolePermission> builder)
    {
        builder.ToTable("sys_admin_role_permission");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateTime).HasColumnName("create_time").IsRequired();
        builder.Ignore(e => e.UpdateTime);

        builder.Property(e => e.RoleId).HasColumnName("role_id").IsRequired();
        builder.Property(e => e.PermissionId).HasColumnName("permission_id").IsRequired();

        builder.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique().HasDatabaseName("uk_sys_admin_role_permission");
        builder.HasIndex(e => e.RoleId).HasDatabaseName("idx_sys_admin_role_permission_role");
        builder.HasIndex(e => e.PermissionId).HasDatabaseName("idx_sys_admin_role_permission_permission");
    }
}

public class SysLoginLogConfiguration : IEntityTypeConfiguration<SysLoginLog>
{
    public void Configure(EntityTypeBuilder<SysLoginLog> builder)
    {
        builder.ToTable("sys_login_log");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateTime).HasColumnName("create_time").IsRequired();
        builder.Ignore(e => e.UpdateTime);

        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.LoginName).HasColumnName("login_name").HasMaxLength(100);
        builder.Property(e => e.ClientId).HasColumnName("client_id").HasMaxLength(200);
        builder.Property(e => e.LoginType).HasColumnName("login_type").HasMaxLength(32).IsRequired();
        builder.Property(e => e.LoginResult).HasColumnName("login_result").IsRequired();
        builder.Property(e => e.FailReason).HasColumnName("fail_reason").HasMaxLength(1000);
        builder.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(64);
        builder.Property(e => e.UserAgent).HasColumnName("user_agent").HasMaxLength(1000);
        builder.Property(e => e.RequestId).HasColumnName("request_id").HasMaxLength(128);

        builder.HasIndex(e => e.UserId).HasDatabaseName("idx_sys_login_log_user");
        builder.HasIndex(e => e.LoginName).HasDatabaseName("idx_sys_login_log_login_name");
        builder.HasIndex(e => e.ClientId).HasDatabaseName("idx_sys_login_log_client");
        builder.HasIndex(e => e.LoginType).HasDatabaseName("idx_sys_login_log_type");
        builder.HasIndex(e => e.LoginResult).HasDatabaseName("idx_sys_login_log_result");
        builder.HasIndex(e => e.CreateTime).HasDatabaseName("idx_sys_login_log_time");
        builder.HasIndex(e => e.RequestId).HasDatabaseName("idx_sys_login_log_request");
        builder.HasIndex(e => new { e.UserId, e.CreateTime }).HasDatabaseName("idx_sys_login_log_user_time");
        builder.HasIndex(e => new { e.ClientId, e.CreateTime }).HasDatabaseName("idx_sys_login_log_client_time");
    }
}

public class SysAuditLogConfiguration : IEntityTypeConfiguration<SysAuditLog>
{
    public void Configure(EntityTypeBuilder<SysAuditLog> builder)
    {
        builder.ToTable("sys_audit_log");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateTime).HasColumnName("create_time").IsRequired();
        builder.Ignore(e => e.UpdateTime);

        builder.Property(e => e.EventType).HasColumnName("event_type").HasMaxLength(64).IsRequired();
        builder.Property(e => e.OperationType).HasColumnName("operation_type").HasMaxLength(64).IsRequired();
        builder.Property(e => e.OperateUserId).HasColumnName("operate_user_id");
        builder.Property(e => e.TargetUserId).HasColumnName("target_user_id");
        builder.Property(e => e.ClientId).HasColumnName("client_id").HasMaxLength(200);
        builder.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(64);
        builder.Property(e => e.UserAgent).HasColumnName("user_agent").HasMaxLength(1000);
        builder.Property(e => e.EventResult).HasColumnName("event_result").IsRequired();
        builder.Property(e => e.FailReason).HasColumnName("fail_reason").HasMaxLength(1000);
        builder.Property(e => e.Content).HasColumnName("content").HasColumnType("text");
        builder.Property(e => e.RequestId).HasColumnName("request_id").HasMaxLength(128);
        builder.Property(e => e.PreviousHash).HasColumnName("previous_hash").HasMaxLength(128);
        builder.Property(e => e.LogHash).HasColumnName("log_hash").HasMaxLength(128).IsRequired();

        builder.HasIndex(e => e.OperateUserId).HasDatabaseName("idx_sys_audit_operate_user");
        builder.HasIndex(e => e.TargetUserId).HasDatabaseName("idx_sys_audit_target_user");
        builder.HasIndex(e => e.ClientId).HasDatabaseName("idx_sys_audit_client");
        builder.HasIndex(e => e.EventType).HasDatabaseName("idx_sys_audit_event");
        builder.HasIndex(e => e.OperationType).HasDatabaseName("idx_sys_audit_operation");
        builder.HasIndex(e => e.EventResult).HasDatabaseName("idx_sys_audit_result");
        builder.HasIndex(e => e.CreateTime).HasDatabaseName("idx_sys_audit_time");
        builder.HasIndex(e => e.RequestId).HasDatabaseName("idx_sys_audit_request");
        builder.HasIndex(e => new { e.EventType, e.CreateTime }).HasDatabaseName("idx_sys_audit_event_time");
        builder.HasIndex(e => new { e.OperateUserId, e.CreateTime }).HasDatabaseName("idx_sys_audit_operate_user_time");
        builder.HasIndex(e => new { e.TargetUserId, e.CreateTime }).HasDatabaseName("idx_sys_audit_target_user_time");
    }
}
