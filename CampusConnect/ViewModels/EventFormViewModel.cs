using System.ComponentModel.DataAnnotations;
using CampusConnect.Validation;

namespace CampusConnect.ViewModels;

public class EventFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    [Display(Name = "Event date")]
    [FutureDate]
    public DateTime EventDate { get; set; } = DateTime.UtcNow.AddDays(1);

    [Required]
    [Range(1, 5000, ErrorMessage = "Capacity must be greater than 0.")]
    public int Capacity { get; set; }
}
