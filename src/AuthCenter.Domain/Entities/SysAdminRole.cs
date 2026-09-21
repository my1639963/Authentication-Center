namespace AuthCenter.Domain.Entities;

public class SysAdminRole : SoftDeleteEntity
{
    public string RoleCode { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Status { get; set; }
}
