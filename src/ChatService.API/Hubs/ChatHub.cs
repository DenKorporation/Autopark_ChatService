using ChatService.BLL.DTOs.Request;
using ChatService.BLL.Errors.Base;
using ChatService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.Hubs;

[Authorize]
public class ChatHub(
    IChatMessageService chatMessageService,
    IChatService chatService)
    : Hub<IChatClient>
{
    public async Task SendMessage(
        ChatMessageRequest request)
    {
        var chatResult = await chatService.GetByIdAsync(request.ChatId);

        if (chatResult.IsFailed)
        {
            await Clients.Caller.ReceiveError(
                chatResult.Errors.OfType<BaseError>().FirstOrDefault()!);
        }

        var result = await chatMessageService.CreateAsync(request);

        if (result.IsFailed)
        {
            await Clients.Caller.ReceiveError(result.Errors.OfType<BaseError>().FirstOrDefault()!);
        }

        var response = result.Value;
        var userIds = chatResult
            .Value
            .Participants
            .Where(id => id != request.SenderId)
            .Select(id => id.ToString())
            .ToList();

        await Clients.Users(userIds).ReceiveMessage(response);
    }
}
