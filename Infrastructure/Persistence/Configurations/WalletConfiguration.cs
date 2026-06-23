using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");

        builder.HasIndex(w => w.Code).IsUnique();

        builder.Property(w => w.Code).HasMaxLength(12).IsRequired().UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(w => w.Balance).HasPrecision(18, 2);

        builder.HasOne(w => w.User)
               .WithOne()
               .HasForeignKey<Wallet>(w => w.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}