namespace ChatService.BLL.Errors.Base;

public class InternalServerError(string code, string message = "Something went wrong")
    : BaseError(code, message)
{
}
