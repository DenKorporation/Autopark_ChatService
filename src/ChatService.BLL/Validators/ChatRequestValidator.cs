using ChatService.BLL.DTOs.Request;
using FluentValidation;

namespace ChatService.BLL.Validators;

public class ChatRequestValidator : AbstractValidator<ChatRequest>
{
    public ChatRequestValidator()
    {
        RuleFor(x => x.Participants)
            .NotEmpty()
            .WithMessage("Chat participants was expected")
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("All chat participants must be different")
            .Must(x => x.Count == 2)
            .WithMessage("Chat must have 2 participants");
    }
}
