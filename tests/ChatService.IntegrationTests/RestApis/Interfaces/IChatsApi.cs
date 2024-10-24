using ChatService.BLL.DTOs.Request;
using ChatService.BLL.DTOs.Response;
using ChatService.IntegrationTests.Responses;
using Refit;

namespace ChatService.IntegrationTests.RestApis.Interfaces;

public interface IChatsApi
{
    [Get("/chats")]
    Task<ApiResponse<PagedList<ChatResponse>>> GetAllChatsAsync(
        [Query] Guid userId,
        [Query] PaginationRequest request,
        CancellationToken cancellationToken = default);

    [Get("/chats/{chatId}")]
    Task<ApiResponse<ChatResponse>> GetChatByIdAsync(
        Guid chatId,
        CancellationToken cancellationToken = default);

    [Post("/chats")]
    Task<ApiResponse<ChatResponse>> CreateChatAsync(
        [Body] ChatRequest createChatRequest,
        CancellationToken cancellationToken = default);
}
