namespace ChatService.DAL.Models;

public class Chat
{
    public Guid Id { get; set; }
    public ICollection<Guid> Participants { get; set; } = null!;
    public DateTime LastModified { get; set; }
}
