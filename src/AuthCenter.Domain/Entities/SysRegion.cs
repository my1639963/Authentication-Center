namespace AuthCenter.Domain.Entities;

public class SysRegion : SoftDeleteEntity
{
    public long? ParentId { get; set; }
    public string RegionCode { get; set; } = string.Empty;
    public string RegionName { get; set; } = string.Empty;
    public int RegionLevel { get; set; }
    public int Status { get; set; }
    public int Sort { get; set; }
}
