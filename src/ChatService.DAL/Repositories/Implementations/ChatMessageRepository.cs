using ChatService.DAL.Contexts;
using ChatService.DAL.Models;
using ChatService.DAL.Repositories.Interfaces;
using MongoDB.Driver;

namespace ChatService.DAL.Repositories.Implementations;

public class ChatMessageRepository(ChatContext context)
    : IChatMessageRepository
{
    public async Task<IReadOnlyCollection<ChatMessage>> GetAllFromChatAsync(
        Guid chatId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<ChatMessage>
            .Filter
            .Where(cm => cm.ChatId == chatId);

        return await context
            .ChatMessages
            .Find(filter)
            .SortByDescending(m => m.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> CountOfChatMessagesAsync(Guid chatId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<ChatMessage>
            .Filter
            .Where(cm => cm.ChatId == chatId);

        return await context
            .ChatMessages
            .Find(filter)
            .CountDocumentsAsync(cancellationToken);
    }

    public async Task<ChatMessage> CreateAsync(ChatMessage message, CancellationToken cancellationToken = default)
    {
        await context
            .ChatMessages
            .InsertOneAsync(message, cancellationToken: cancellationToken);

        return message;
    }
}
