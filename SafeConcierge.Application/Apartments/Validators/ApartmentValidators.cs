using FluentValidation;
using SafeConcierge.Application.Apartments.Commands;

namespace SafeConcierge.Application.Apartments.Validators;

public class CreateApartmentCommandValidator : AbstractValidator<CreateApartmentCommand>
{
    public CreateApartmentCommandValidator()
    {
        RuleFor(x => x.Number).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Floor).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TowerId).NotEmpty();
    }
}

