namespace SafeConcierge.Application.Common.DTOs;

public record CondominiumDto(Guid Id, string Name, string Cnpj, string Address, DateTime CreatedAt, bool IsDisabled);

public record TowerDto(Guid Id, string Name, Guid CondominiumId, DateTime CreatedAt, bool IsDisabled);

public record ApartmentDto(Guid Id, string Number, int Floor, Guid TowerId, DateTime CreatedAt, bool IsDisabled);

public record DeliveryCompanyDto(Guid Id, string Name, DateTime CreatedAt, bool IsDisabled);

public record UserDto(
    Guid Id, string Name, string Email, string Role,
    Guid CondominiumId, Guid? ApartmentId, bool Active, DateTime CreatedAt);

public record PickupCodeDto(Guid Id, Guid PackageId, string Code, DateTime ExpiresAt, bool Used);

public record PackageDto(
    Guid Id, Guid CondominiumId, Guid ApartmentId, Guid ResidentId,
    Guid? DeliveryCompanyId, Guid ReceivedById, string Status,
    string Description, DateTime ReceivedAt, DateTime? DeliveredAt,
    string? ResidentName, string? ApartmentNumber, string? DeliveryCompanyName);

public record PackageDetailDto(
    Guid Id, Guid CondominiumId, Guid ApartmentId, Guid ResidentId,
    Guid? DeliveryCompanyId, Guid ReceivedById, string Status,
    string Description, DateTime ReceivedAt, DateTime? DeliveredAt,
    string? ResidentName, string? ApartmentNumber, string? DeliveryCompanyName,
    string? ReceivedByName, PickupCodeDto? ActivePickupCode);

public record DeliveryLogDto(
    Guid Id, Guid PackageId, string PickupCode,
    Guid DeliveredToId, string? DeliveredToName,
    Guid DeliveredById, string? DeliveredByName,
    DateTime DeliveredAt, string? SignatureUrl);

public record NotificationDto(
    Guid Id, Guid UserId, Guid? PackageId,
    string Type, string Content, bool Sent, DateTime? SentAt, DateTime CreatedAt);

public record AuditLogDto(
    Guid Id, Guid UserId, string? UserName,
    string EntityName, Guid EntityId,
    string Action, string? OldData, string? NewData, DateTime CreatedAt);

public record LoginResponseDto(string Token, DateTime ExpiresAt, UserDto User);

public record DashboardSummaryDto(
    int TotalPackages, int Pending, int Delivered, int Expired, int Canceled,
    double AvgPickupTimeHours);

