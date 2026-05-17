using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Enums;

namespace SafeConcierge.Application.Users.Commands;

public record CreateUserCommand(string Name, string Email, string Password, UserRole Role,
    Guid CondominiumId, Guid? ApartmentId, Guid RequestedByUserId) : IRequest<Result<Guid>>;

public record UpdateUserCommand(Guid Id, string Name, string Email, UserRole Role,
    Guid? ApartmentId, Guid RequestedByUserId) : IRequest<Result<bool>>;

public record DeactivateUserCommand(Guid Id, Guid RequestedByUserId) : IRequest<Result<bool>>;

public record ReactivateUserCommand(Guid Id, Guid RequestedByUserId) : IRequest<Result<bool>>;
