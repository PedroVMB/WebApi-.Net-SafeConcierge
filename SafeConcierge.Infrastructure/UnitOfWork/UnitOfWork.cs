using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Infrastructure.Data;
using SafeConcierge.Infrastructure.Repositories;

namespace SafeConcierge.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IApartmentRepository? _apartments;
    private IAuditLogRepository? _auditLogs;
    private ICondominiumRepository? _condominiums;
    private IDeliveryCompanyRepository? _deliveryCompanies;
    private IDeliveryLogRepository? _deliveryLogs;
    private INotificationRepository? _notifications;
    private IPackageRepository? _packages;
    private IPickupCodeRepository? _pickupCodes;
    private ITowerRepository? _towers;
    private IUserRepository? _users;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IApartmentRepository Apartments
        => _apartments ??= new ApartmentRepository(_context);

    public IAuditLogRepository AuditLogs
        => _auditLogs ??= new AuditLogRepository(_context);

    public ICondominiumRepository Condominiums
        => _condominiums ??= new CondominiumRepository(_context);

    public IDeliveryCompanyRepository DeliveryCompanies
        => _deliveryCompanies ??= new DeliveryCompanyRepository(_context);

    public IDeliveryLogRepository DeliveryLogs
        => _deliveryLogs ??= new DeliveryLogRepository(_context);

    public INotificationRepository Notifications
        => _notifications ??= new NotificationRepository(_context);

    public IPackageRepository Packages
        => _packages ??= new PackageRepository(_context);

    public IPickupCodeRepository PickupCodes
        => _pickupCodes ??= new PickupCodeRepository(_context);

    public ITowerRepository Towers
        => _towers ??= new TowerRepository(_context);

    public IUserRepository Users
        => _users ??= new UserRepository(_context);

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}

