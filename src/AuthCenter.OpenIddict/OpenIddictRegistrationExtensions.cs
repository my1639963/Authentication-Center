using AuthCenter.OpenIddict.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuthCenter.OpenIddict;

public static class OpenIddictRegistrationExtensions
{
    public static IServiceCollection AddOpenIddictServices(this IServiceCollection services)
    {
        services.AddScoped<IOAuthClientService, OAuthClientService>();
        return services;
    }
}
