using System.Security.Claims;
using AuthCenter.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using static OpenIddict.Server.OpenIddictServerEvents;

namespace AuthCenter.OpenIddict.Handlers;

public class CustomTokenClaimsHandler : IOpenIddictServerHandler<ProcessSignInContext>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public CustomTokenClaimsHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<ProcessSignInContext>()
            .UseScopedHandler<CustomTokenClaimsHandler>()
            .SetOrder(int.MaxValue - 100_000)
            .Build();

    public async ValueTask HandleAsync(ProcessSignInContext context)
    {
        var subject = context.Principal?.GetClaim(OpenIddictConstants.Claims.Subject);
        if (string.IsNullOrEmpty(subject))
            return;

        if (!long.TryParse(subject, out var userId))
            return;

        using var scope = _scopeFactory.CreateScope();
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var userOrgRepo = scope.ServiceProvider.GetRequiredService<IUserOrganizationRepository>();
        var mainOrgRepo = scope.ServiceProvider.GetRequiredService<IUserMainOrganizationRepository>();

        var user = await userRepo.GetByIdAsync(userId);
        if (user == null) return;

        context.AccessTokenPrincipal?.SetClaim("username", user.LoginName);
        context.AccessTokenPrincipal?.SetClaim("name", user.RealName);
        context.AccessTokenPrincipal?.SetClaim("token_version", user.TokenVersion.ToString());

        var mainOrg = await mainOrgRepo.GetByUserIdAsync(userId);
        if (mainOrg != null)
        {
            var userOrg = await userOrgRepo.GetByIdAsync(mainOrg.UserOrganizationId);
            if (userOrg != null)
            {
                context.AccessTokenPrincipal?.SetClaim("unit_id", userOrg.UnitId.ToString());
                if (userOrg.DepartmentId.HasValue)
                    context.AccessTokenPrincipal?.SetClaim("department_id", userOrg.DepartmentId.Value.ToString());
                if (userOrg.PositionId.HasValue)
                    context.AccessTokenPrincipal?.SetClaim("position_id", userOrg.PositionId.Value.ToString());
            }
        }
    }
}
