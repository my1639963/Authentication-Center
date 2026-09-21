namespace AuthCenter.Domain.Entities;

public abstract class BaseEntity
{
    public long Id { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
}

public abstract class SoftDeleteEntity : BaseEntity
{
    public int IsDeleted { get; set; }
}
