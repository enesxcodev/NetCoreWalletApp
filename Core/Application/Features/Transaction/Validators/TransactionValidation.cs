using Application.Features.Transaction.Commands;
using FluentValidation;

namespace Application.Features.Transaction.Validators
{
    public class TransactionValidation : AbstractValidator<TransactionCreateCommand>
    {
        public TransactionValidation()
        {
            RuleFor(x => x.WalletCode)
                .NotEmpty().WithMessage("Alıcı cüzdan kodu boş olamaz.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Miktar 0 veya ondan küçük olamaz.");
        }
    }
}
