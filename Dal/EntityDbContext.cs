using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Models;

namespace Dal;

public sealed class EntityDbContext: IdentityDbContext<User, IdentityRole<int>, int>
{
    public EntityDbContext() { }

    public EntityDbContext(DbContextOptions<EntityDbContext> optionsBuilderOptions) : base(optionsBuilderOptions)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
            
        builder.ApplyConfigurationsFromAssembly(typeof(EntityDbContext).Assembly);
    }
}