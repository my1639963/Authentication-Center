using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace AuthCenter.OpenIddict.Services;

public interface IOAuthClientService
{
    Task<OAuthClientDto?> GetByClientIdAsync(string clientId, CancellationToken ct = default);
    Task<IReadOnlyList<OAuthClientDto>> GetAllAsync(CancellationToken ct = default);
    Task<OAuthClientDto> CreateAsync(CreateOAuthClientRequest request, CancellationToken ct = default);
    Task UpdateAsync(string clientId, UpdateOAuthClientRequest request, CancellationToken ct = default);
    Task DeleteAsync(string clientId, CancellationToken ct = default);
}

public record OAuthClientDto(
    string ClientId,
    string? DisplayName,
    string? ClientType,
    IReadOnlyList<string> RedirectUris,
    IReadOnlyList<string> Permissions
);

public record CreateOAuthClientRequest(
    string ClientId,
    string? ClientSecret,
    string? DisplayName,
    string? ClientType,
    IReadOnlyList<string>? RedirectUris,
    IReadOnlyList<string>? Permissions
);

public record UpdateOAuthClientRequest(
    string? DisplayName,
    IReadOnlyList<string>? RedirectUris,
    IReadOnlyList<string>? Permissions
);

public class OAuthClientService : IOAuthClientService
{
    private readonly IOpenIddictApplicationManager _applicationManager;

    public OAuthClientService(IOpenIddictApplicationManager applicationManager)
    {
        _applicationManager = applicationManager;
    }

    public async Task<OAuthClientDto?> GetByClientIdAsync(string clientId, CancellationToken ct = default)
    {
        var app = await _applicationManager.FindByClientIdAsync(clientId, ct);
        if (app == null) return null;
        return await MapToDtoAsync(app, ct);
    }

    public async Task<IReadOnlyList<OAuthClientDto>> GetAllAsync(CancellationToken ct = default)
    {
        var result = new List<OAuthClientDto>();
        var apps = _applicationManager.ListAsync(count: null, offset: null, ct);
        await foreach (var app in apps)
        {
            var dto = await MapToDtoAsync(app, ct);
            if (dto != null) result.Add(dto);
        }
        return result;
    }

    public async Task<OAuthClientDto> CreateAsync(CreateOAuthClientRequest request, CancellationToken ct = default)
    {
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = request.ClientId,
            ClientSecret = request.ClientSecret,
            DisplayName = request.DisplayName,
        };

        if (request.ClientType != null)
            descriptor.ClientType = request.ClientType;

        if (request.RedirectUris != null)
            foreach (var uri in request.RedirectUris)
                descriptor.RedirectUris.Add(new Uri(uri));

        if (request.Permissions != null)
            foreach (var perm in request.Permissions)
                descriptor.Permissions.Add(perm);

        await _applicationManager.CreateAsync(descriptor, ct);

        var app = await _applicationManager.FindByClientIdAsync(request.ClientId, ct);
        return (await MapToDtoAsync(app!, ct))!;
    }

    public async Task UpdateAsync(string clientId, UpdateOAuthClientRequest request, CancellationToken ct = default)
    {
        var app = await _applicationManager.FindByClientIdAsync(clientId, ct)
            ?? throw new InvalidOperationException($"Client '{clientId}' not found.");

        var descriptor = new OpenIddictApplicationDescriptor();
        await _applicationManager.PopulateAsync(descriptor, app, ct);

        if (request.DisplayName != null)
            descriptor.DisplayName = request.DisplayName;

        if (request.RedirectUris != null)
        {
            descriptor.RedirectUris.Clear();
            foreach (var uri in request.RedirectUris)
                descriptor.RedirectUris.Add(new Uri(uri));
        }

        if (request.Permissions != null)
        {
            descriptor.Permissions.Clear();
            foreach (var perm in request.Permissions)
                descriptor.Permissions.Add(perm);
        }

        await _applicationManager.UpdateAsync(app, descriptor, ct);
    }

    public async Task DeleteAsync(string clientId, CancellationToken ct = default)
    {
        var app = await _applicationManager.FindByClientIdAsync(clientId, ct)
            ?? throw new InvalidOperationException($"Client '{clientId}' not found.");
        await _applicationManager.DeleteAsync(app, ct);
    }

    private async Task<OAuthClientDto?> MapToDtoAsync(object app, CancellationToken ct)
    {
        var descriptor = new OpenIddictApplicationDescriptor();
        await _applicationManager.PopulateAsync(descriptor, app, ct);

        return new OAuthClientDto(
            ClientId: descriptor.ClientId ?? string.Empty,
            DisplayName: descriptor.DisplayName,
            ClientType: descriptor.ClientType,
            RedirectUris: descriptor.RedirectUris.Select(u => u.ToString()).ToList(),
            Permissions: descriptor.Permissions.ToList()
        );
    }
}
