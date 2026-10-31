using System.ComponentModel;
using Application.Features.Wallet.Queries;
using MediatR;

namespace Infrastructure.Agent;

public sealed class WalletAgentTools(IMediator mediator)
{
    public IReadOnlyList<TransferContactResult>? LastContactResults { get; private set; }

    public void ClearContactResults() => LastContactResults = null;

    [Description("Oturum açmış kullanıcının cüzdan kodunu ve güncel bakiyesini getirir.")]
    public async Task<WalletSummaryToolResult> GetWalletSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetWalletSummaryQuery(), cancellationToken);

        if (result.IsFailure || result.Data is null)
        {
            return new WalletSummaryToolResult(
                Success: false,
                WalletCode: null,
                Balance: null,
                Error: result.Error ?? "Cüzdan bilgisi alınamadı.");
        }

        return new WalletSummaryToolResult(
            Success: true,
            WalletCode: result.Data.Code,
            Balance: result.Data.Balance,
            Error: null);
    }

    /// <summary>İsim, soyisim, kullanıcı adı veya e-posta ile cüzdanı olan kullanıcıları arar.</summary>
    [Description("İsim, soyisim, tam ad, kullanıcı adı veya e-posta adresine göre cüzdanı olan kullanıcıları arar. İlk 10 sonucu döndürür.")]
    public async Task<ContactSearchToolResult> SearchTransferContactsAsync(
        [Description("Aranacak isim, soyisim, kullanıcı adı veya e-posta parçası.")] string search,
        CancellationToken cancellationToken = default)
    {
        var normalizedSearch = search?.Trim() ?? string.Empty;
        if (normalizedSearch.Length == 0 || normalizedSearch.Length > 100)
        {
            LastContactResults = null;
            return new ContactSearchToolResult(false, [], 0, "Arama metni 1–100 karakter arasında olmalıdır.");
        }

        var result = await mediator.Send(
            new SearchTransferContactsQuery(normalizedSearch, Page: 1, PageSize: 10),
            cancellationToken);

        if (result.IsFailure || result.Data is null)
        {
            LastContactResults = null;
            return new ContactSearchToolResult(false, [], 0, result.Error ?? "Kişiler aranamadı.");
        }

        LastContactResults = result.Data.Items;
        return new ContactSearchToolResult(true, result.Data.Items, result.Data.TotalCount, null);
    }
}

public sealed record WalletSummaryToolResult(
    bool Success,
    string? WalletCode,
    decimal? Balance,
    string? Error);

/// <summary>Kişi arama aracının modele döndürdüğü sonuç.</summary>
public sealed record ContactSearchToolResult(
    bool Success,
    IReadOnlyList<TransferContactResult> Contacts,
    int TotalCount,
    string? Error);
