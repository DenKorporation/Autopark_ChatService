namespace ChatService.BLL.DTOs.Response;

public record ChatResponse(
    Guid Id,
    ICollection<Guid> Participants,
    DateTime LastModified);
