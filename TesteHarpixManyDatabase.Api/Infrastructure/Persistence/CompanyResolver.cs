using Microsoft.EntityFrameworkCore;

namespace TesteHarpixManyDatabase.Api.Infrastructure.Persistence;
public interface ICompanyResolver
{
    Task<string> GetConnectionStringAsync(int id);
    Task EnsureCompanyDatabaseAsync(int companyId);
}

public class CompanyResolver(SecurityDbContext context, IConfiguration configuration) : ICompanyResolver
{
    public async Task<string> GetConnectionStringAsync(int id)
    {
        var company = await context.Companies.FindAsync(id);
        if (company == null) throw new Exception("Company não encontrado");

        return configuration.GetConnectionString("CompanyDb")!
         .Replace("{companyId}", company.Id.ToString());
    }
    public async Task EnsureCompanyDatabaseAsync(int id)
    {
        var connectionString = await GetConnectionStringAsync(id);

        var options = new DbContextOptionsBuilder<CompanyDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        using var db = new CompanyDbContext(options);
        await db.Database.EnsureCreatedAsync();
    }
}
