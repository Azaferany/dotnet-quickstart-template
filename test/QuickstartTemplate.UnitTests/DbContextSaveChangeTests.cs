using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using NSubstitute;
using QuickstartTemplate.UnitTests.DummyDbContext;
using Xunit;

namespace QuickstartTemplate.UnitTests;

public class DbContextSaveChangeTests
{
    [Fact]
    public async Task Simple_insert_test()
    {
        //Arrange
        var services = new ServiceCollection();
        var now = DateTime.Now;

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(_ => new DefaultHttpContext());

        var systemClock = Substitute.For<ISystemClock>();
        systemClock.UtcNow.Returns(_ => now);

        services.AddSingleton(provider => httpContextAccessor);
        services.AddSingleton(provider => systemClock);

        services.AddEntityFrameworkInMemoryDatabase();

        services.AddDbContextPool<DummyDbContext.DummyDbContext>((provider, builder) =>
        {
            builder.UseInMemoryDatabase("InMemoryDbForTesting");
            builder.UseInternalServiceProvider(provider);
        });

        await using var serviceProviderScope = services.BuildServiceProvider().CreateAsyncScope();
        var serviceProvider = serviceProviderScope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<DummyDbContext.DummyDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        //Act
        var dummyEntity = new DummyEntity
        {
            Name = "test name"
        };

        dbContext.Add(dummyEntity);

        await dbContext.SaveChangesAsync();

        var res = await dbContext.DummyEntities.FirstOrDefaultAsync(x => x.Id == dummyEntity.Id);

        //Assert
        res?.Name.Should().BeEquivalentTo(dummyEntity.Name);
    }

    [Fact]
    public async Task Insert_TimeableDummyEntity_and_test_timeable_props_will_fill()
    {
        //Arrange
        var services = new ServiceCollection();
        var now = DateTime.Now;
        var userId = "64624";

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(_ => new DefaultHttpContext()
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", userId) }, "test type", "sub", null))
        });

        var systemClock = Substitute.For<ISystemClock>();
        systemClock.UtcNow.Returns(_ => now);

        services.AddSingleton(provider => httpContextAccessor);
        services.AddSingleton(provider => systemClock);

        services.AddEntityFrameworkInMemoryDatabase();

        services.AddDbContextPool<DummyDbContext.DummyDbContext>((provider, builder) =>
        {
            builder.EnableSensitiveDataLogging();
            builder.UseInMemoryDatabase("InMemoryDbForTesting");
            builder.UseInternalServiceProvider(provider);
        });
        using var serviceProvider = services.BuildServiceProvider();
        using var serviceProviderScope = services.BuildServiceProvider().CreateScope();
        var scopeServiceProvider = serviceProviderScope.ServiceProvider;
        var dbContext = scopeServiceProvider.GetRequiredService<DummyDbContext.DummyDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        //Act
        var dummyEntity = new TimeableDummyEntity
        {
            Name = "test name"
        };

        dbContext.Add(dummyEntity);

        await dbContext.SaveChangesAsync();

        var res = await dbContext.TimeableDummyEntities.FirstOrDefaultAsync(x => x.Id == dummyEntity.Id);

