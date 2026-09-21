namespace AuthCenter.Domain.Entities;

public class SysUserOrganization : SoftDeleteEntity
{
    public long UserId { get; set; }
    public long UnitId { get; set; }
    public long? DepartmentId { get; set; }
    public long? PositionId { get; set; }
    public long? RegionId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int Status { get; set; }
}
