using ChatService.BLL.DTOs.Common;
using ChatService.BLL.DTOs.Request;
using ChatService.BLL.DTOs.Response;
using FluentResults;

namespace ChatService.BLL.Services.Interfaces;

public interface IChatMessageService
{
    public Task<Result<PagedList<ChatMessageResponse>>> GetAllFromChatAsync(
        Guid chatId,
        PaginationRequest request,
        CancellationToken cancellationToken = default);

    public Task<Result<ChatMessageResponse>> CreateAsync(
        ChatMessageRequest request,
        CancellationToken cancellationToken = default);
}
