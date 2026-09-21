using AuthCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenIddict.EntityFrameworkCore.Models;

namespace AuthCenter.Infrastructure.Persistence.Configurations;

public class OpenIddictEntityFrameworkCoreTokenConfiguration : IEntityTypeConfiguration<OpenIddictEntityFrameworkCoreToken>
{
    public void Configure(EntityTypeBuilder<OpenIddictEntityFrameworkCoreToken> builder)
    {

        builder.ToTable("oauth_token");
        builder.Property<string>("ApplicationId").HasMaxLength(128);
        builder.Property<string>("AuthorizationId").HasMaxLength(128);
        builder.Property(e => e.Subject).HasMaxLength(128);
        builder.Property(e => e.Status).HasMaxLength(128);
        builder.Property(e => e.Type).HasMaxLength(128);
    }
}

public class OpenIddictEntityFrameworkCoreApplicationConfiguration : IEntityTypeConfiguration<OpenIddictEntityFrameworkCoreApplication>
{
    public void Configure(EntityTypeBuilder<OpenIddictEntityFrameworkCoreApplication> builder)
    {

        builder.ToTable("oauth_application");
    }
}

public class OpenIddictEntityFrameworkCoreAuthorizationConfiguration : IEntityTypeConfiguration<OpenIddictEntityFrameworkCoreAuthorization>
{
    public void Configure(EntityTypeBuilder<OpenIddictEntityFrameworkCoreAuthorization> builder)
    {
        builder.ToTable("oauth_authorization");
    }
}

public class OpenIddictEntityFrameworkCoreScopeConfiguration : IEntityTypeConfiguration<OpenIddictEntityFrameworkCoreScope>
{
    public void Configure(EntityTypeBuilder<OpenIddictEntityFrameworkCoreScope> builder)
    {
        builder.ToTable("oauth_scope");
    }
}