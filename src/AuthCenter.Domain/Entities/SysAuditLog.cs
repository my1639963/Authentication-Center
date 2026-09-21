namespace AuthCenter.Domain.Entities;

public class SysAuditLog : BaseEntity
{
    public string EventType { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty;
    public long? OperateUserId { get; set; }
    public long? TargetUserId { get; set; }
    public string? ClientId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public int EventResult { get; set; }
    public string? FailReason { get; set; }
    public string? Content { get; set; }
    public string? RequestId { get; set; }
    public string? PreviousHash { get; set; }
    public string LogHash { get; set; } = string.Empty;
}
