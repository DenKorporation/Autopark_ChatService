using System.Net;
using ChatService.BLL.DTOs.Request;
using ChatService.DAL.Models;
using ChatService.IntegrationTests.DataGenerators;
using ChatService.IntegrationTests.RestApis.Interfaces;
using FluentAssertions;
using Refit;

namespace ChatService.IntegrationTests.Controllers;

public class ChatMessageControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly IChatMessagesApi _chatMessagesApi;
    private ICollection<Chat> _chats = null!;
    private ICollection<ChatMessage> _messages = null!;

    public ChatMessageControllerTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
        _chatMessagesApi = RestService.For<IChatMessagesApi>(Client);
    }

    public async Task InitializeAsync()
    {
        _chats = await CreateChatsAsync();
        _messages = await CreateChatMessagesAsync(_chats);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAllMessages_ChatExists_ReturnsAllMessages()
    {
        // Arrange
        var chat = _chats.First();
        var messagesForChat = _messages
            .Where(m => m.ChatId == chat.Id);

        // Act
        var response = await _chatMessagesApi.GetAllChatMessagesAsync(chat.Id, new PaginationRequest(1, 5));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var messageResponses = response.Content!;
        messageResponses.Should().NotBeNull();
        messageResponses.TotalCount.Should().Be(messagesForChat.Count());
    }

    private async Task<ICollection<Chat>> CreateChatsAsync()
    {
        var chats = ChatDataFaker
            .ChatFaker
            .Generate(3);

        await TestDbContext.Chats.InsertManyAsync(chats);

        return chats;
    }

    private async Task<ICollection<ChatMessage>> CreateChatMessagesAsync(ICollection<Chat> chats)
    {
        var messages = ChatMessageDataFaker
            .ChatMessageFaker
            .RuleFor(
                x => x.ChatId,
                f => f.PickRandom(chats.Select(c => c.Id).ToList()))
            .RuleFor(
                x => x.SenderId,
                (f, m) => f.PickRandom(chats.First(c => c.Id == m.ChatId).Participants))
            .Generate(10);

        await TestDbContext.ChatMessages.InsertManyAsync(messages);

        return messages;
    }
}
