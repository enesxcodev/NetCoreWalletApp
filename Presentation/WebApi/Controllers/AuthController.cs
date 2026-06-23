using Application.Common;
using Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class AuthController(IMediator mediator) : CustomBaseController
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCreateCommand request)
        {
            var result = await mediator.Send(request);
            return CreateActionResult(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand request)
        {
            // İstek pipeline'dan geçer, validasyon hatası yoksa Handler çalışır ve buraya düşer
            var result = await mediator.Send(request);

            // CustomBaseController içindeki sihirli metot durum kodunu (200 mi, 400 mü) otomatik ayarlar
            return CreateActionResult(result);
        }
    }
}
