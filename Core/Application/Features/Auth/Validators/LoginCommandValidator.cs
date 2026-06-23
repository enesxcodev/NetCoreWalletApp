using Application.Features.Auth.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Validators
{
    internal class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("username boş olamaz");
            RuleFor(x => x.Password).MinimumLength(6).WithMessage("şifre alanı minimum 6 karakter olmalıdır");
        }
    }
}
