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
[Route("api/v{apiVersion:apiVersion}/chats")]
public class ChatController(IChatService service)
    : ControllerBase
{
    [HttpGet("")]
    public async Task<IResult> GetAllChatsAsync(
        Guid userId,
        [FromQuery] PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await service.GetAllForUserAsync(userId, request, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpGet("{chatId:guid}")]
    public async Task<IResult> GetChatByIdAsync(Guid chatId, CancellationToken cancellationToken = default)
    {
        var result = await service.GetByIdAsync(chatId, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpPost("")]
    public async Task<IResult> CreateChat(ChatRequest request, CancellationToken cancellationToken = default)
    {
        var result = await service.CreateAsync(request, cancellationToken);

        return result.ToAspResult(value => Results.Created(string.Empty, value));
    }
}
