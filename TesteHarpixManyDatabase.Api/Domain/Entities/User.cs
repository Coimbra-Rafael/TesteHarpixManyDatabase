namespace TesteHarpixManyDatabase.Api.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Company> Companies { get; set; } = [];
}
