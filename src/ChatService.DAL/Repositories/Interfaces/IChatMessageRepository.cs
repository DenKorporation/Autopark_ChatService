using ChatService.DAL.Models;

namespace ChatService.DAL.Repositories.Interfaces;

public interface IChatMessageRepository
{
    public Task<IReadOnlyCollection<ChatMessage>> GetAllFromChatAsync(
        Guid chatId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    public Task<long> CountOfChatMessagesAsync(Guid chatId, CancellationToken cancellationToken = default);
    public Task<ChatMessage> CreateAsync(ChatMessage message, CancellationToken cancellationToken = default);
}
