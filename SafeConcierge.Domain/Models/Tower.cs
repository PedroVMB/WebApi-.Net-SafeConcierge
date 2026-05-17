namespace SafeConcierge.Domain.Models;

public class Tower
{
    public string Name { get; set; }
    public Condominium Condominium { get; set; }
    public int CondominiumId { get; set; }
}