using Application.Common;
using Application.Common.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace WebApi.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Varsayılan olarak sistemsel bir hata (500) kabul ediyoruz
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "Sistemsel beklenmedik bir hata oluştu.";
        var resultStatus = ResultStatus.Error;

        // 🚀 Eğer hata bizim bilerek fırlattığımız bir Domain kuralıysa:
        if (exception is DomainException)
        {
            statusCode = HttpStatusCode.BadRequest; // 400 Bad Request
            message = exception.Message;            // "Yetersiz bakiye." vb.
            resultStatus = ResultStatus.BadRequest;
        }

        // Bizim standart Result formatımızı hazırlıyoruz
        var responseResult = Result.Failure(message, resultStatus);

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/json";

        // JSON olarak istemciye basıyoruz
        await httpContext.Response.WriteAsJsonAsync(responseResult, cancellationToken);

        // Hata başarıyla ele alındı, pipeline'ı durdur diyoruz
        return true;
    }
}