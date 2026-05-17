using FluentValidation;
using SafeConcierge.Application.Towers.Commands;

namespace SafeConcierge.Application.Towers.Validators;

public class CreateTowerCommandValidator : AbstractValidator<CreateTowerCommand>
{
    public CreateTowerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CondominiumId).NotEmpty();
    }
}

public class UpdateTowerCommandValidator : AbstractValidator<UpdateTowerCommand>
{
    public UpdateTowerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

