namespace SafeConcierge.Domain.Models;

public class DeliveryLog : Base
{
    public Guid PackageId { get; set; }
    public Package Package { get; set; }

    // Código informado ao morador
    public string PickupCode { get; set; } = string.Empty;

    // Quem recebeu a encomenda
    public Guid DeliveredToId { get; set; }
    public User DeliveredTo { get; set; }

    // Porteiro/funcionário que entregou
    public Guid DeliveredById { get; set; }
    public User DeliveredBy { get; set; }

    public DateTime DeliveredAt { get; set; }

    public string? SignatureUrl { get; set; }
}