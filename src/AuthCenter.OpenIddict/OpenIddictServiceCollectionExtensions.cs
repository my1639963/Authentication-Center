using AuthCenter.Infrastructure.Persistence;
using AuthCenter.OpenIddict.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthCenter.OpenIddict;

public static class OpenIddictServiceCollectionExtensions
{
    public static IServiceCollection AddOpenIddictServer(this IServiceCollection services)
    {
        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<AuthCenterDbContext>();
            })
            .AddServer(options =>
            {
                options.SetAuthorizationEndpointUris("/oauth2/authorize")
                    .SetTokenEndpointUris("/oauth2/token")
                    .SetRevocationEndpointUris("/oauth2/revoke")
                    .SetIntrospectionEndpointUris("/oauth2/introspect")
                    .SetEndSessionEndpointUris("/oauth2/logout");

                options.AllowAuthorizationCodeFlow()
                    .AllowPasswordFlow()
                    .AllowClientCredentialsFlow()
                    .AllowRefreshTokenFlow();

                // 不要求 client_id，简化登录流程
                options.RemoveEventHandler(global::OpenIddict.Server.OpenIddictServerHandlers.ValidateClientId.Descriptor);

                // 开发环境使用临时证书，生产环境应使用真实证书
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                options.UseAspNetCore()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableTokenEndpointPassthrough()
                    .EnableEndSessionEndpointPassthrough()
                    .EnableStatusCodePagesIntegration()
                    .DisableTransportSecurityRequirement();

                options.SetAccessTokenLifetime(TimeSpan.FromMinutes(10));
                options.SetRefreshTokenLifetime(TimeSpan.FromDays(30));

                options.AddEventHandler(CustomTokenClaimsHandler.Descriptor);
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        return services;
    }
}
