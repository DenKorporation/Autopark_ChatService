namespace ChatService.BLL.DTOs.Response;

public record UserResponse(
    Guid Id,
    string Role,
    string Email,
    string FirstName,
    string LastName,
    string Patronymic);
