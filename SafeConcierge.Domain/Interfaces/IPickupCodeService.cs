using SafeConcierge.Domain.Models;

namespace SafeConcierge.Domain.Interfaces;

public interface IPickupCodeService
{
    Task<PickupCode> GenerateAsync(Guid packageId, CancellationToken cancellationToken = default);
    Task InvalidateByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default);
}

