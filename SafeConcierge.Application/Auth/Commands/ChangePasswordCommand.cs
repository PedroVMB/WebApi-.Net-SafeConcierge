using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;
using MediatR;

namespace SafeConcierge.Application.Auth.Commands;

public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword) : IRequest<Result<bool>>;

