using ChatService.BLL.DTOs.Response;
using ChatService.DAL.Constants;
using ChatService.DAL.Contexts;
using ChatService.DAL.Models;
using ChatService.IntegrationTests.DataGenerators;
using ChatService.IntegrationTests.JwtTokenGeneration;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.IntegrationTests.SignalRHubs;

public class ChatHubIntegrationTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>,
        IAsyncLifetime
{
    private const string BasePath = "chat-hub";

    private ICollection<User> _users = null!;
    private Chat _chat = null!;

    private CustomWebApplicationFactory Factory { get; } = factory;
    private ChatContext TestDbContext { get; } = factory.Services.GetRequiredService<ChatContext>();

    public async Task InitializeAsync()
    {
        _users = await CreateUsersAsync();
        _chat = await CreateChatsAsync(_users);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ChatHub_ValidJwtToken_ConnectsToHub()
    {
        // Arrange
        var connection = CreateConnection(_users.First());

        // Act
        await connection.StartAsync();

        // Assert
        connection.State.Should().Be(HubConnectionState.Connected);
    }

    [Fact]
    public async Task ChatHub_TwoValidConnection_SecondConnectionReceivedMessage()
    {
        // Arrange
        var messageRequest = ChatMessageDataFaker
            .ChatMessageRequestFaker
            .RuleFor(x => x.ChatId, _ => _chat.Id)
            .RuleFor(x => x.SenderId, _ => _users.First().Id)
            .Generate();

        var firstConnection = CreateConnection(_users.First());
        var secondConnection = CreateConnection(_users.Last());

        await firstConnection.StartAsync();

        await secondConnection.StartAsync();

        ChatMessageResponse? receivedMessage = null;
        secondConnection.On<ChatMessageResponse>(
            "ReceiveMessage",
            response => receivedMessage = response);

        // Act
        await firstConnection.SendAsync("SendMessage", messageRequest);

        await Task.Delay(1000);

        // Assert
        receivedMessage.Should().NotBeNull();
        receivedMessage!.Content.Should().Be(messageRequest.Content);
    }

    private HubConnection CreateConnection(User user)
    {
        return new HubConnectionBuilder()
            .WithUrl(
                $"{Factory.Server.BaseAddress}{BasePath}",
                options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(GenerateToken(user))!;
                    options.HttpMessageHandlerFactory = _ => Factory.Server.CreateHandler();
                })
            .Build();
    }

    private string GenerateToken(User user)
    {
        return new TestJwtToken()
            .WithId(user.Id)
            .WithRole(Roles.Administrator)
            .Build();
    }

    private async Task<ICollection<User>> CreateUsersAsync()
    {
        var users = UserDataFaker.UserFaker.Generate(2);

        await TestDbContext.Users.InsertManyAsync(users);

        return users;
    }

    private async Task<Chat> CreateChatsAsync(ICollection<User> users)
    {
        var chat = ChatDataFaker
            .ChatFaker
            .RuleFor(
                x => x.Participants,
                _ => users.Select(u => u.Id).ToList())
            .Generate();

        await TestDbContext.Chats.InsertOneAsync(chat);

        return chat;
    }
}
