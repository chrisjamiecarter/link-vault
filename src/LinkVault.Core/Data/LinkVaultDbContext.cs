using LinkVault.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkVault.Core.Data;

public class LinkVaultDbContext(DbContextOptions<LinkVaultDbContext> options)
    : DbContext(options)
{
    public DbSet<Link> Links => Set<Link>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }
}
