using ChatService.BLL.Errors.Base;

namespace ChatService.BLL.Errors.Users;

public class UserNotChatMemberError(
    string code = "User.Conflict",
    string message = "User not chat member")
    : ConflictError(code, message)
{
    public UserNotChatMemberError(Guid id)
        : this(message: $"User '{id}' not chat member")
    {
    }
}
