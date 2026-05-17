using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;
using MediatR;

namespace SafeConcierge.Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponseDto>>;

