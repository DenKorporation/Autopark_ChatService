using System.Net;
using ChatService.BLL.DTOs.Request;
using ChatService.DAL.Models;
using ChatService.IntegrationTests.DataGenerators;
using ChatService.IntegrationTests.Responses;
using ChatService.IntegrationTests.RestApis.Interfaces;
using FluentAssertions;
using MongoDB.Driver;
using Refit;

namespace ChatService.IntegrationTests.Controllers;

public class ChatControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly IChatsApi _chatsApi;
    private ICollection<User> _users = null!;
    private ICollection<Chat> _chats = null!;

    public ChatControllerTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
        _chatsApi = RestService.For<IChatsApi>(Client);
    }

    public async Task InitializeAsync()
    {
        _users = await CreateUsersAsync();
        _chats = await CreateChatsAsync(_users);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAllChats_ChatsExists_ReturnsAllChats()
    {
        // Arrange
        var user = _users.First();
        var chatForUser = _chats
            .Where(c => c.Participants.Any(id => id == user.Id));

        // Act
        var response = await _chatsApi.GetAllChatsAsync(user.Id, new PaginationRequest(1, 5));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var chatResponses = response.Content!;
        chatResponses.Should().NotBeNull();
        chatResponses.TotalCount.Should().Be(chatForUser.Count());
    }

    [Fact]
    public async Task GetChatById_ChatExist_ReturnsChat()
    {
        // Arrange
        var chat = _chats.First();

        // Act
        var response = await _chatsApi.GetChatByIdAsync(chat.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var chatResponse = response.Content;
        chatResponse.Should().NotBeNull();
        chatResponse!.Id.Should().Be(chat.Id);
    }

    [Fact]
    public async Task GetChatById_ChatNotExists_ReturnsNotFound()
    {
        // Act
        var response = await _chatsApi.GetChatByIdAsync(Guid.NewGuid());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorMessage = await response.Error!.GetContentAsAsync<ErrorMessage>();
        errorMessage.Should().NotBeNull();
        errorMessage!.Code.Should().Be("Chat.NotFound");
    }

    [Fact]
    public async Task CreateChat_ChatNotExists_ReturnsCreatedWithChat()
    {
        // Arrange
        var users = _users.Take(2).ToList();

        await EnsureChatNotExistAsync(users);

        var chatRequest = ChatDataFaker.ChatRequestFaker
            .RuleFor(
                x => x.Participants,
                _ => users.Select(u => u.Id).ToList())
            .Generate();

        // Act
        var response = await _chatsApi.CreateChatAsync(chatRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var chatResponse = response.Content;
        chatResponse.Should().NotBeNull();
        chatResponse!.Participants.Should().BeEquivalentTo(users.Select(u => u.Id));
    }

    private async Task<ICollection<User>> CreateUsersAsync()
    {
        var users = UserDataFaker.UserFaker.Generate(5);

        await TestDbContext.Users.InsertManyAsync(users);

        return users;
    }

    private async Task<ICollection<Chat>> CreateChatsAsync(ICollection<User> users)
    {
        var chats = ChatDataFaker
            .ChatFaker
            .RuleFor(
                x => x.Participants,
                f => f.PickRandom(users, 2).Select(u => u.Id).ToList())
            .Generate(3);

        await TestDbContext.Chats.InsertManyAsync(chats);

        return chats;
    }

    private async Task EnsureChatNotExistAsync(ICollection<User> users)
    {
        var chat = _chats.FirstOrDefault(
            c => c.Participants.Count == users.Count
                 && users.Select(u => u.Id).All(c.Participants.Contains));

        if (chat is not null)
        {
            var filter = Builders<Chat>
                .Filter
                .Where(c => c.Id == chat.Id);

            await TestDbContext.Chats.DeleteOneAsync(filter);
        }
    }
}
