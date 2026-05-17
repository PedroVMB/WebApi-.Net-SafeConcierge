using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;
using SafeConcierge.Infrastructure.Data;

namespace SafeConcierge.Infrastructure.Services;

public class PickupCodeService : IPickupCodeService
{
    private readonly AppDbContext _context;
    private static readonly TimeSpan CodeExpiration = TimeSpan.FromDays(7);

    public PickupCodeService(AppDbContext context) => _context = context;

    public async Task<PickupCode> GenerateAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        string code;
        // RN007 — gerar código aleatório único, não previsível
        do
        {
            code = GenerateRandomCode();
        } while (_context.PickupCodes.Any(p => p.Code == code));

        var pickupCode = new PickupCode
        {
            Id = Guid.NewGuid(),
            PackageId = packageId,
            Code = code,
            ExpiresAt = DateTime.UtcNow.Add(CodeExpiration),
            Used = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.PickupCodes.Add(pickupCode);
        await _context.SaveChangesAsync(cancellationToken);
        return pickupCode;
    }

    public async Task InvalidateByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        var activeCodes = _context.PickupCodes
            .Where(p => p.PackageId == packageId && !p.Used)
            .ToList();

        foreach (var code in activeCodes)
        {
            code.Used = true;
            code.UpdatedAt = DateTime.UtcNow;
        }

        if (activeCodes.Any())
            await _context.SaveChangesAsync(cancellationToken);
    }

    private static string GenerateRandomCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

