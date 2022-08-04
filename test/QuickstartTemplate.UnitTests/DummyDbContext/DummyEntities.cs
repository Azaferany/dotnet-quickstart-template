using System;
using QuickstartTemplate.ApplicationCore.Entities;

namespace QuickstartTemplate.UnitTests.DummyDbContext;

public class DummyEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class SoftDeletableDummyEntity : ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
}

public class TimeableDummyEntity : ITimeable
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? CreatedById { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string? ModifiedById { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }
}

public class TimeableAndSoftDeletableDummyEntity : ITimeable, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? CreatedById { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string? ModifiedById { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
}