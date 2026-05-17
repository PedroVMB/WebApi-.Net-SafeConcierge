using MediatR;
using Microsoft.AspNetCore.Identity;
using SafeConcierge.Application.Auth.Commands;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Models;

namespace SafeConcierge.Application.Auth.Handlers;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
{
    private readonly UserManager<User> _userManager;

    public ChangePasswordCommandHandler(UserManager<User> userManager) => _userManager = userManager;

    public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null) return Result<bool>.Fail("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        return result.Succeeded
            ? Result<bool>.Ok(true)
            : Result<bool>.Fail(string.Join("; ", result.Errors.Select(e => e.Description)));
    }
}

