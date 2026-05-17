namespace SafeConcierge.Domain.Models;

public class Tower : Base
{
    public string Name { get; set; } = string.Empty;
    public Guid CondominiumId { get; set; }
    public Condominium Condominium { get; set; } = null!;
    public ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
}