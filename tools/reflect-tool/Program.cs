// reflect-tool: 反射工具，用于检查实体类型和 EF Core 配置
// 用法: dotnet run

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using AuthCenter.Infrastructure.Persistence;
using AuthCenter.Domain.Entities;

Console.WriteLine("=== AuthCenter 实体反射工具 ===\n");

// 列出所有业务实体及其属性
var entityTypes = new[]
{
    typeof(SysUser),
    typeof(SysUnit),
    typeof(SysDepartment),
    typeof(SysPosition),
    typeof(SysRegion),
    typeof(SysUserOrganization),
    typeof(SysUserMainOrganization),
    typeof(SysUserPasswordHistory),
    typeof(SysAdminRole),
    typeof(SysAdminPermission),
    typeof(SysAdminRolePermission),
    typeof(SysAdminUserRole),
    typeof(SysLoginLog),
    typeof(SysAuditLog),
};

foreach (var type in entityTypes)
{
    Console.WriteLine($"--- {type.Name} ---");
    var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
    foreach (var prop in props)
    {
        var propType = prop.PropertyType;
        var typeName = propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>)
            ? $"{Nullable.GetUnderlyingType(propType)?.Name}?"
            : propType.Name;
        Console.WriteLine($"  {prop.Name,-25} {typeName}");
    }
    Console.WriteLine();
}

// 检查 OpenIddict 实体
Console.WriteLine("--- OpenIddict 实体检查 ---");
var openIddictAssembly = AppDomain.CurrentDomain.GetAssemblies()
    .FirstOrDefault(a => a.GetName().Name?.Contains("OpenIddict") == true);

if (openIddictAssembly != null)
{
    Console.WriteLine($"已加载: {openIddictAssembly.FullName}");
}
else
{
    Console.WriteLine("OpenIddict 程序集未加载（需启动 Web 项目后才会加载）");
}

Console.WriteLine("\n完成。");
