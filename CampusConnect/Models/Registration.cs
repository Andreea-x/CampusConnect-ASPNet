using System.ComponentModel.DataAnnotations;

namespace CampusConnect.Models;

public class Registration
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    [Required]
    public int EventId { get; set; }

    public Event? Event { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime RegistrationDate { get; set; }
}
