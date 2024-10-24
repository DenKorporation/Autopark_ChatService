using ChatService.BLL.DTOs.Request;
using FluentValidation;

namespace ChatService.BLL.Validators;

public class ChatMessageRequestValidator : AbstractValidator<ChatMessageRequest>
{
    public ChatMessageRequestValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty()
            .WithMessage("Chat ID was expected");

        RuleFor(x => x.SenderId)
            .NotEmpty()
            .WithMessage("Sender ID was expected");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Message must not be empty");
    }
}
