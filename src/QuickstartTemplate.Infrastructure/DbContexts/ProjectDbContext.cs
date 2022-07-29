using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using QuickstartTemplate.ApplicationCore.Common;
using QuickstartTemplate.ApplicationCore.Contracts;
using QuickstartTemplate.ApplicationCore.Entities;

namespace QuickstartTemplate.Infrastructure.DbContexts;

public class ProjectDbContext : DbContext, IProjectDbContext
{
    public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // https://stackoverflow.com/questions/63063207/ef-core-setqueryfilter-reverse-isactive-to-isdeleted-in-onmodelcreating
        foreach (var entityType in modelBuilder.Model.FindEntityTypes(typeof(ISoftDeletable)))
        {
            Expression<Func<ISoftDeletable, bool>> filterExpression = entity => !entity.IsDeleted;

            entityType.SetQueryFilter(Expression.Lambda(filterExpression.Body, filterExpression.Parameters));
        }
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();

        SetEntitiesContracts(cancellationToken);

        ChangeTracker.AutoDetectChangesEnabled = false;

        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        ChangeTracker.AutoDetectChangesEnabled = true;

        return result;
    }

    private void SetEntitiesContracts(CancellationToken cancellationToken)
    {
        var httpContext = this.GetService<IHttpContextAccessor>().HttpContext;
        var dateTimeProvider = this.GetService<IDateTimeProvider>();

        foreach (var entry in ChangeTracker.Entries())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (entry.Entity is ISoftDeletable)
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.CurrentValues[nameof(ISoftDeletable.IsDeleted)] = true;
                    entry.CurrentValues[nameof(ISoftDeletable.DeletedOn)] = dateTimeProvider.GetNow();
                }
            }

            if (entry.Entity is ITimeable)
            {
                if (entry.State == EntityState.Added)
                {
                    if (httpContext?.User?.Identity?.IsAuthenticated == true)
                    {
                        entry.CurrentValues[nameof(ITimeable.CreatedById)] = httpContext.User.UserId();
                    }

                    entry.CurrentValues[nameof(ITimeable.CreatedOn)] = dateTimeProvider.GetNow();
                }
                else if (httpContext != null && httpContext.User != null && entry.State == EntityState.Modified)
                {
                    if (httpContext?.User?.Identity?.IsAuthenticated == true)
                    {
                        entry.CurrentValues[nameof(ITimeable.ModifiedById)] = httpContext.User.UserId();
                    }

                    entry.CurrentValues[nameof(ITimeable.ModifiedOn)] = dateTimeProvider.GetNow();
                }
            }
        }
    }
}