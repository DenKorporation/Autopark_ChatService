using ChatService.DAL.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace ChatService.DAL.Contexts;

public class ChatContext : IDisposable
{
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;

    public ChatContext(MongoClientSettings settings)
    {
        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        _client = new MongoClient(settings);
        _database = _client.GetDatabase("chat");
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    public IMongoCollection<ChatMessage> ChatMessages => _database.GetCollection<ChatMessage>("ChatMessages");
    public IMongoCollection<Chat> Chats => _database.GetCollection<Chat>("Chats");

    public void Dispose()
    {
        _client.Dispose();
    }
}
