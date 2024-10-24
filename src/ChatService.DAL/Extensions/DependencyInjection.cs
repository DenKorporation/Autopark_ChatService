using ChatService.DAL.Contexts;
using ChatService.DAL.Repositories.Implementations;
using ChatService.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace ChatService.DAL.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddDalServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.ConfigureDbContext(configuration);
        services.ConfigureRepositories();

        return services;
    }

    private static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(MongoClientSettings.FromConnectionString(configuration.GetConnectionString("Database")));

        services.AddSingleton<ChatContext>();
        services.AddScoped<ChatContextInitializer>();
    }

    private static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}
