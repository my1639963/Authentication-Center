namespace AuthCenter.Domain.Entities;

public class SysLoginLog : BaseEntity
{
    public long? UserId { get; set; }
    public string? LoginName { get; set; }
    public string? ClientId { get; set; }
    public string LoginType { get; set; } = string.Empty;
    public int LoginResult { get; set; }
    public string? FailReason { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestId { get; set; }
}
