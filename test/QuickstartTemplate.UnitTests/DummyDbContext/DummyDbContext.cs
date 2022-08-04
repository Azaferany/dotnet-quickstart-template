using Microsoft.EntityFrameworkCore;
using QuickstartTemplate.Infrastructure.DbContexts;

namespace QuickstartTemplate.UnitTests.DummyDbContext;

public class DummyDbContext : ProjectDbContext
{
    public DummyDbContext(DbContextOptions<DummyDbContext> options) : base(options)
    {
    }

    public DbSet<DummyEntity> DummyEntities { get; set; }
    public DbSet<TimeableDummyEntity> TimeableDummyEntities { get; set; }
    public DbSet<SoftDeletableDummyEntity> SoftDeletableDummyEntities { get; set; }
    public DbSet<TimeableAndSoftDeletableDummyEntity> TimeableAndSoftDeletableDummyEntities { get; set; }

}