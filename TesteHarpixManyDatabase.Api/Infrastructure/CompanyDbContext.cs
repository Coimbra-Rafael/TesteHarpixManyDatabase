using Microsoft.EntityFrameworkCore;
using TesteHarpixManyDatabase.Api.Domain.Entities;

namespace TesteHarpixManyDatabase.Api.Infrastructure;

public class CompanyDbContext(DbContextOptions<CompanyDbContext> option) : DbContext(option)
{
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
