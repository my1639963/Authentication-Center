namespace AuthCenter.Domain.Entities;

public class SysUserMainOrganization : BaseEntity
{
    public long UserId { get; set; }
    public long UserOrganizationId { get; set; }
}
