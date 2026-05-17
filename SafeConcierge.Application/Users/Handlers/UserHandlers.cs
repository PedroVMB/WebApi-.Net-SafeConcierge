using MediatR;
using Microsoft.AspNetCore.Identity;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Application.Users.Commands;
using SafeConcierge.Application.Users.Queries;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;

namespace SafeConcierge.Application.Users.Handlers;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly UserManager<User> _userManager;
    private readonly IAuditLogService _audit;

    public CreateUserCommandHandler(UserManager<User> userManager, IAuditLogService audit)
    {
        _userManager = userManager;
        _audit = audit;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null) return Result<Guid>.Fail("Email already in use.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            Name = request.Name,
            Role = request.Role,
            CondominiumId = request.CondominiumId,
            ApartmentId = request.ApartmentId,
            Active = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return Result<Guid>.Fail(string.Join("; ", result.Errors.Select(e => e.Description)));

        await _audit.LogAsync(request.RequestedByUserId, nameof(User), user.Id, "CREATE", cancellationToken: cancellationToken);
        return Result<Guid>.Ok(user.Id);
    }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<bool>>
{
    private readonly UserManager<User> _userManager;
    private readonly IAuditLogService _audit;

    public UpdateUserCommandHandler(UserManager<User> userManager, IAuditLogService audit)
    {
        _userManager = userManager;
        _audit = audit;
    }

    public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user is null) return Result<bool>.Fail("User not found.");

        user.Name = request.Name;
        user.Email = request.Email;
        user.UserName = request.Email;
        user.Role = request.Role;
        user.ApartmentId = request.ApartmentId;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Result<bool>.Fail(string.Join("; ", result.Errors.Select(e => e.Description)));

        await _audit.LogAsync(request.RequestedByUserId, nameof(User), request.Id, "UPDATE", cancellationToken: cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result<bool>>
{
    private readonly UserManager<User> _userManager;
    private readonly IAuditLogService _audit;

    public DeactivateUserCommandHandler(UserManager<User> userManager, IAuditLogService audit)
    {
        _userManager = userManager;
        _audit = audit;
    }

    public async Task<Result<bool>> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user is null) return Result<bool>.Fail("User not found.");

        user.Active = false;
        await _userManager.UpdateAsync(user);
        await _audit.LogAsync(request.RequestedByUserId, nameof(User), request.Id, "DEACTIVATE", cancellationToken: cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public class ReactivateUserCommandHandler : IRequestHandler<ReactivateUserCommand, Result<bool>>
{
    private readonly UserManager<User> _userManager;
    private readonly IAuditLogService _audit;

    public ReactivateUserCommandHandler(UserManager<User> userManager, IAuditLogService audit)
    {
        _userManager = userManager;
        _audit = audit;
    }

    public async Task<Result<bool>> Handle(ReactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user is null) return Result<bool>.Fail("User not found.");

        user.Active = true;
        await _userManager.UpdateAsync(user);
        await _audit.LogAsync(request.RequestedByUserId, nameof(User), request.Id, "REACTIVATE", cancellationToken: cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public class GetUsersByCondominiumQueryHandler : IRequestHandler<GetUsersByCondominiumQuery, IEnumerable<UserDto>>
{
    private readonly IUnitOfWork _uow;
    public GetUsersByCondominiumQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<IEnumerable<UserDto>> Handle(GetUsersByCondominiumQuery request, CancellationToken cancellationToken)
    {
        var query = _uow.Users.GetByCondominiumId(request.CondominiumId).AsQueryable();
        if (request.Role.HasValue) query = query.Where(u => u.Role == request.Role.Value);
        if (request.Active.HasValue) query = query.Where(u => u.Active == request.Active.Value);
        var result = query.Select(u => new UserDto(u.Id, u.Name, u.Email!, u.Role.ToString(), u.CondominiumId, u.ApartmentId, u.Active, u.CreatedAt));
        return Task.FromResult<IEnumerable<UserDto>>(result.ToList());
    }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUnitOfWork _uow;
    public GetUserByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var u = _uow.Users.GetById(request.Id);
        if (u is null) return Task.FromResult(Result<UserDto>.Fail("User not found."));
        return Task.FromResult(Result<UserDto>.Ok(new UserDto(u.Id, u.Name, u.Email!, u.Role.ToString(), u.CondominiumId, u.ApartmentId, u.Active, u.CreatedAt)));
    }
}

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    private readonly IUnitOfWork _uow;
    public GetCurrentUserQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var u = _uow.Users.GetById(request.UserId);
        if (u is null) return Task.FromResult(Result<UserDto>.Fail("User not found."));
        return Task.FromResult(Result<UserDto>.Ok(new UserDto(u.Id, u.Name, u.Email!, u.Role.ToString(), u.CondominiumId, u.ApartmentId, u.Active, u.CreatedAt)));
    }
}

