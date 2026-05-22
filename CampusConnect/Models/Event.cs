using System.ComponentModel.DataAnnotations;
using CampusConnect.Validation;

namespace CampusConnect.Models;

public class Event
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.DateTime)]
    [FutureDate]
    public DateTime EventDate { get; set; }

    [Range(1, 5000, ErrorMessage = "Capacity must be greater than 0.")]
    public int Capacity { get; set; }

    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
