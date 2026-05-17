using FluentValidation;
using SafeConcierge.Application.Condominiums.Commands;

namespace SafeConcierge.Application.Condominiums.Validators;

public class CreateCondominiumCommandValidator : AbstractValidator<CreateCondominiumCommand>
{
    public CreateCondominiumCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Cnpj).NotEmpty().Length(14).Matches("^[0-9]+$").WithMessage("CNPJ must contain only digits.");
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
    }
}

public class UpdateCondominiumCommandValidator : AbstractValidator<UpdateCondominiumCommand>
{
    public UpdateCondominiumCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Cnpj).NotEmpty().Length(14).Matches("^[0-9]+$").WithMessage("CNPJ must contain only digits.");
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
    }
}

