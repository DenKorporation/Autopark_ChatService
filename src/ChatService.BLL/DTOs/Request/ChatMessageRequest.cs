namespace ChatService.BLL.DTOs.Request;

public record ChatMessageRequest(
    Guid ChatId,
    Guid SenderId,
    string Content);
