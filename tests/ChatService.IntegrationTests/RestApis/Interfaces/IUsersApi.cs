using ChatService.BLL.DTOs.Response;
using Refit;

namespace ChatService.IntegrationTests.RestApis.Interfaces;

public interface IUsersApi
{
    [Get("/users")]
    Task<ApiResponse<ICollection<UserResponse>>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    [Get("/users/{userId}")]
    Task<ApiResponse<UserResponse>> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
