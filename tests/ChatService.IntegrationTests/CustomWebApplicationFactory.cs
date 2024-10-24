using ChatService.DAL.Contexts;
using ChatService.IntegrationTests.JwtTokenGeneration;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace ChatService.IntegrationTests;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>,
        IAsyncLifetime
{
    private const string TestingEnvironment = "Testing";

    private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder()
        .WithImage("mongo:latest")
        .WithPortBinding(27017, true)
        .WithWaitStrategy(Wait.ForUnixContainer())
        .Build();

    public async Task InitializeAsync()
    {
        await _mongoDbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _mongoDbContainer.StopAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(TestingEnvironment);
        builder.ConfigureTestServices(
            services =>
            {
                // MongoSettings
                var mongoSettingsDescriptor = services
                    .SingleOrDefault(s => s.ServiceType == typeof(MongoClientSettings));

                if (mongoSettingsDescriptor is not null)
                {
                    services.Remove(mongoSettingsDescriptor);
                }

                var chatContextDescriptor = services
                    .SingleOrDefault(s => s.ServiceType == typeof(ChatContext));

                if (chatContextDescriptor is not null)
                {
                    services.Remove(chatContextDescriptor);
                }

                services.AddSingleton(
                    MongoClientSettings.FromConnectionString(_mongoDbContainer.GetConnectionString()));

                services.AddSingleton<ChatContext>();

                // Authentication
                services.Configure<JwtBearerOptions>(
                    JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.MetadataAddress = default!;
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidIssuer = JwtTokenProvider.Issuer,
                            ValidateAudience = false,
                            IssuerSigningKey = JwtTokenProvider.SecurityKey,
                        };
                    });
            });
    }
}
