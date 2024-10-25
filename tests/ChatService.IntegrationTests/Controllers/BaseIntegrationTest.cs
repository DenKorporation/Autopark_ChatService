using System.Net.Http.Headers;
using ChatService.DAL.Constants;
using ChatService.DAL.Contexts;
using ChatService.IntegrationTests.JwtTokenGeneration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.IntegrationTests.Controllers;

public abstract class BaseIntegrationTest
    : IClassFixture<CustomWebApplicationFactory>,
        IDisposable
{
    private const string BasePath = "api/v1.0";
    private static string? _authenticationToken;

    protected BaseIntegrationTest(CustomWebApplicationFactory factory)
    {
        TestDbContext = factory.Services.GetRequiredService<ChatContext>();
        Client = factory.CreateClient();

        if (_authenticationToken is null)
        {
            _authenticationToken = AdminJwtToken;
        }

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationToken);

        if (Client.BaseAddress is not null)
        {
            Client.BaseAddress = new Uri(Client.BaseAddress.AbsoluteUri + BasePath);
        }
    }

    protected ChatContext TestDbContext { get; }
    protected HttpClient Client { get; }

    private static string AdminJwtToken =>
        new TestJwtToken().WithRole(Roles.Administrator).Build();

    public void Dispose()
    {
        Client.Dispose();
    }
}
