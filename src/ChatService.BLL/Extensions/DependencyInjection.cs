using System.Reflection;
using ChatService.BLL.Services.Implementations;
using ChatService.BLL.Services.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.BLL.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddBllServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.ConfigureMapping();
        services.ConfigureValidation();
        services.ConfigureServices();

        return services;
    }

    private static IServiceCollection ConfigureMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }

    private static IServiceCollection ConfigureValidation(this IServiceCollection services)
    {
        services
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services
            .AddFluentValidationAutoValidation();

        return services;
    }

    private static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IChatService, Services.Implementations.ChatService>();
        services.AddScoped<IChatMessageService, ChatMessageService>();

        return services;
    }
}
