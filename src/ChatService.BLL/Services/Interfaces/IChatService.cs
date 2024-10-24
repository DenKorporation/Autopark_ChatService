using ChatService.BLL.DTOs.Common;
using ChatService.BLL.DTOs.Request;
using ChatService.BLL.DTOs.Response;
using FluentResults;

namespace ChatService.BLL.Services.Interfaces;

public interface IChatService
{
    public Task<Result<PagedList<ChatResponse>>> GetAllForUserAsync(
        Guid userId,
        PaginationRequest request,
        CancellationToken cancellationToken = default);

    public Task<Result<ChatResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<Result<ChatResponse>> CreateAsync(ChatRequest request, CancellationToken cancellationToken = default);
}
