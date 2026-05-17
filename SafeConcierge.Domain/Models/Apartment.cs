namespace SafeConcierge.Domain.Models;

public class Apartment : Base
{
    public string Number { get; set; } = string.Empty;
    public int Floor { get; set; }
    public Guid TowerId { get; set; }
    public Tower Tower { get; set; } = null!;
    public ICollection<User> Residents { get; set; } = new List<User>();
}