namespace ChatService.BLL.Errors.Base;

public class ConflictError(string code, string message)
    : BaseError(code, message)
{
}
