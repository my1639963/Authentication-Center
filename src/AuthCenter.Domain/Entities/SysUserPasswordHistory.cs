namespace AuthCenter.Domain.Entities;

public class SysUserPasswordHistory : BaseEntity
{
    public long UserId { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
}
