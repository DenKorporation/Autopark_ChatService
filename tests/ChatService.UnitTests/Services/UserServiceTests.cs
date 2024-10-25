using AutoMapper;
using ChatService.BLL.DTOs.Request;
using ChatService.BLL.DTOs.Response;
using ChatService.BLL.Errors.Base;
using ChatService.BLL.Errors.Users;
using ChatService.BLL.Services.Implementations;
using ChatService.DAL.Models;
using ChatService.DAL.Repositories.Interfaces;
using ChatService.UnitTests.DataGenerators;
using FluentAssertions;
using JetBrains.Annotations;
using Moq;

namespace ChatService.UnitTests.Services;

[TestSubject(typeof(UserService))]
public class UserServiceTests
{
    private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
    private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userService = new UserService(_userRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_UsersExist_ReturnsAllUsers()
    {
        // Arrange
        var userData = UserDataFaker
            .UserFaker
            .Generate(1)
            .AsReadOnly();

        _userRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(userData);

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();

        _userRepositoryMock.Verify(
            repo =>
                repo.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            mapper =>
                mapper.Map<IReadOnlyCollection<UserResponse>>(It.IsAny<IReadOnlyCollection<User>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_UserExist_ReturnsOkWithUser()
    {
        // Arrange
        var user = UserDataFaker.UserFaker.Generate();

        _userRepositoryMock.Setup(
                repo =>
                    repo.GetByIdAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdAsync(user.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _userRepositoryMock.Verify(
            repo =>
                repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            mapper =>
                mapper.Map<UserResponse>(It.IsAny<User>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_UserNotExist_ReturnsUserNotFoundError()
    {
        var id = Guid.NewGuid();

        // Arrange
        _userRepositoryMock.Setup(
                repo =>
                    repo.GetByIdAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetByIdAsync(id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotFoundError>();
        _userRepositoryMock.Verify(
            repo =>
                repo.GetByIdAsync(id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_UserAlreadyExists_ReturnsUserDuplicationError()
    {
        // Arrange
        var user = UserDataFaker.UserFaker.Generate();
        var userRequest = UserDataFaker
            .UserRequestFaker
            .Generate();

        _userRepositoryMock
            .Setup(
                x =>
                    x.GetByIdAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.CreateAsync(userRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserDuplicationError>();
        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _userRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_SaveChangesFailure_ReturnsInternalServerError()
    {
        // Arrange
        var user = UserDataFaker.UserFaker.Generate();
        var userRequest = UserDataFaker
            .UserRequestFaker
            .Generate();

        _mapperMock
            .Setup(x => x.Map<User>(It.IsAny<UserRequest>()))
            .Returns(user);

        _userRepositoryMock
            .Setup(
                x =>
                    x.CreateAsync(
                        It.IsAny<User>(),
                        It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _userService.CreateAsync(userRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _userRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SaveChangesSuccess_ReturnsOkWithValue()
    {
        // Arrange
        var user = UserDataFaker.UserFaker.Generate();
        var userRequest = UserDataFaker
            .UserRequestFaker
            .Generate();

        _mapperMock
            .Setup(x => x.Map<User>(It.IsAny<UserRequest>()))
            .Returns(user);

        // Act
        var result = await _userService.CreateAsync(userRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _userRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UserNotExists_ReturnsUserNotFoundError()
    {
        // Arrange
        var userRequest = UserDataFaker
            .UserRequestFaker
            .Generate();

        _userRepositoryMock
            .Setup(
                x =>
                    x.GetByIdAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.UpdateAsync(userRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotFoundError>();
        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _userRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_SaveChangesFailure_ReturnsInternalServerError()
    {
        // Arrange
        var user = UserDataFaker.UserFaker.Generate();
        var mappedUser = UserDataFaker.UserFaker.Generate();
        var userRequest = UserDataFaker
            .UserRequestFaker
            .Generate();

        _userRepositoryMock
            .Setup(
                x =>
                    x.GetByIdAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mapperMock
            .Setup(x => x.Map<User>(It.IsAny<UserRequest>()))
            .Returns(mappedUser);

        _userRepositoryMock
            .Setup(
                x =>
                    x.UpdateAsync(
                        It.IsAny<User>(),
                        It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _userService.UpdateAsync(userRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _userRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SaveChangesSuccess_ReturnsOkWithValue()
    {
        // Arrange
        var user = UserDataFaker.UserFaker.Generate();
        var mappedUser = UserDataFaker.UserFaker.Generate();
        var userRequest = UserDataFaker
            .UserRequestFaker
            .Generate();

        _userRepositoryMock
            .Setup(
                x =>
                    x.GetByIdAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mapperMock
            .Setup(x => x.Map<User>(It.IsAny<UserRequest>()))
            .Returns(mappedUser);

        // Act
        var result = await _userService.UpdateAsync(userRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _userRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_UserNotExists_ReturnsUserNotFoundError()
    {
        // Arrange
        var id = Guid.NewGuid();

        _userRepositoryMock.Setup(
                x =>
                    x.GetByIdAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.DeleteAsync(id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotFoundError>();
        _userRepositoryMock.Verify(
            x =>
                x.GetByIdAsync(
                    id,
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_SaveChangesFailure_ReturnsInternalServerError()
    {
        // Arrange
        var user = UserDataFaker.UserFaker.Generate();

        _userRepositoryMock
            .Setup(
                x =>
                    x.GetByIdAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(
                x =>
                    x.DeleteAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _userService.DeleteAsync(user.Id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _userRepositoryMock.Verify(
            x =>
                x.DeleteAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_SaveChangesSuccess_ReturnsOk()
    {
        // Arrange
        var user = UserDataFaker.UserFaker.Generate();

        _userRepositoryMock
            .Setup(
                x =>
                    x.GetByIdAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.DeleteAsync(user.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _userRepositoryMock.Verify(
            x =>
                x.DeleteAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
