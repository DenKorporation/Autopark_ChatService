using Bogus;
using ChatService.BLL.DTOs.Request;
using ChatService.DAL.Constants;
using ChatService.DAL.Models;

namespace ChatService.IntegrationTests.DataGenerators;

public static class UserDataFaker
{
    public static Faker<User> UserFaker => new Faker<User>()
        .RuleFor(x => x.Id, _ => Guid.NewGuid())
        .RuleFor(x => x.Role, Roles.Administrator)
        .RuleFor(u => u.Email, (f, u) => f.Internet.ExampleEmail(u.FirstName, u.LastName))
        .RuleFor(u => u.FirstName, f => f.Name.FirstName())
        .RuleFor(u => u.LastName, f => f.Name.LastName())
        .RuleFor(u => u.Patronymic, f => f.Name.FirstName());

    public static Faker<UserRequest> UserRequestFaker => new Faker<UserRequest>()
        .CustomInstantiator(
            f => new UserRequest(
                Guid.NewGuid(),
                Roles.Administrator,
                null!,
                f.Name.FirstName(),
                f.Name.LastName(),
                f.Name.FirstName()))
        .RuleFor(u => u.Email, (f, u) => f.Internet.ExampleEmail(u.FirstName, u.LastName));
}
