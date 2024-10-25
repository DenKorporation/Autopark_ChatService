using ChatService.DAL.Models;

namespace ChatService.DAL.Repositories.Interfaces;

public interface IUserRepository
{
    public Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
    public Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
