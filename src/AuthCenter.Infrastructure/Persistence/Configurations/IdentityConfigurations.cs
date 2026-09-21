using AuthCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthCenter.Infrastructure.Persistence.Configurations;

public class SysUserConfiguration : IEntityTypeConfiguration<SysUser>
{
    public void Configure(EntityTypeBuilder<SysUser> builder)
    {
        builder.ToTable("sys_user");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateTime).HasColumnName("create_time").IsRequired();
        builder.Property(e => e.UpdateTime).HasColumnName("update_time").IsRequired();
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(0);

        builder.Property(e => e.LoginName).HasColumnName("login_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(512).IsRequired();
        builder.Property(e => e.RealName).HasColumnName("real_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Mobile).HasColumnName("mobile").HasMaxLength(32);
        builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(200);
        builder.Property(e => e.IdCardCiphertext).HasColumnName("id_card_ciphertext").HasMaxLength(1024);
        builder.Property(e => e.IdCardHash).HasColumnName("id_card_hash").HasMaxLength(128);
        builder.Property(e => e.Status).HasColumnName("status").IsRequired().HasDefaultValue(0);
        builder.Property(e => e.LoginFailCount).HasColumnName("login_fail_count").IsRequired().HasDefaultValue(0);
        builder.Property(e => e.LockUntil).HasColumnName("lock_until");
        builder.Property(e => e.MustModifyPwd).HasColumnName("must_modify_pwd").IsRequired().HasDefaultValue(0);
        builder.Property(e => e.PasswordExpireTime).HasColumnName("password_expire_time");
        builder.Property(e => e.SecurityStamp).HasColumnName("security_stamp");
        builder.Property(e => e.TokenVersion).HasColumnName("token_version").IsRequired().HasDefaultValue(1);
        builder.Property(e => e.LastLoginTime).HasColumnName("last_login_time");
        builder.Property(e => e.LastLoginIp).HasColumnName("last_login_ip");

        builder.HasIndex(e => e.LoginName).IsUnique().HasDatabaseName("uk_sys_user_login_name");
        builder.HasIndex(e => e.RealName).HasDatabaseName("idx_sys_user_real_name");
        builder.HasIndex(e => e.Mobile).HasDatabaseName("idx_sys_user_mobile");
        builder.HasIndex(e => e.IdCardHash).HasDatabaseName("idx_sys_user_id_card_hash");
        builder.HasIndex(e => e.Status).HasDatabaseName("idx_sys_user_status");
        builder.HasIndex(e => e.CreateTime).HasDatabaseName("idx_sys_user_create_time");
    }
}

public class SysRegionConfiguration : IEntityTypeConfiguration<SysRegion>
{
    public void Configure(EntityTypeBuilder<SysRegion> builder)
    {
        builder.ToTable("sys_region");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateTime).HasColumnName("create_time").IsRequired();
        builder.Property(e => e.UpdateTime).HasColumnName("update_time").IsRequired();
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(0);

        builder.Property(e => e.ParentId).HasColumnName("parent_id");
        builder.Property(e => e.RegionCode).HasColumnName("region_code").HasMaxLength(32).IsRequired();
        builder.Property(e => e.RegionName).HasColumnName("region_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.RegionLevel).HasColumnName("region_level").IsRequired();
        builder.Property(e => e.Status).HasColumnName("status").IsRequired().HasDefaultValue(0);
        builder.Property(e => e.Sort).HasColumnName("sort").IsRequired().HasDefaultValue(0);

        builder.HasIndex(e => e.RegionCode).IsUnique().HasDatabaseName("uk_sys_region_code");
        builder.HasIndex(e => e.ParentId).HasDatabaseName("idx_sys_region_parent");
        builder.HasIndex(e => e.RegionLevel).HasDatabaseName("idx_sys_region_level");
        builder.HasIndex(e => e.Status).HasDatabaseName("idx_sys_region_status");
    }
}

public class SysUnitConfiguration : IEntityTypeConfiguration<SysUnit>
{
    public void Configure(EntityTypeBuilder<SysUnit> builder)
    {
        builder.ToTable("sys_unit");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateTime).HasColumnName("create_time").IsRequired();
        builder.Property(e => e.UpdateTime).HasColumnName("update_time").IsRequired();
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(0);

        builder.Property(e => e.ParentId).HasColumnName("parent_id");
        builder.Property(e => e.UnitCode).HasColumnName("unit_code").HasMaxLength(64).IsRequired();
        builder.Property(e => e.UnitName).HasColumnName("unit_name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.UnitType).HasColumnName("unit_type").HasMaxLength(32);
        builder.Property(e => e.UnitLevel).HasColumnName("unit_level");
        builder.Property(e => e.RegionId).HasColumnName("region_id");
        builder.Property(e => e.Status).HasColumnName("status").IsRequired().HasDefaultValue(0);
        builder.Property(e => e.Sort).HasColumnName("sort").IsRequired().HasDefaultValue(0);
        builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(1000);

        builder.HasIndex(e => e.UnitCode).IsUnique().HasDatabaseName("uk_sys_unit_code");
        builder.HasIndex(e => e.ParentId).HasDatabaseName("idx_sys_unit_parent");
        builder.HasIndex(e => e.RegionId).HasDatabaseName("idx_sys_unit_region");
        builder.HasIndex(e => e.Status).HasDatabaseName("idx_sys_unit_status");
        builder.HasIndex(e => e.UnitName).HasDatabaseName("idx_sys_unit_name");
    }
}

public class SysDepartmentConfiguration : IEntityTypeConfiguration<SysDepartment>
{
    public void Configure(EntityTypeBuilder<SysDepartment> builder)
    {
        builder.ToTable("sys_department");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateTime).HasColumnName("create_time").IsRequired();
        builder.Property(e => e.UpdateTime).HasColumnName("update_time").IsRequired();
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(0);

