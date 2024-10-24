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

public class ChatMessageService(
    IChatMessageRepository chatMessageRepository,
    IChatRepository chatRepository,
    IMapper mapper)
    : IChatMessageService
{
    public async Task<Result<PagedList<ChatMessageResponse>>> GetAllFromChatAsync(
        Guid chatId,
        PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        var chat = await chatRepository.GetByIdAsync(chatId, cancellationToken);

        if (chat is null)
        {
            return new ChatNotFoundError(chatId);
        }

        var messages = await chatMessageRepository.GetAllFromChatAsync(
            chatId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var count = (int)await chatMessageRepository.CountOfChatMessagesAsync(chatId, cancellationToken);

        var resultMessages = mapper.Map<IReadOnlyCollection<ChatMessageResponse>>(messages);

        return new PagedList<ChatMessageResponse>(resultMessages, request.PageNumber, request.PageSize, count);
    }

    public async Task<Result<ChatMessageResponse>> CreateAsync(
        ChatMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        var chat = await chatRepository.GetByIdAsync(request.ChatId, cancellationToken);

        if (chat is null)
        {
            return new ChatNotFoundError(request.ChatId);
        }

        if (chat.Participants.All(id => id != request.SenderId))
        {
            return new UserNotChatMemberError(request.SenderId);
        }

        var createMessage = mapper.Map<ChatMessage>(request);
        createMessage.Timestamp = DateTime.UtcNow;

        try
        {
            await chatMessageRepository.CreateAsync(createMessage, cancellationToken);

            await chatRepository.UpdateLastModifiedAsync(request.ChatId, createMessage.Timestamp, cancellationToken);
        }
        catch (Exception)
        {
            return new InternalServerError("ChatMessage.Create");
        }

        return mapper.Map<ChatMessageResponse>(createMessage);
    }
}
