using Microsoft.AspNetCore.Identity;
using SafeConcierge.Domain.Enums;

namespace SafeConcierge.Domain.Models;

public class User : IdentityUser<Guid>
{
    public Guid CondominiumId { get; set; }
    public Condominium Condominium { get; set; }

    public Guid? ApartmentId { get; set; }
    public Apartment? Apartment { get; set; }

    public string Name { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public bool Active { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}