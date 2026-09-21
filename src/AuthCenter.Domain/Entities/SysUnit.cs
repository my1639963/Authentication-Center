namespace AuthCenter.Domain.Entities;

public class SysUnit : SoftDeleteEntity
{
    public long? ParentId { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public string? UnitType { get; set; }
    public int? UnitLevel { get; set; }
    public long? RegionId { get; set; }
    public int Status { get; set; }
    public int Sort { get; set; }
    public string? Description { get; set; }
}
