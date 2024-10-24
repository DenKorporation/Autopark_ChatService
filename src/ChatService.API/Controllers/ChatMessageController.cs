using Asp.Versioning;
using ChatService.API.Extensions;
using ChatService.BLL.DTOs.Request;
using ChatService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{apiVersion:apiVersion}/messages")]
public class ChatMessageController(IChatMessageService service)
    : ControllerBase
{
    [HttpGet("")]
    public async Task<IResult> GetAllChatMessagesAsync(
        Guid chatId,
        [FromQuery] PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await service.GetAllFromChatAsync(chatId, request, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }
}
