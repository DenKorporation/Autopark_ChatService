using System.Net;
using ChatService.DAL.Models;
using ChatService.IntegrationTests.DataGenerators;
using ChatService.IntegrationTests.Responses;
using ChatService.IntegrationTests.RestApis.Interfaces;
using FluentAssertions;
using Refit;

namespace ChatService.IntegrationTests.Controllers;

public class UserControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly IUsersApi _usersApi;
    private ICollection<User> _users = null!;

    public UserControllerTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
        _usersApi = RestService.For<IUsersApi>(Client);
    }

    public async Task InitializeAsync()
    {
        _users = await CreateUsersAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAllUsers_UsersExists_ReturnsAllUsers()
    {
        // Act
        var response = await _usersApi.GetAllUsersAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var userResponses = response.Content!;
        userResponses.Should().NotBeNull();
        userResponses.Count.Should().Be(_users.Count);
    }

    [Fact]
    public async Task GetUserById_UserExist_ReturnsUser()
    {
        // Arrange
        var user = _users.First();

        // Act
        var response = await _usersApi.GetUserByIdAsync(user.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var userResponse = response.Content;
        userResponse.Should().NotBeNull();
        userResponse!.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetUserById_UserNotExists_ReturnsNotFound()
    {
        // Act
        var response = await _usersApi.GetUserByIdAsync(Guid.NewGuid());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorMessage = await response.Error!.GetContentAsAsync<ErrorMessage>();
        errorMessage.Should().NotBeNull();
        errorMessage!.Code.Should().Be("User.NotFound");
    }

    private async Task<ICollection<User>> CreateUsersAsync()
    {
        var users = UserDataFaker.UserFaker.Generate(5);

        await TestDbContext.Users.InsertManyAsync(users);

        return users;
    }
}
