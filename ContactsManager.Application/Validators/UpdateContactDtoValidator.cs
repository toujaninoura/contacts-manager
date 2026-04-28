using ContactsManager.Application.DTOs.Contacts;
using FluentValidation;

namespace ContactsManager.Application.Validators;

public class UpdateContactDtoValidator : AbstractValidator<UpdateContactDto>
{
    public UpdateContactDtoValidator()
    {
        RuleFor(x => x.Nom)
            .NotEmpty().WithMessage("Le nom est requis.")
            .MaximumLength(100).WithMessage("Le nom ne peut pas depasser 100 caracteres.");

        RuleFor(x => x.Prenom)
            .NotEmpty().WithMessage("Le prenom est requis.")
            .MaximumLength(100).WithMessage("Le prenom ne peut pas depasser 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email est requis.")
            .EmailAddress().WithMessage("L'email doit etre une adresse valide.")
            .MaximumLength(256).WithMessage("L'email ne peut pas depasser 256 caracteres.");

        RuleFor(x => x.Telephone)
            .MaximumLength(20).WithMessage("Le telephone ne peut pas depasser 20 caracteres.")
            .When(x => x.Telephone is not null);
    }
}
