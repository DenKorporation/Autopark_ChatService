using ChatService.DAL.Contexts;
using ChatService.DAL.Models;
using ChatService.DAL.Repositories.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ChatService.DAL.Repositories.Implementations;

public class UserRepository(ChatContext context)
    : IUserRepository
{
    public async Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context
            .Users
            .Find(_ => true)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>
            .Filter
            .Where(u => u.Id == id);

        return await context
            .Users
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        await context
            .Users
            .InsertOneAsync(user, cancellationToken: cancellationToken);

        return user;
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>
            .Filter
            .Where(u => u.Id == user.Id);

        await context
            .Users
            .ReplaceOneAsync(filter, user, cancellationToken: cancellationToken);

        return user;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>
            .Filter
            .Where(u => u.Id == id);

        await context
            .Users
            .DeleteOneAsync(filter, cancellationToken);
    }
}
