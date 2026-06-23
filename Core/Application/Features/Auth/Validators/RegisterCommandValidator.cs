using Application.Features.Auth.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Validators
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCreateCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.FirstName)
                        .NotEmpty().WithMessage("FirstName Boş olamaz")
                        .MinimumLength(2).WithMessage("Ad en az 2 karakter olmalıdır.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta adresi boş geçilemez.")
                .EmailAddress().WithMessage("Geçerli bir e-posta formatı giriniz.");
        }
    }
}
