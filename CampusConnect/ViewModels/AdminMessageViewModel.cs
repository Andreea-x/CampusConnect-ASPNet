namespace CampusConnect.ViewModels;

public class AdminMessageViewModel
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public string? AdminReply { get; set; }
    public DateTime? RepliedAt { get; set; }
}
