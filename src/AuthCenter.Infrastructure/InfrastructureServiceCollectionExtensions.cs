using AuthCenter.Domain.Repositories;
using AuthCenter.Domain.Services;
using AuthCenter.Infrastructure.Persistence;
using AuthCenter.Infrastructure.Repositories;
using AuthCenter.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthCenter.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AuthCenterDbContext>(options =>
            options.UseMySQL(connectionString).UseOpenIddict());

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AuthCenterDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRegionRepository, RegionRepository>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<IUserOrganizationRepository, UserOrganizationRepository>();
        services.AddScoped<IUserMainOrganizationRepository, UserMainOrganizationRepository>();
        services.AddScoped<IPasswordHistoryRepository, PasswordHistoryRepository>();
        services.AddScoped<IAdminRoleRepository, AdminRoleRepository>();
        services.AddScoped<IAdminPermissionRepository, AdminPermissionRepository>();
        services.AddScoped<IAdminUserRoleRepository, AdminUserRoleRepository>();
        services.AddScoped<IAdminRolePermissionRepository, AdminRolePermissionRepository>();
        services.AddScoped<ILoginLogRepository, LoginLogRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        services.AddSingleton<ICryptoProvider, SoftwareCryptoProvider>();
        services.AddSingleton<IIdGenerator, SnowflakeIdGenerator>();

        return services;
    }
}
