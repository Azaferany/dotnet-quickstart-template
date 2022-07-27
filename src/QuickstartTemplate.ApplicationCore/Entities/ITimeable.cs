namespace QuickstartTemplate.ApplicationCore.Entities;

public interface ITimeable
{
    public int CreatedById { get; set; }
    public DateTimeOffset CreatedOn { get; set; }

    public int ModifiedById { get; set; }
    public DateTimeOffset ModifiedOn { get; set; }

}