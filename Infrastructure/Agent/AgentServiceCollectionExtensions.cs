using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;

namespace Infrastructure.Agent;

public static class AgentServiceCollectionExtensions
{
    /// <summary>Cüzdan asistanı servislerini kaydeder.</summary>
    public static IServiceCollection AddWalletAgent(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var apiKey = configuration["OpenAI:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenAI:ApiKey ayarı bulunamadı.");

        services.AddSingleton(new OpenAIClient(apiKey));
        services.AddScoped<WalletAgentTools>();
        services.AddScoped<Application.Features.Assistant.IWalletAssistant, WalletAssistant>();
        return services;
    }
}
