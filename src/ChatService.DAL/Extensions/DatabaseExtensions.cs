using ChatService.DAL.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChatService.DAL.Extensions;

public static class DatabaseExtensions
{
    public static async Task<IHost> InitializeDatabaseAsync(
        this IHost app,
        CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<ChatContextInitializer>();

        await initializer.SeedAsync(cancellationToken);

        return app;
    }
}
