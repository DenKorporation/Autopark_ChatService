namespace ChatService.DAL.Models;

public class User
{
    public Guid Id { get; set; }
    public string Role { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Patronymic { get; set; } = null!;
}
