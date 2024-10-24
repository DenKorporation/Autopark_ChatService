using Bogus;
using ChatService.BLL.DTOs.Request;
using ChatService.DAL.Models;

namespace ChatService.IntegrationTests.DataGenerators;

public static class ChatMessageDataFaker
{
    public static Faker<ChatMessage> ChatMessageFaker => new Faker<ChatMessage>()
        .RuleFor(m => m.Id, _ => Guid.NewGuid())
        .RuleFor(m => m.ChatId, _ => Guid.NewGuid())
        .RuleFor(m => m.SenderId, _ => Guid.NewGuid())
        .RuleFor(m => m.Content, f => f.Lorem.Sentence())
        .RuleFor(m => m.Timestamp, f => f.Date.Recent(5));

    public static Faker<ChatMessageRequest> ChatMessageRequestFaker => new Faker<ChatMessageRequest>()
        .CustomInstantiator(
            f => new ChatMessageRequest(
                Guid.NewGuid(),
                Guid.NewGuid(),
                f.Lorem.Sentence()));
}
