using AuthCenter.Application.Services;
using AuthCenter.Infrastructure;
using AuthCenter.OpenIddict;
using Scalar.AspNetCore;

namespace AuthCenter.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Missing connection string 'Default'.");

        builder.Services.AddInfrastructure(connectionString);
        builder.Services.AddOpenIddictServer();
        builder.Services.AddOpenIddictServices();

        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IRegionService, RegionService>();
        builder.Services.AddScoped<IUnitService, UnitService>();
        builder.Services.AddScoped<IDepartmentService, DepartmentService>();
        builder.Services.AddScoped<IPositionService, PositionService>();
        builder.Services.AddScoped<IAdminRoleService, AdminRoleService>();
        builder.Services.AddScoped<IAdminPermissionService, AdminPermissionService>();
        builder.Services.AddScoped<IUserAdminService, UserAdminService>();
        builder.Services.AddScoped<IAuditService, AuditService>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins("*")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        builder.Services.AddControllers();
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Title = "AuthCenter API";
                document.Info.Description = "认证中心 API 文档";
                document.Info.Version = "v1";
                return Task.CompletedTask;
            });
        });

        var validationScheme = "OpenIddict.Validation.AspNetCore";

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = validationScheme;
            options.DefaultAuthenticateScheme = validationScheme;
            options.DefaultChallengeScheme = validationScheme;
        });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithTitle("AuthCenter API");
                options.WithTheme(ScalarTheme.Default);
            });
        }

        app.UseCors("AllowFrontend");

        // 移除 401 响应中的 WWW-Authenticate 头，防止浏览器弹出原生登录窗口
        // 必须注册在 UseAuthentication 之前，这样响应阶段才能在 Auth 之后执行
        app.Use(async (context, next) =>
        {
            await next();
            if (context.Response.StatusCode == 401 && !context.Response.HasStarted)
            {
                context.Response.Headers.Remove("WWW-Authenticate");
            }
        });

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
