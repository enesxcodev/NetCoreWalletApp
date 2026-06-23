using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities
{
    public class Wallet : BaseEntity
    {
        public string Code { get; private init; } = null!;
        public decimal Balance { get; private set; }
        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;

        public Wallet() { }
        public Wallet(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Balance = 0;
            Code = GenerateCode();
        }

        public Wallet(Guid userId, string code, decimal balance)
        {
            Id = Guid.NewGuid(); 
            UserId = userId;
            Code = code;       
            Balance = balance; 
        }
        private string GenerateCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new char[8];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }

            return $"WLT-{new string(result)}";
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0) throw new DomainException("Yatırılacak tutar sıfırdan büyük olmalıdır.");
            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0) throw new DomainException("Çekilecek tutar sıfırdan büyük olmalıdır.");
            if (Balance < amount) throw new DomainException("Yetersiz bakiye.");
            Balance -= amount;
        }
    }
}
