using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Enums;

namespace SafeConcierge.Application.Users.Queries;

public record GetUsersByCondominiumQuery(Guid CondominiumId, UserRole? Role = null, bool? Active = null)
    : IRequest<IEnumerable<UserDto>>;

public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDto>>;
public record GetCurrentUserQuery(Guid UserId) : IRequest<Result<UserDto>>;

