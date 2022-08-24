namespace QuickstartTemplate.ApplicationCore.Entities;

public interface ITimeable
{
    public string? CreatedById { get; set; }
    public DateTimeOffset CreatedOn { get; set; }

    public string? ModifiedById { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }

}
