using System.ComponentModel.DataAnnotations;

namespace CampusConnect.ViewModels;

public class AdminReplyMessageViewModel
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public string? ExistingReply { get; set; }

    [Required]
    [StringLength(1000)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Reply")]
    public string ReplyContent { get; set; } = string.Empty;
}
