namespace SafeConcierge.Domain.Models;

public class PickupCode : Base
{
    public Guid PackageId { get; set; }
    public Package Package { get; set; }
    public string Code { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool Used { get; set; }
}