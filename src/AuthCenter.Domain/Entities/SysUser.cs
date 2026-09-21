namespace AuthCenter.Domain.Entities;

public class SysUser : SoftDeleteEntity
{
    public string LoginName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? IdCardCiphertext { get; set; }
    public string? IdCardHash { get; set; }
    public int Status { get; set; }
    public int LoginFailCount { get; set; }
    public DateTime? LockUntil { get; set; }
    public int MustModifyPwd { get; set; }
    public DateTime? PasswordExpireTime { get; set; }
    public string? SecurityStamp { get; set; }
    public long TokenVersion { get; set; } = 1;
    public DateTime? LastLoginTime { get; set; }
    public string? LastLoginIp { get; set; }
}
