using ChatService.BLL.DTOs.Request;
using ChatService.BLL.DTOs.Response;
using ChatService.IntegrationTests.Responses;
using Refit;

namespace ChatService.IntegrationTests.RestApis.Interfaces;

public interface IChatMessagesApi
{
    [Get("/messages")]
    Task<ApiResponse<PagedList<ChatMessageResponse>>> GetAllChatMessagesAsync(
        [Query] Guid chatId,
        [Query] PaginationRequest request,
        CancellationToken cancellationToken = default);
}
