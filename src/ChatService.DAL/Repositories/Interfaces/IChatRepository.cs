using ChatService.DAL.Models;

namespace ChatService.DAL.Repositories.Interfaces;

public interface IChatRepository
{
    public Task<IReadOnlyCollection<Chat>> GetAllForUserAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    public Task<long> CountOfUserChatsAsync(Guid userId, CancellationToken cancellationToken = default);
    public Task<Chat?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public Task<Chat?> GetByParticipantsAsync(
        IEnumerable<Guid> participantsIds,
        CancellationToken cancellationToken = default);

    public Task<Chat> CreateAsync(Chat chat, CancellationToken cancellationToken = default);

    public Task UpdateLastModifiedAsync(
        Guid chatId,
        DateTime lastModified,
        CancellationToken cancellationToken = default);
}
