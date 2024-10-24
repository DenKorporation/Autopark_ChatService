using AutoMapper;
using ChatService.BLL.DTOs.Common;
using ChatService.BLL.DTOs.Request;
using ChatService.BLL.DTOs.Response;
using ChatService.BLL.Errors.Base;
using ChatService.BLL.Errors.Chats;
using ChatService.BLL.Errors.Users;
using ChatService.BLL.Services.Interfaces;
using ChatService.DAL.Models;
using ChatService.DAL.Repositories.Interfaces;
using FluentResults;

namespace ChatService.BLL.Services.Implementations;

public class ChatService(
    IUserRepository userRepository,
    IChatRepository chatRepository,
    IMapper mapper)
    : IChatService
{
    public async Task<Result<PagedList<ChatResponse>>> GetAllForUserAsync(
        Guid userId,
        PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return new UserNotFoundError(userId);
        }

        var chats = await chatRepository.GetAllForUserAsync(
            userId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var count = (int)await chatRepository.CountOfUserChatsAsync(userId, cancellationToken);

        var resultChats = mapper.Map<IReadOnlyCollection<ChatResponse>>(chats);

        return new PagedList<ChatResponse>(resultChats, request.PageNumber, request.PageSize, count);
    }

    public async Task<Result<ChatResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var chat = await chatRepository.GetByIdAsync(id, cancellationToken);

        if (chat is null)
        {
            return new ChatNotFoundError(id);
        }

        return mapper.Map<ChatResponse>(chat);
    }

    public async Task<Result<ChatResponse>> CreateAsync(
        ChatRequest request,
        CancellationToken cancellationToken = default)
    {
        var chat = await chatRepository.GetByParticipantsAsync(request.Participants, cancellationToken);

        if (chat is not null)
        {
            return new ChatDuplicationError(request.Participants);
        }

        foreach (var participantId in request.Participants)
        {
            var user = await userRepository.GetByIdAsync(participantId, cancellationToken);

            if (user is null)
            {
                return new UserNotFoundError(participantId);
            }
        }

        var createChat = mapper.Map<Chat>(request);
        createChat.LastModified = DateTime.UtcNow;

        try
        {
            await chatRepository.CreateAsync(createChat, cancellationToken);
        }
        catch (Exception)
        {
            return new InternalServerError("Chat.Create");
        }

        return mapper.Map<ChatResponse>(createChat);
    }
}
