using ChatService.BLL.DTOs.Request;
using ChatService.BLL.DTOs.Response;
using FluentResults;

namespace ChatService.BLL.Services.Interfaces;

public interface IUserService
{
    public Task<Result<IReadOnlyCollection<UserResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<Result<UserResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public Task<Result> CreateAsync(
        UserRequest request,
        CancellationToken cancellationToken = default);

    public Task<Result> UpdateAsync(
        UserRequest request,
        CancellationToken cancellationToken = default);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
