using System.ComponentModel.DataAnnotations;

namespace CampusConnect.Models;

public class Message
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    [Required]
    [StringLength(1000)]
    [DataType(DataType.MultilineText)]
    public string Content { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    public DateTime SentAt { get; set; }

    [StringLength(1000)]
    [DataType(DataType.MultilineText)]
    public string? AdminReply { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? RepliedAt { get; set; }
}
