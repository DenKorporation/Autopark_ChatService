using AutoMapper;
using ChatService.BLL.DTOs.Request;
using ChatService.BLL.DTOs.Response;
using ChatService.BLL.Errors.Base;
using ChatService.BLL.Errors.Users;
using ChatService.BLL.Services.Interfaces;
using ChatService.DAL.Models;
using ChatService.DAL.Repositories.Interfaces;
using FluentResults;

namespace ChatService.BLL.Services.Implementations;

public class UserService(
    IUserRepository userRepository,
    IMapper mapper)
    : IUserService
{
    public async Task<Result<IReadOnlyCollection<UserResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);

        var result = mapper.Map<IReadOnlyCollection<UserResponse>>(users);

        return result.ToResult();
    }

    public async Task<Result<UserResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return new UserNotFoundError(id);
        }

        return mapper.Map<UserResponse>(user);
    }

    public async Task<Result> CreateAsync(UserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is not null)
        {
            return new UserDuplicationError(request.Id);
        }

        var createUser = mapper.Map<User>(request);

        try
        {
            await userRepository.CreateAsync(createUser, cancellationToken);
        }
        catch (Exception)
        {
            return new InternalServerError("User.Create");
        }

        return Result.Ok();
    }

    public async Task<Result> UpdateAsync(UserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return new UserNotFoundError(request.Id);
        }

        var updateUser = mapper.Map<User>(request);

        try
        {
            await userRepository.UpdateAsync(updateUser, cancellationToken);
        }
        catch (Exception)
        {
            return new InternalServerError("User.Update");
        }

        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return new UserNotFoundError(id);
        }

        try
        {
            await userRepository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception)
        {
            return new InternalServerError("User.Delete");
        }

        return Result.Ok();
    }
}
