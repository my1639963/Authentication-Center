namespace AuthCenter.Domain.Entities;

public class SysDepartment : SoftDeleteEntity
{
    public long UnitId { get; set; }
    public long? ParentId { get; set; }
    public string DeptCode { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public long? LeaderUserId { get; set; }
    public int Status { get; set; }
    public int Sort { get; set; }
    public string? Description { get; set; }
}
