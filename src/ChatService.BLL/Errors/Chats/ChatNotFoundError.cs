using ChatService.BLL.Errors.Base;

namespace ChatService.BLL.Errors.Chats;

public class ChatNotFoundError(string code = "Chat.NotFound", string message = "Chat not found")
    : NotFoundError(code, message)
{
    public ChatNotFoundError(Guid chatId)
        : this(message: $"Chat '{chatId}' not found")
    {
    }
}
