using ChatService.BLL.Errors.Base;

namespace ChatService.BLL.Errors.Chats;

public class ChatDuplicationError(string code = "Chat.Duplicate", string message = "Chat already exists.")
    : ConflictError(code, message)
{
    public ChatDuplicationError(IEnumerable<Guid> participantIds)
        : this(message: $"Chat with '{string.Join(", ", participantIds)}' participants already exists")
    {
    }
}
