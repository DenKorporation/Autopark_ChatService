using Asp.Versioning;
using ChatService.API.Extensions;
using ChatService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{apiVersion:apiVersion}/users")]
public class UserController(IUserService service)
    : ControllerBase
{
    [HttpGet("")]
    public async Task<IResult> GetAllUsersAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await service.GetAllAsync(cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IResult> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await service.GetByIdAsync(userId, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }
}
