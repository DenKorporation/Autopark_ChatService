using AutoMapper;
using ChatService.BLL.DTOs.Request;
using ChatService.BLL.DTOs.Response;
using ChatService.BLL.Errors.Base;
using ChatService.BLL.Errors.Chats;
using ChatService.BLL.Errors.Users;
using ChatService.DAL.Models;
using ChatService.DAL.Repositories.Interfaces;
using ChatService.UnitTests.DataGenerators;
using FluentAssertions;
using JetBrains.Annotations;
using Moq;

namespace ChatService.UnitTests.Services;

[TestSubject(typeof(BLL.Services.Implementations.ChatService))]
public class ChatServiceTests
{
    private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
    private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
    private readonly Mock<IChatRepository> _chatRepositoryMock = new Mock<IChatRepository>();
    private readonly BLL.Services.Implementations.ChatService _chatService;

    public ChatServiceTests()
    {
        _chatService = new BLL.Services.Implementations.ChatService(
            _userRepositoryMock.Object,
            _chatRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllForUserAsync_UserNotExists_ReturnsUserNotFoundError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new PaginationRequest(1, 5);
        _userRepositoryMock
            .Setup(
                repo =>
                    repo.GetByIdAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _chatService.GetAllForUserAsync(id, request);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotFoundError>();
        _chatRepositoryMock.Verify(
            repo =>
                repo.GetAllForUserAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAllForUserAsync_UserExists_ReturnsOkWithPaginatedList()
    {
        // Arrange
        var user = UserDataFaker.UserFaker.Generate();
        var request = new PaginationRequest(1, 5);
        var chats = ChatDataFaker
            .ChatFaker
            .Generate(5)
            .AsReadOnly();

        _userRepositoryMock
            .Setup(
                repo =>
                    repo.GetByIdAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _chatRepositoryMock
            .Setup(
                repo =>
                    repo.GetAllForUserAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(chats);

        _chatRepositoryMock
            .Setup(
                repo =>
                    repo.CountOfUserChatsAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(chats.Count);

        // Act
        var result = await _chatService.GetAllForUserAsync(user.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(chats.Count);
        _chatRepositoryMock.Verify(
            repo =>
                repo.GetAllForUserAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ChatExist_ReturnsOkWithChat()
    {
        // Arrange
        var chat = ChatDataFaker.ChatFaker.Generate();

        _chatRepositoryMock.Setup(
                repo =>
                    repo.GetByIdAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(chat);

        // Act
        var result = await _chatService.GetByIdAsync(chat.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _chatRepositoryMock.Verify(
            repo =>
                repo.GetByIdAsync(chat.Id, It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            mapper =>
                mapper.Map<ChatResponse>(It.IsAny<Chat>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ChatNotExist_ReturnsChatNotFoundError()
    {
        var id = Guid.NewGuid();

        // Arrange
        _chatRepositoryMock.Setup(
                repo =>
                    repo.GetByIdAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync((Chat?)null);

        // Act
        var result = await _chatService.GetByIdAsync(id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<ChatNotFoundError>();
        _chatRepositoryMock.Verify(
            repo =>
                repo.GetByIdAsync(id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ChatAlreadyExists_ReturnsChatDuplicationError()
    {
        // Arrange
        var chat = ChatDataFaker.ChatFaker.Generate();
        var chatRequest = ChatDataFaker
            .ChatRequestFaker
            .Generate();

        _chatRepositoryMock
            .Setup(
                x =>
                    x.GetByParticipantsAsync(
                        chatRequest.Participants,
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(chat);

        // Act
        var result = await _chatService.CreateAsync(chatRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<ChatDuplicationError>();
        _chatRepositoryMock.Verify(
            x => x.GetByParticipantsAsync(
                It.IsAny<IEnumerable<Guid>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _chatRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<Chat>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_UserNotExists_ReturnsUserNotFoundError()
    {
        // Arrange
        var chatRequest = ChatDataFaker
            .ChatRequestFaker
            .Generate();

        _userRepositoryMock
            .Setup(
                x =>
                    x.GetByIdAsync(
                        It.IsIn(chatRequest.Participants.ToArray()),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _chatService.CreateAsync(chatRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotFoundError>();
        _chatRepositoryMock.Verify(
            x => x.GetByParticipantsAsync(
                It.IsAny<IEnumerable<Guid>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _chatRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<Chat>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_SaveChangesFailure_ReturnsInternalServerError()
    {
        // Arrange
        var user = UserDataFaker.UserFaker.Generate();
        var chat = ChatDataFaker.ChatFaker.Generate();
        var chatRequest = ChatDataFaker
            .ChatRequestFaker
            .Generate();

        _userRepositoryMock
            .Setup(
                x =>
                    x.GetByIdAsync(
                        It.IsIn(chatRequest.Participants.ToArray()),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mapperMock
            .Setup(x => x.Map<Chat>(It.IsAny<ChatRequest>()))
            .Returns(chat);

        _chatRepositoryMock
            .Setup(
                x =>
                    x.CreateAsync(
                        It.IsAny<Chat>(),
                        It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _chatService.CreateAsync(chatRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _chatRepositoryMock.Verify(
            x => x.GetByParticipantsAsync(
                It.IsAny<IEnumerable<Guid>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _chatRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<Chat>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SaveChangesSuccess_ReturnsOkWithValue()
    {
        // Arrange
        var user = UserDataFaker.UserFaker.Generate();
        var chat = ChatDataFaker.ChatFaker.Generate();
        var chatRequest = ChatDataFaker
            .ChatRequestFaker
            .Generate();

        _userRepositoryMock
            .Setup(
                x =>
                    x.GetByIdAsync(
                        It.IsIn(chatRequest.Participants.ToArray()),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mapperMock
            .Setup(x => x.Map<Chat>(It.IsAny<ChatRequest>()))
            .Returns(chat);

        // Act
        var result = await _chatService.CreateAsync(chatRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _chatRepositoryMock.Verify(
            x => x.GetByParticipantsAsync(
                It.IsAny<IEnumerable<Guid>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _chatRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<Chat>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
