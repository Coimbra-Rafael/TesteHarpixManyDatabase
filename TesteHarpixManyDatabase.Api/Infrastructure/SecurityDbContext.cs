using Microsoft.EntityFrameworkCore;
using TesteHarpixManyDatabase.Api.Domain.Entities;

namespace TesteHarpixManyDatabase.Api.Infrastructure;

public class SecurityDbContext(DbContextOptions<SecurityDbContext> option) : DbContext(option)
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
           .HasMany(u => u.Companies)
           .WithMany(c => c.Users)
           .UsingEntity<Dictionary<string, object>>(
               "UserCompany", // Nome da tabela de junção
               r => r.HasOne<Company>().WithMany().HasForeignKey("CompanyId"),
               l => l.HasOne<User>().WithMany().HasForeignKey("UserId"),
               je =>
               {
                   je.HasKey("UserId", "CompanyId");
                   je.ToTable("UserCompany");
               });
    }
}
