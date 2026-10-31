using System.ComponentModel.DataAnnotations;
using Application.Common;
using Application.Features.Assistant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
public sealed class AssistantController(IWalletAssistant assistant) : CustomBaseController
{
    [HttpPost("message")]
    public async Task<IActionResult> Message(
        [FromBody] AssistantMessageRequest request,
        CancellationToken cancellationToken)
    {
        var reply = await assistant.AskAsync(request.Message, cancellationToken);
        return CreateActionResult(Result<AssistantMessageResponse>.Success(
            new AssistantMessageResponse(reply.Text, reply.Contacts)));
    }
}

public sealed record AssistantMessageRequest(
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    string Message);

public sealed record AssistantMessageResponse(
    string Reply,
    IReadOnlyList<Application.Features.Wallet.Queries.TransferContactResult>? Contacts);
