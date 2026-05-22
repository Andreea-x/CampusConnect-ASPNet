namespace CampusConnect.ViewModels;

public class AdminRegistrationViewModel
{
    public int RegistrationId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string EventTitle { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public DateTime RegistrationDate { get; set; }
}
