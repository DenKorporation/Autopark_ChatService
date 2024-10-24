namespace ChatService.BLL.DTOs.Request;

public record UserRequest(
    Guid Id,
    string Role,
    string Email,
    string FirstName,
    string LastName,
    string Patronymic);
