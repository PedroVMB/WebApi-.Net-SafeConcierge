using SafeConcierge.Domain.Enums;

namespace SafeConcierge.Domain.Models;

public class Package : Base
{
    public Guid CondominiumId { get; set; }
    public Condominium Condominium { get; set; }

    public Guid ApartmentId { get; set; }
    public Apartment Apartment { get; set; }

    // Morador destinatário
    public Guid ResidentId { get; set; }
    public User Resident { get; set; }

    public Guid? DeliveryCompanyId { get; set; }
    public DeliveryCompany? DeliveryCompany { get; set; }

    // Porteiro que recebeu a encomenda
    public Guid ReceivedById { get; set; }
    public User ReceivedBy { get; set; }

    public PackageStatus Status { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime ReceivedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }
}