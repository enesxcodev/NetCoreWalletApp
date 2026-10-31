using Application.Features.Assistant;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;

namespace Infrastructure.Agent;

/// <summary>OpenAI ile cüzdan asistanını çalıştırır.</summary>
public sealed class WalletAssistant(
    OpenAIClient openAIClient,
    WalletAgentTools walletAgentTools,
    IConfiguration configuration) : IWalletAssistant
{
    private const string Instructions = """
        Türkçe yanıt veren bir cüzdan asistanısın.
        Bakiye sorularında get_wallet_summary aracını kullan.
        Bir kullanıcıyı bulma veya arama isteğinde search_transfer_contacts aracını kullan.
        Kişi ararken kullanıcının verdiği isim, soyisim, kullanıcı adı veya e-posta metnini aynen arama aracına gönder.
        Arama sonuçlarını kullanıcıya kısa bir cümleyle açıkla. Kişi listesini ayrıca uygulama tablo olarak gösterecek.
        Araçtan gelmeyen cüzdan veya bakiye bilgisini uydurma.
        Para gönderme, para yatırma veya para çekme işlemi yapma.
        Bu işlemler istenirse bu sürümde yalnızca bilgi verebildiğini söyle.
        """;

    public async Task<AssistantReply> AskAsync(string message, CancellationToken cancellationToken)
    {
        var model = configuration["OpenAI:Model"];
        if (string.IsNullOrWhiteSpace(model))
            throw new InvalidOperationException("OpenAI:Model ayarı bulunamadı.");

        walletAgentTools.ClearContactResults();

        var agent = openAIClient
            .GetChatClient(model)
            .AsIChatClient()
            .AsAIAgent(
                instructions: Instructions,
                name: "WalletAssistant",
                tools:
                [
                    AIFunctionFactory.Create(
                        walletAgentTools.GetWalletSummaryAsync,
                        name: "get_wallet_summary",
                        description: "Giriş yapmış kullanıcının cüzdan kodunu ve güncel bakiyesini getirir."),
                    AIFunctionFactory.Create(
                        walletAgentTools.SearchTransferContactsAsync,
                        name: "search_transfer_contacts",
                        description: "İsim, soyisim, kullanıcı adı veya e-posta ile cüzdan sahibi kullanıcı arar.")
                ]);

        var response = await agent.RunAsync(message, cancellationToken: cancellationToken);
        return new AssistantReply(response.Text, walletAgentTools.LastContactResults);
    }
}
