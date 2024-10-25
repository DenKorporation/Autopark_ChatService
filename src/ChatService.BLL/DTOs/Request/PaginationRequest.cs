namespace ChatService.BLL.DTOs.Request;

public record PaginationRequest(
    int PageNumber,
    int PageSize);
