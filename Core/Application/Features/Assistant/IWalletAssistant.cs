namespace Application.Features.Assistant;

using Application.Features.Wallet.Queries;

/// <summary>Cüzdan asistanına uygulama katmanından erişim sağlar.</summary>
public interface IWalletAssistant
{
    Task<AssistantReply> AskAsync(string message, CancellationToken cancellationToken);
}

/// <summary>Asistan yanıtını ve varsa bulunan kişileri taşır.</summary>
public sealed record AssistantReply(
    string Text,
    IReadOnlyList<TransferContactResult>? Contacts = null);
