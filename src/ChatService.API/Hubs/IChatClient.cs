using ChatService.BLL.DTOs.Response;
using ChatService.BLL.Errors.Base;

namespace ChatService.API.Hubs;

public interface IChatClient
{
    public Task ReceiveMessage(ChatMessageResponse message);
    public Task ReceiveError(BaseError error);
}
