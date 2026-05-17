namespace SafeConcierge.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IApartmentRepository Apartments { get; }
    IAuditLogRepository AuditLogs { get; }
    ICondominiumRepository Condominiums { get; }
    IDeliveryCompanyRepository DeliveryCompanies { get; }
    IDeliveryLogRepository DeliveryLogs { get; }
    INotificationRepository Notifications { get; }
    IPackageRepository Packages { get; }
    IPickupCodeRepository PickupCodes { get; }
    ITowerRepository Towers { get; }
    IUserRepository Users { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}