        builder.Property(e => e.UnitId).HasColumnName("unit_id").IsRequired();
        builder.Property(e => e.ParentId).HasColumnName("parent_id");
        builder.Property(e => e.DeptCode).HasColumnName("dept_code").HasMaxLength(64).IsRequired();
        builder.Property(e => e.DeptName).HasColumnName("dept_name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.LeaderUserId).HasColumnName("leader_user_id");
        builder.Property(e => e.Status).HasColumnName("status").IsRequired().HasDefaultValue(0);
        builder.Property(e => e.Sort).HasColumnName("sort").IsRequired().HasDefaultValue(0);
        builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(1000);

        builder.HasIndex(e => new { e.UnitId, e.DeptCode }).IsUnique().HasDatabaseName("uk_sys_department_unit_code");
        builder.HasIndex(e => e.UnitId).HasDatabaseName("idx_sys_department_unit");
        builder.HasIndex(e => e.ParentId).HasDatabaseName("idx_sys_department_parent");
        builder.HasIndex(e => e.LeaderUserId).HasDatabaseName("idx_sys_department_leader");
        builder.HasIndex(e => e.Status).HasDatabaseName("idx_sys_department_status");
    }
}

public class SysPositionConfiguration : IEntityTypeConfiguration<SysPosition>
{
    public void Configure(EntityTypeBuilder<SysPosition> builder)
    {
        builder.ToTable("sys_position");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateTime).HasColumnName("create_time").IsRequired();
        builder.Property(e => e.UpdateTime).HasColumnName("update_time").IsRequired();
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(0);

        builder.Property(e => e.PositionCode).HasColumnName("position_code").HasMaxLength(64).IsRequired();
        builder.Property(e => e.PositionName).HasColumnName("position_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.PositionType).HasColumnName("position_type").HasMaxLength(32);
        builder.Property(e => e.PositionLevel).HasColumnName("position_level");
        builder.Property(e => e.Status).HasColumnName("status").IsRequired().HasDefaultValue(0);
        builder.Property(e => e.Sort).HasColumnName("sort").IsRequired().HasDefaultValue(0);
        builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(1000);

        builder.HasIndex(e => e.PositionCode).IsUnique().HasDatabaseName("uk_sys_position_code");
        builder.HasIndex(e => e.Status).HasDatabaseName("idx_sys_position_status");
        builder.HasIndex(e => e.PositionType).HasDatabaseName("idx_sys_position_type");
    }
}

public class SysUserOrganizationConfiguration : IEntityTypeConfiguration<SysUserOrganization>
{
    public void Configure(EntityTypeBuilder<SysUserOrganization> builder)
    {
        builder.ToTable("sys_user_organization");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateTime).HasColumnName("create_time").IsRequired();
        builder.Property(e => e.UpdateTime).HasColumnName("update_time").IsRequired();
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(0);

        builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(e => e.UnitId).HasColumnName("unit_id").IsRequired();
        builder.Property(e => e.DepartmentId).HasColumnName("department_id");
        builder.Property(e => e.PositionId).HasColumnName("position_id");
        builder.Property(e => e.RegionId).HasColumnName("region_id");
        builder.Property(e => e.StartTime).HasColumnName("start_time");
        builder.Property(e => e.EndTime).HasColumnName("end_time");
        builder.Property(e => e.Status).HasColumnName("status").IsRequired().HasDefaultValue(0);

        builder.HasIndex(e => e.UserId).HasDatabaseName("idx_sys_user_org_user");
        builder.HasIndex(e => e.UnitId).HasDatabaseName("idx_sys_user_org_unit");
        builder.HasIndex(e => e.DepartmentId).HasDatabaseName("idx_sys_user_org_department");
        builder.HasIndex(e => e.PositionId).HasDatabaseName("idx_sys_user_org_position");
        builder.HasIndex(e => e.RegionId).HasDatabaseName("idx_sys_user_org_region");
        builder.HasIndex(e => e.Status).HasDatabaseName("idx_sys_user_org_status");
    }
}

public class SysUserMainOrganizationConfiguration : IEntityTypeConfiguration<SysUserMainOrganization>
{
    public void Configure(EntityTypeBuilder<SysUserMainOrganization> builder)
    {
        builder.ToTable("sys_user_main_organization");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateTime).HasColumnName("create_time").IsRequired();
        builder.Property(e => e.UpdateTime).HasColumnName("update_time").IsRequired();

        builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(e => e.UserOrganizationId).HasColumnName("user_organization_id").IsRequired();

        builder.HasIndex(e => e.UserId).IsUnique().HasDatabaseName("uk_sys_user_main_org_user");
        builder.HasIndex(e => e.UserOrganizationId).IsUnique().HasDatabaseName("uk_sys_user_main_org_relation");
    }
}

public class SysUserPasswordHistoryConfiguration : IEntityTypeConfiguration<SysUserPasswordHistory>
{
    public void Configure(EntityTypeBuilder<SysUserPasswordHistory> builder)
    {
        builder.ToTable("sys_user_password_history");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreateTime).HasColumnName("create_time").IsRequired();
        builder.Ignore(e => e.UpdateTime);

        builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(512).IsRequired();

        builder.HasIndex(e => new { e.UserId, e.CreateTime }).HasDatabaseName("idx_sys_pwd_history_user_time");
    }
}
