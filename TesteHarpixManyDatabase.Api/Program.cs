using Microsoft.EntityFrameworkCore;
using TesteHarpixManyDatabase.Api.Domain.Entities;
using TesteHarpixManyDatabase.Api.Infrastructure;
using TesteHarpixManyDatabase.Api.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("X-Company-Id", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "X-Company-Id",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Description = "Informe o ID da empresa (tenant)"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "X-Company-Id"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<SecurityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SecurityDb")));

builder.Services.AddScoped<ICompanyResolver, CompanyResolver>();
builder.Services.AddScoped(sp =>
{
    var http = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    var resolver = sp.GetRequiredService<ICompanyResolver>();

    var companyIdHeader = http?.Request.Headers["X-Company-Id"].FirstOrDefault();
    if (string.IsNullOrWhiteSpace(companyIdHeader))
        throw new Exception("CompanyId não informado");

    var connection = resolver.GetConnectionStringAsync(Convert.ToInt32(companyIdHeader)).Result;

    var options = new DbContextOptionsBuilder<CompanyDbContext>()
        .UseNpgsql(connection)
        .Options;

    return new CompanyDbContext(options);
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapPost("/companies", async (SecurityDbContext db, ICompanyResolver resolver, CancellationToken ct) =>
{
    var company = new Company
    {
        Name = "teste "
    };


    await db.Companies.AddAsync(company, ct);
    await db.SaveChangesAsync(ct);
    await resolver.EnsureCompanyDatabaseAsync(company.Id);
    return Results.Created($"/companies/{company.Id}", company);
});
app.MapPost("/products", async (CompanyDbContext db, Product product, CancellationToken ct) =>
{
    await db.Products.AddAsync(product, ct);
    await db.SaveChangesAsync(ct);
    return Results.Created($"/products/{product.Id}", product);
});

await app.RunAsync();