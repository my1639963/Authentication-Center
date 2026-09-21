namespace AuthCenter.Domain.Entities;

public class SysAdminUserRole : BaseEntity
{
    public long UserId { get; set; }
    public long RoleId { get; set; }
}
