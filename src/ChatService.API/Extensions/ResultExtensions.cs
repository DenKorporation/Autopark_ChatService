using ChatService.BLL.Errors;
using ChatService.BLL.Errors.Base;
using FluentResults;

namespace ChatService.API.Extensions;

public static class ResultExtensions
{
    public static IResult ToAspResult<TValue>(this Result<TValue> result, Func<TValue, IResult> success)
    {
        return result switch
        {
            { IsFailed: true } => result.ToProblemDetail(),
            { IsSuccess: true } => success(result.Value),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static IResult ToAspResult(this Result result, IResult success)
    {
        return result switch
        {
            { IsFailed: true } => result.ToProblemDetail(),
            { IsSuccess: true } => success,
            _ => throw new ArgumentOutOfRangeException(nameof(result)),
        };
    }

    public static IResult ToAspResult(this Result result, Func<IResult> success)
    {
        return result switch
        {
            { IsFailed: true } => result.ToProblemDetail(),
            { IsSuccess: true } => success(),
            _ => throw new ArgumentOutOfRangeException(nameof(result)),
        };
    }

    public static IResult ToProblemDetail(this ResultBase result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Can't convert success result to problem");
        }

        var error = result.Errors.FirstOrDefault();
        Dictionary<string, object?> extensions = new Dictionary<string, object?> { { "message", error!.Message } };

        if (error is BaseError baseError)
        {
            extensions.Add("code", baseError.Code);
        }

        if (error is ValidationError validationError)
        {
            extensions.Add("errors", validationError.ValidationErrors);
        }

        return Results.Problem(
            statusCode: GetStatusCode(error),
            title: GetTitle(error),
            extensions: extensions);
    }

    private static int GetStatusCode(IError error) => error switch
    {
        BadRequestError => StatusCodes.Status400BadRequest,
        NotFoundError => StatusCodes.Status404NotFound,
        ConflictError => StatusCodes.Status409Conflict,
        InternalServerError => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string GetTitle(IError error) => error switch
    {
        BadRequestError => "Bad Request",
        NotFoundError => "Not Found",
        ConflictError => "Conflict",
        InternalServerError => "Internal Server Error",
        _ => "Internal server error"
    };
}
