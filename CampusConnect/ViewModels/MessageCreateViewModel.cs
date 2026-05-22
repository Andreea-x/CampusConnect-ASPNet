using System.ComponentModel.DataAnnotations;

namespace CampusConnect.ViewModels;

public class MessageCreateViewModel
{
    [Required]
    [StringLength(1000)]
    [DataType(DataType.MultilineText)]
    public string Content { get; set; } = string.Empty;
}
