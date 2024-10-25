using ChatService.DAL.Constants;
using ChatService.DAL.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ChatService.DAL.Contexts;

public class ChatContextInitializer(
    ILogger<ChatContextInitializer> logger,
    ChatContext dbContext)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await TrySeedAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");

            throw;
        }
    }

    private static List<User> GetPreconfiguredUsers()
    {
        return
        [
            new User
            {
                Id = new Guid("1A24A4F4-E9CB-437F-9369-ED37448CA4C4"),
                FirstName = "Ivanov",
                LastName = "Ivanov",
                Patronymic = "Ivanovich",
                Email = "admin@example.com",
                Role = Roles.Administrator,
            },
            new User
            {
                Id = new Guid("BE1E9E60-E11B-4C44-B4EE-54D511740523"),
                FirstName = "TestFirstName",
                LastName = "TestLastName",
                Patronymic = "TestPatronymic",
                Email = "test@example.com",
                Role = Roles.Administrator,
            },
        ];
    }

    private static List<Chat> GetPreconfiguredChats()
    {
        return
        [
            new Chat
            {
                Id = new Guid("8AA2083D-471A-409F-AB82-057C29378A87"),
                Participants =
                [
                    new Guid("2E6E4981-3349-4EFC-92BD-2DB1EAF9776A"),
                    new Guid("BE1E9E60-E11B-4C44-B4EE-54D511740523"),
                ],
                LastModified = DateTime.UtcNow,
            },
        ];
    }

    private static List<ChatMessage> GetPreconfiguredChatMessages()
    {
        return
        [
            new ChatMessage
            {
                Id = new Guid("2A6D38F0-99B1-4A0D-B75F-C88957C16EF2"),
                ChatId = new Guid("8AA2083D-471A-409F-AB82-057C29378A87"),
                Content = "This is a message",
                SenderId = new Guid("2E6E4981-3349-4EFC-92BD-2DB1EAF9776A"),
                Timestamp = DateTime.UtcNow,
            },
            new ChatMessage
            {
                Id = new Guid("C51BAC2E-CD38-44AC-8994-F9634239A83C"),
                ChatId = new Guid("8AA2083D-471A-409F-AB82-057C29378A87"),
                Content = "This also is a message",
                SenderId = new Guid("BE1E9E60-E11B-4C44-B4EE-54D511740523"),
                Timestamp = DateTime.UtcNow,
            },
        ];
    }

    private async Task TrySeedAsync(CancellationToken cancellationToken = default)
    {
        if (await dbContext.Users.CountDocumentsAsync(
                FilterDefinition<User>.Empty,
                cancellationToken: cancellationToken) == 0)
        {
            await dbContext
                .Users
                .InsertManyAsync(GetPreconfiguredUsers(), cancellationToken: cancellationToken);
        }

        if (await dbContext.Chats.CountDocumentsAsync(
                FilterDefinition<Chat>.Empty,
                cancellationToken: cancellationToken) == 0)
        {
            await dbContext
                .Chats
                .InsertManyAsync(GetPreconfiguredChats(), cancellationToken: cancellationToken);
        }

        if (await dbContext.ChatMessages.CountDocumentsAsync(
                FilterDefinition<ChatMessage>.Empty,
                cancellationToken: cancellationToken) == 0)
        {
            await dbContext
                .ChatMessages
                .InsertManyAsync(GetPreconfiguredChatMessages(), cancellationToken: cancellationToken);
        }
    }
}
