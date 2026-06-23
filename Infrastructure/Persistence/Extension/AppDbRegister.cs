using Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Persistence.Context;
using Persistence.Identity;
using System.Data;
namespace Persistence.Extension
{
    public static class AppDbRegister
    {
        public static IServiceCollection AddContext(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Sql Config bulunamadı.");
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<IDbConnection>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("Default");

                return new SqlConnection(connectionString);
            });
            
            //redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "WalletApp_"; // redis cache anahtarı 
            });

            //mongodb

            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

            services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                var mongoSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);

                // 🎯 YENİ VERSİYON ÇÖZÜMÜ: Guid ayarını sadece Mongo cluster'ına özel kilitliyoruz
                mongoSettings.ClusterConfigurator = cb =>
                {
                    cb.Subscribe<MongoDB.Driver.Core.Events.CommandStartedEvent>(e => { });
                };

                return new MongoClient(mongoSettings);
            });

            services.AddScoped<IMongoDatabase>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                return client.GetDatabase(settings.DatabaseName);
            });

            return services;
        }
    }
}
