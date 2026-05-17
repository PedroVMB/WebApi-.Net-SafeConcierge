using MediatR;
using Microsoft.AspNetCore.Identity;
using SafeConcierge.Application.Auth.Commands;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;

namespace SafeConcierge.Application.Auth.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(UserManager<User> userManager, SignInManager<User> signInManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !user.Active)
            return Result<LoginResponseDto>.Fail("Invalid credentials.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return Result<LoginResponseDto>.Fail("Invalid credentials.");

        var token = _jwtService.GenerateToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(60);

        var userDto = new UserDto(user.Id, user.Name, user.Email!, user.Role.ToString(),
            user.CondominiumId, user.ApartmentId, user.Active, user.CreatedAt);

        return Result<LoginResponseDto>.Ok(new LoginResponseDto(token, expiresAt, userDto));
    }
}

