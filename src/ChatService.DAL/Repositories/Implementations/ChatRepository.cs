using ChatService.DAL.Contexts;
using ChatService.DAL.Models;
using ChatService.DAL.Repositories.Interfaces;
using MongoDB.Driver;

namespace ChatService.DAL.Repositories.Implementations;

public class ChatRepository(ChatContext context)
    : IChatRepository
{
    public async Task<IReadOnlyCollection<Chat>> GetAllForUserAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Chat>
            .Filter
            .Where(c => c.Participants.Any(id => id == userId));

        return await context
            .Chats
            .Find(filter)
            .SortByDescending(c => c.LastModified)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> CountOfUserChatsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Chat>
            .Filter
            .Where(c => c.Participants.Any(id => id == userId));

        return await context
            .Chats
            .Find(filter)
            .CountDocumentsAsync(cancellationToken);
    }

    public async Task<Chat?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Chat>
            .Filter
            .Where(c => c.Id == id);

        return await context.Chats
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Chat?> GetByParticipantsAsync(
        IEnumerable<Guid> participantsIds,
        CancellationToken cancellationToken = default)
    {
        var participantsList = participantsIds.ToList();

        var filter = Builders<Chat>
            .Filter
            .Where(
                с =>
                    с.Participants.Count == participantsList.Count &&
                    с.Participants.All(guid => participantsList.Contains(guid)));

        return await context.Chats
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Chat> CreateAsync(Chat chat, CancellationToken cancellationToken = default)
    {
        await context
            .Chats
            .InsertOneAsync(chat, cancellationToken: cancellationToken);

        return chat;
    }

    public async Task UpdateLastModifiedAsync(
        Guid chatId,
        DateTime lastModified,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Chat>
            .Filter
            .Where(u => u.Id == chatId);

        var update = Builders<Chat>
            .Update
            .Set(c => c.LastModified, lastModified);

        await context
            .Chats
            .UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }
}
