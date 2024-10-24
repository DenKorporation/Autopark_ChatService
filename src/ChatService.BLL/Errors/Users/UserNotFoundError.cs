using ChatService.BLL.Errors.Base;

namespace ChatService.BLL.Errors.Users;

public class UserNotFoundError(string code = "User.NotFound", string message = "User not found")
    : NotFoundError(code, message)
{
    public UserNotFoundError(Guid userId)
        : this(message: $"User '{userId}' not found")
    {
    }
}
