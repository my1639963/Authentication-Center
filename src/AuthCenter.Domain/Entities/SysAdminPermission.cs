namespace AuthCenter.Domain.Entities;

public class SysAdminPermission : BaseEntity
{
    public string PermissionCode { get; set; } = string.Empty;
    public string PermissionName { get; set; } = string.Empty;
    public string? Description { get; set; }
}
