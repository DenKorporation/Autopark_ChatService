namespace ChatService.BLL.DTOs.Response;

public record ChatMessageResponse(
    Guid Id,
    Guid ChatId,
    Guid SenderId,
    string Content,
    DateTime Timestamp);
