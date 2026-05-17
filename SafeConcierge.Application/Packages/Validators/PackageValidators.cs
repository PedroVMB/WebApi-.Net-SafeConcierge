using FluentValidation;
using SafeConcierge.Application.Packages.Commands;

namespace SafeConcierge.Application.Packages.Validators;

public class RegisterPackageCommandValidator : AbstractValidator<RegisterPackageCommand>
{
    public RegisterPackageCommandValidator()
    {
        RuleFor(x => x.CondominiumId).NotEmpty();
        RuleFor(x => x.ApartmentId).NotEmpty();
        RuleFor(x => x.ResidentId).NotEmpty();
        RuleFor(x => x.ReceivedById).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.ReceivedAt).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
            .WithMessage("Received date cannot be in the future.");
    }
}

public class PickupPackageCommandValidator : AbstractValidator<PickupPackageCommand>
{
    public PickupPackageCommandValidator()
    {
        RuleFor(x => x.PackageId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.ResidentId).NotEmpty();
        RuleFor(x => x.DeliveredById).NotEmpty();
    }
}

public class CancelPackageCommandValidator : AbstractValidator<CancelPackageCommand>
{
    public CancelPackageCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}