        //Assert
        res?.Name.Should().BeEquivalentTo(dummyEntity.Name);
        res?.CreatedOn.Should().BeExactly(now);
        res?.ModifiedOn.Should().BeNull();
        res?.CreatedById.Should().BeEquivalentTo(userId);
        res?.ModifiedById.Should().BeNull();
    }

    [Fact]
    public async Task Insert_SoftDeletableDummyEntity_and_test_softdeletable_props_will_fill()
    {
        //Arrange
        var services = new ServiceCollection();
        var now = DateTime.Now;
        var userId = "64624";

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(_ => new DefaultHttpContext()
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", userId) }, "test type", "sub", null))
        });

        var systemClock = Substitute.For<ISystemClock>();
        systemClock.UtcNow.Returns(_ => now);

        services.AddSingleton(provider => httpContextAccessor);
        services.AddSingleton(provider => systemClock);

        services.AddEntityFrameworkInMemoryDatabase();

        services.AddDbContextPool<DummyDbContext.DummyDbContext>((provider, builder) =>
        {
            builder.EnableSensitiveDataLogging();
            builder.UseInMemoryDatabase("InMemoryDbForTesting");
            builder.UseInternalServiceProvider(provider);
        });
        using var serviceProvider = services.BuildServiceProvider();
        using var serviceProviderScope = services.BuildServiceProvider().CreateScope();
        var scopeServiceProvider = serviceProviderScope.ServiceProvider;
        var dbContext = scopeServiceProvider.GetRequiredService<DummyDbContext.DummyDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        //Act
        var dummyEntity = new SoftDeletableDummyEntity
        {
            Name = "test name"
        };

        dbContext.Add(dummyEntity);

        await dbContext.SaveChangesAsync();

        var res = await dbContext.SoftDeletableDummyEntities.FirstOrDefaultAsync(x => x.Id == dummyEntity.Id);

        //Assert
        res?.Name.Should().BeEquivalentTo(dummyEntity.Name);
        res?.IsDeleted.Should().BeFalse();
        res?.DeletedOn.Should().BeNull();

    }

    [Fact]
    public async Task Insert_TimeableAndSoftDeletableDummyEntity_and_test_softdeletable_and_timeable_props_will_fill()
    {
        //Arrange
        var services = new ServiceCollection();
        var now = DateTime.Now;
        var userId = "64624";

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(_ => new DefaultHttpContext()
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", userId) }, "test type", "sub", null))
        });

        var systemClock = Substitute.For<ISystemClock>();
        systemClock.UtcNow.Returns(_ => now);

        services.AddSingleton(provider => httpContextAccessor);
        services.AddSingleton(provider => systemClock);

        services.AddEntityFrameworkInMemoryDatabase();

        services.AddDbContextPool<DummyDbContext.DummyDbContext>((provider, builder) =>
        {
            builder.EnableSensitiveDataLogging();
            builder.UseInMemoryDatabase("InMemoryDbForTesting");
            builder.UseInternalServiceProvider(provider);
        });
        using var serviceProvider = services.BuildServiceProvider();
        using var serviceProviderScope = services.BuildServiceProvider().CreateScope();
        var scopeServiceProvider = serviceProviderScope.ServiceProvider;
        var dbContext = scopeServiceProvider.GetRequiredService<DummyDbContext.DummyDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        //Act
        var dummyEntity = new TimeableAndSoftDeletableDummyEntity
        {
            Name = "test name"
        };

        dbContext.Add(dummyEntity);

        await dbContext.SaveChangesAsync();

        var res = await dbContext.TimeableAndSoftDeletableDummyEntities.FirstOrDefaultAsync(x => x.Id == dummyEntity.Id);

        dummyEntity.Name = "test name updated";
        await dbContext.SaveChangesAsync();

        //Assert
        res?.Name.Should().BeEquivalentTo(dummyEntity.Name);
        res?.CreatedOn.Should().BeExactly(dummyEntity.CreatedOn);
        res?.CreatedById.Should().BeSameAs(dummyEntity.CreatedById);
        res?.ModifiedOn.Should().BeExactly(now);
        res?.ModifiedById.Should().BeSameAs(userId);
        res?.IsDeleted.Should().BeFalse();
        res?.DeletedOn.Should().BeNull();

    }


    [Fact]
    public async Task Update_TimeableDummyEntity_and_test_timeable_props_will_fill()
    {
        //Arrange
        var services = new ServiceCollection();
        var now = DateTime.Now;
        var userId = "64624";

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(_ => new DefaultHttpContext()
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", userId) }, "test type", "sub", null))
        });

        var systemClock = Substitute.For<ISystemClock>();
        systemClock.UtcNow.Returns(_ => now);

        services.AddSingleton(provider => httpContextAccessor);
        services.AddSingleton(provider => systemClock);

        services.AddEntityFrameworkInMemoryDatabase();

        services.AddDbContextPool<DummyDbContext.DummyDbContext>((provider, builder) =>
        {
            builder.EnableSensitiveDataLogging();
            builder.UseInMemoryDatabase("InMemoryDbForTesting");
            builder.UseInternalServiceProvider(provider);
        });
        using var serviceProvider = services.BuildServiceProvider();
        using var serviceProviderScope = services.BuildServiceProvider().CreateScope();
        var scopeServiceProvider = serviceProviderScope.ServiceProvider;
        var dbContext = scopeServiceProvider.GetRequiredService<DummyDbContext.DummyDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        //Act
        var dummyEntity = new TimeableDummyEntity
        {
            Name = "test name",
        };

        dbContext.Add(dummyEntity);

        await dbContext.SaveChangesAsync();

        var res = await dbContext.TimeableDummyEntities.FirstOrDefaultAsync(x => x.Id == dummyEntity.Id);

        dummyEntity.CreatedById = "6060";
        dummyEntity.CreatedOn = now.AddDays(-1);
        dummyEntity.Name = "test name updated";
        await dbContext.SaveChangesAsync();

        //Assert
        res?.Name.Should().BeEquivalentTo(dummyEntity.Name);
        res?.CreatedOn.Should().BeExactly(dummyEntity.CreatedOn);
        res?.CreatedById.Should().BeSameAs(dummyEntity.CreatedById);
        res?.ModifiedOn.Should().BeExactly(now);
        res?.ModifiedById.Should().BeSameAs(userId);
    }

    [Fact]
    public async Task Update_TimeableAndSoftDeletableDummyEntity_and_test_softdeletable_and_timeable_props_will_fill()
    {
        //Arrange
        var services = new ServiceCollection();
        var now = DateTime.Now;
        var userId = "64624";

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(_ => new DefaultHttpContext()
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", userId) }, "test type", "sub", null))
        });

        var systemClock = Substitute.For<ISystemClock>();
        systemClock.UtcNow.Returns(_ => now);

        services.AddSingleton(provider => httpContextAccessor);
        services.AddSingleton(provider => systemClock);

        services.AddEntityFrameworkInMemoryDatabase();

        services.AddDbContextPool<DummyDbContext.DummyDbContext>((provider, builder) =>
        {
            builder.EnableSensitiveDataLogging();
            builder.UseInMemoryDatabase("InMemoryDbForTesting");
            builder.UseInternalServiceProvider(provider);
        });
        using var serviceProvider = services.BuildServiceProvider();
        using var serviceProviderScope = services.BuildServiceProvider().CreateScope();
        var scopeServiceProvider = serviceProviderScope.ServiceProvider;
        var dbContext = scopeServiceProvider.GetRequiredService<DummyDbContext.DummyDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        //Act
        var dummyEntity = new TimeableAndSoftDeletableDummyEntity
        {
            Name = "test name"
        };

        dbContext.Add(dummyEntity);

        await dbContext.SaveChangesAsync();

        var res = await dbContext.TimeableAndSoftDeletableDummyEntities.FirstOrDefaultAsync(x => x.Id == dummyEntity.Id);

        dummyEntity.CreatedById = "6060";
        dummyEntity.CreatedOn = now.AddDays(-1);
        dummyEntity.Name = "test name updated";

        await dbContext.SaveChangesAsync();

        //Assert
        res?.Name.Should().BeEquivalentTo(dummyEntity.Name);
        res?.CreatedOn.Should().BeExactly(dummyEntity.CreatedOn);
        res?.CreatedById.Should().BeSameAs(dummyEntity.CreatedById);
        res?.ModifiedOn.Should().BeExactly(now);
        res?.ModifiedById.Should().BeSameAs(userId);
        res?.IsDeleted.Should().BeFalse();
        res?.DeletedOn.Should().BeNull();

    }

    [Fact]
    public async Task Delete_TimeableAndSoftDeletableDummyEntity_and_test_softdeletable_and_timeable_props_will_fill()
    {
        //Arrange
        var services = new ServiceCollection();
        var now = DateTime.Now;
        var userId = "64624";

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(_ => new DefaultHttpContext()
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", userId) }, "test type", "sub", null))
        });

        var systemClock = Substitute.For<ISystemClock>();
        systemClock.UtcNow.Returns(_ => now);

        services.AddSingleton(provider => httpContextAccessor);
        services.AddSingleton(provider => systemClock);

        services.AddEntityFrameworkInMemoryDatabase();

        services.AddDbContextPool<DummyDbContext.DummyDbContext>((provider, builder) =>
        {
            builder.EnableSensitiveDataLogging();
            builder.UseInMemoryDatabase("InMemoryDbForTesting");
            builder.UseInternalServiceProvider(provider);
        });
        using var serviceProvider = services.BuildServiceProvider();
        using var serviceProviderScope = services.BuildServiceProvider().CreateScope();
        var scopeServiceProvider = serviceProviderScope.ServiceProvider;
        var dbContext = scopeServiceProvider.GetRequiredService<DummyDbContext.DummyDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        //Act
        var dummyEntity = new TimeableAndSoftDeletableDummyEntity
        {
            Name = "test name"
        };

        dbContext.Add(dummyEntity);

        await dbContext.SaveChangesAsync();

        var res = await dbContext.TimeableAndSoftDeletableDummyEntities.FirstOrDefaultAsync(x => x.Id == dummyEntity.Id);

        dbContext.Remove(dummyEntity);
        await dbContext.SaveChangesAsync();

        //Assert
        res?.Name.Should().BeEquivalentTo(dummyEntity.Name);
        res?.CreatedOn.Should().BeExactly(dummyEntity.CreatedOn);
        res?.CreatedById.Should().BeSameAs(dummyEntity.CreatedById);
        res?.ModifiedOn.Should().BeExactly(now);
        res?.ModifiedById.Should().BeSameAs(userId);
        res?.IsDeleted.Should().BeTrue();
        res?.DeletedOn.Should().BeExactly(now);

    }
    [Fact]
    public async Task Delete_SoftDeletableDummyEntity_and_test_softdeletable_props_will_fill()
    {
        //Arrange
        var services = new ServiceCollection();
        var now = DateTime.Now;
        var userId = "64624";

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(_ => new DefaultHttpContext()
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", userId) }, "test type", "sub", null))
        });

        var systemClock = Substitute.For<ISystemClock>();
        systemClock.UtcNow.Returns(_ => now);

        services.AddSingleton(provider => httpContextAccessor);
        services.AddSingleton(provider => systemClock);

        services.AddEntityFrameworkInMemoryDatabase();

        services.AddDbContextPool<DummyDbContext.DummyDbContext>((provider, builder) =>
        {
            builder.EnableSensitiveDataLogging();
            builder.UseInMemoryDatabase("InMemoryDbForTesting");
            builder.UseInternalServiceProvider(provider);
        });
        using var serviceProvider = services.BuildServiceProvider();
        using var serviceProviderScope = services.BuildServiceProvider().CreateScope();
        var scopeServiceProvider = serviceProviderScope.ServiceProvider;
        var dbContext = scopeServiceProvider.GetRequiredService<DummyDbContext.DummyDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        //Act
        var dummyEntity = new SoftDeletableDummyEntity
        {
            Name = "test name"
        };

        dbContext.Add(dummyEntity);

        await dbContext.SaveChangesAsync();

        var res = await dbContext.SoftDeletableDummyEntities.FirstOrDefaultAsync(x => x.Id == dummyEntity.Id);

        dbContext.Remove(dummyEntity);
        await dbContext.SaveChangesAsync();
        //Assert
        res?.Name.Should().BeEquivalentTo(dummyEntity.Name);
        res?.IsDeleted.Should().BeTrue();
        res?.DeletedOn.Should().BeExactly(now);

    }


}
