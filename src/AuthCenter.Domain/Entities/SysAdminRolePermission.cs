namespace AuthCenter.Domain.Entities;

public class SysAdminRolePermission : BaseEntity
{
    public long RoleId { get; set; }
    public long PermissionId { get; set; }
}
