using Bogus;
using ChatService.BLL.DTOs.Request;
using ChatService.DAL.Models;

namespace ChatService.IntegrationTests.DataGenerators;

public static class ChatDataFaker
{
    public static Faker<Chat> ChatFaker => new Faker<Chat>()
        .RuleFor(x => x.Id, _ => Guid.NewGuid())
        .RuleFor(
            x => x.Participants,
            _ => new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
            })
        .RuleFor(c => c.LastModified, f => f.Date.Recent(10));

    public static Faker<ChatRequest> ChatRequestFaker => new Faker<ChatRequest>()
        .CustomInstantiator(
            _ => new ChatRequest(
                new List<Guid>
                {
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                }));
}
