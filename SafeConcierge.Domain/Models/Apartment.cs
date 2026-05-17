namespace SafeConcierge.Domain.Models;

public class Apartment
{
    public string Number { get; set; }
    public int Floor { get; set; }
    public int TowerId { get; set; }
    public Tower Tower { get; set; }
}