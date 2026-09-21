namespace AuthCenter.Domain.Entities;

public class SysPosition : SoftDeleteEntity
{
    public string PositionCode { get; set; } = string.Empty;
    public string PositionName { get; set; } = string.Empty;
    public string? PositionType { get; set; }
    public int? PositionLevel { get; set; }
    public int Status { get; set; }
    public int Sort { get; set; }
    public string? Description { get; set; }
}
