using AutoMapper;
using ChatService.BLL.DTOs.Request;
using ChatService.BLL.Errors.Base;
using ChatService.BLL.Errors.Chats;
using ChatService.BLL.Errors.Users;
using ChatService.BLL.Services.Implementations;
using ChatService.DAL.Models;
using ChatService.DAL.Repositories.Interfaces;
using ChatService.UnitTests.DataGenerators;
using FluentAssertions;
using JetBrains.Annotations;
using Moq;

namespace ChatService.UnitTests.Services;

[TestSubject(typeof(ChatMessageService))]
public class ChatMessageServiceTests
{
    private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
    private readonly Mock<IChatMessageRepository> _chatMessageRepositoryMock = new Mock<IChatMessageRepository>();
    private readonly Mock<IChatRepository> _chatRepositoryMock = new Mock<IChatRepository>();
    private readonly ChatMessageService _chatMessageService;

    public ChatMessageServiceTests()
    {
        _chatMessageService = new ChatMessageService(
            _chatMessageRepositoryMock.Object,
            _chatRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllFromChatAsync_ChatNotExists_ReturnsChatNotFoundError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new PaginationRequest(1, 5);
        _chatRepositoryMock
            .Setup(
                repo =>
                    repo.GetByIdAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync((Chat?)null);

        // Act
        var result = await _chatMessageService.GetAllFromChatAsync(id, request);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<ChatNotFoundError>();
        _chatMessageRepositoryMock.Verify(
            repo =>
                repo.GetAllFromChatAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAllFromChatAsync_ChatExists_ReturnsOkWithPaginatedList()
    {
        // Arrange
        var chat = ChatDataFaker.ChatFaker.Generate();
        var request = new PaginationRequest(1, 5);
        var messages = ChatMessageDataFaker
            .ChatMessageFaker
            .Generate(5)
            .AsReadOnly();

        _chatRepositoryMock
            .Setup(
                repo =>
                    repo.GetByIdAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(chat);

        _chatMessageRepositoryMock
            .Setup(
                repo =>
                    repo.GetAllFromChatAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages);

        _chatMessageRepositoryMock
            .Setup(
                repo =>
                    repo.CountOfChatMessagesAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages.Count);

        // Act
        var result = await _chatMessageService.GetAllFromChatAsync(chat.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(messages.Count);
        _chatMessageRepositoryMock.Verify(
            repo =>
                repo.GetAllFromChatAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ChatNotExists_ReturnsChatNotFoundError()
    {
        // Arrange
        var messageRequest = ChatMessageDataFaker
            .ChatMessageRequestFaker
            .Generate();

        _chatRepositoryMock
            .Setup(
                x =>
                    x.GetByIdAsync(
                        messageRequest.ChatId,
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync((Chat?)null);

        // Act
        var result = await _chatMessageService.CreateAsync(messageRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<ChatNotFoundError>();

        _chatMessageRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<ChatMessage>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_UserNotChatMember_ReturnsUserNotChatMemberError()
    {
        // Arrange
        var chat = ChatDataFaker.ChatFaker.Generate();
        var messageRequest = ChatMessageDataFaker
            .ChatMessageRequestFaker
            .Generate();

        _chatRepositoryMock
            .Setup(
                x =>
                    x.GetByIdAsync(
                        messageRequest.ChatId,
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(chat);

        // Act
        var result = await _chatMessageService.CreateAsync(messageRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotChatMemberError>();

        _chatMessageRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<ChatMessage>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_SaveChangesFailure_ReturnsInternalServerError()
    {
        // Arrange
        var chat = ChatDataFaker.ChatFaker.Generate();
        var mappedChat = ChatMessageDataFaker.ChatMessageFaker.Generate();
        var messageRequest = ChatMessageDataFaker
            .ChatMessageRequestFaker
            .RuleFor(x => x.ChatId, chat.Id)
            .RuleFor(x => x.SenderId, chat.Participants.First)
            .Generate();

        _chatRepositoryMock
            .Setup(
                x =>
                    x.GetByIdAsync(
                        messageRequest.ChatId,
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(chat);

        _mapperMock
            .Setup(x => x.Map<ChatMessage>(It.IsAny<ChatMessageRequest>()))
            .Returns(mappedChat);

        _chatMessageRepositoryMock
            .Setup(
                x =>
                    x.CreateAsync(
                        It.IsAny<ChatMessage>(),
                        It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _chatMessageService.CreateAsync(messageRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _chatMessageRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<ChatMessage>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SaveChangesSuccess_ReturnsOkWithValue()
    {
        // Arrange
        var chat = ChatDataFaker.ChatFaker.Generate();
        var mappedChat = ChatMessageDataFaker.ChatMessageFaker.Generate();
        var messageRequest = ChatMessageDataFaker
            .ChatMessageRequestFaker
            .RuleFor(x => x.ChatId, chat.Id)
            .RuleFor(x => x.SenderId, chat.Participants.First)
            .Generate();

        _chatRepositoryMock
            .Setup(
                x =>
                    x.GetByIdAsync(
                        messageRequest.ChatId,
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(chat);

        _mapperMock
            .Setup(x => x.Map<ChatMessage>(It.IsAny<ChatMessageRequest>()))
            .Returns(mappedChat);

        // Act
        var result = await _chatMessageService.CreateAsync(messageRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _chatMessageRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<ChatMessage>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _chatRepositoryMock.Verify(
            x => x.UpdateLastModifiedAsync(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
