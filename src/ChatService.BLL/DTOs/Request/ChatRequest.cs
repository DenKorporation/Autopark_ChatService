namespace ChatService.BLL.DTOs.Request;

public record ChatRequest(
    ICollection<Guid> Participants);
