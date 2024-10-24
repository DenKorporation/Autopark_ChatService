using ChatService.BLL.Errors.Base;

namespace ChatService.BLL.Errors.Users;

public class UserDuplicationError(
    string code = "User.Duplication",
    string message = "User already exist")
    : ConflictError(code, message)
{
    public UserDuplicationError(Guid id)
        : this(message: $"User '{id}' already exist")
    {
    }
}
