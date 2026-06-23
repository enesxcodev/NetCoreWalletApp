using Application.Common;
using Application.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomBaseController : ControllerBase
{
    [NonAction]
    public IActionResult CreateActionResult<T>(Result<T> result)
    {
        // Direkt gelen durum kodunun int değerini HTTP Status Code olarak gömüyoruz
        return result.Status switch
        {
            ResultStatus.NoContent => NoContent(),
            ResultStatus.Created => Created("", result),
            ResultStatus.Ok => Ok(result),
            _ => new ObjectResult(result) { StatusCode = (int)result.Status }
        };
    }

    [NonAction]
    public IActionResult CreateActionResult(Result result)
    {
        return result.Status switch
        {
            ResultStatus.NoContent => NoContent(),
            _ => new ObjectResult(result) { StatusCode = (int)result.Status }
        };
    }
}