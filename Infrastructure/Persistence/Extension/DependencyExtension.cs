using Application.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Consumers;
using Persistence.Context;
using Persistence.Identity;
using Persistence.Repository;
using Persistence.Services;

namespace Persistence.Extension
{
    public static class DependencyExtension
    {
        public static IServiceCollection PersistenceRegisters(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<AppIdentityUser, IdentityRole<Guid>>(options =>
            {
                // ===== KULLANICI AYARLARI =====
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz0123456789_.";

                // ===== ŞİFRE POLİTİKASI =====
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;

                // ===== KİLİTLEME POLİTİKASI =====
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;

                // ===== SIGN-IN AYARLARI =====
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
          .AddEntityFrameworkStores<AppDbContext>()
          .AddDefaultTokenProviders();

            // ===== REPOSITORY PATTERN KAYITLARI =====
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMessageBus, MessageBus>();

            // ===== USER-SPECIFIC REPOSITORY KAYITLARI =====
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<ITransaction, TransactionRepository>();
            services.AddScoped<ITransactionAuditService, TransactionAuditService>(); //mongo

            // ===== MASSTRANSIT & RABBITMQ CONFIGURATION =====
            services.AddMassTransit(x =>
            {
                x.AddConsumer<UserRegisteredEventConsumer>();
                x.AddConsumer<Application.Consumers.MoneyTransferredConsumer>(); // 🎯 1. Yeni consumer'ı MassTransit'e tanıttık

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqSettings = configuration.GetSection("RabbitMqSettings");

                    cfg.Host(rabbitMqSettings["Host"], h =>
                    {
                        h.Username(rabbitMqSettings["Username"]!);
                        h.Password(rabbitMqSettings["Password"]!);
                    });

                    // 2. 🚀 kullanıcı kaydoldgnda oluşturulacak cuzdanın queusi
                    cfg.ReceiveEndpoint("user-registered-queue", e =>
                    {
                        e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                        e.ConfigureConsumer<UserRegisteredEventConsumer>(context);
                    });

                    // 🎯 3. YENİ: Para transferi yapıldığında MongoDB'ye yazacak olan yeni kuyruğumuz!
                    cfg.ReceiveEndpoint("money-transferred-queue", e =>
                    {
                        // Hata alursa 5 saniyede bir 3 kere tekrar dene (Kurumsal Defans)
                        e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                        e.ConfigureConsumer<Application.Consumers.MoneyTransferredConsumer>(context);
                    });
                });
            });

            return services;
        }
    }
}
