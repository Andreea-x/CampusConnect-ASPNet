using CampusConnect.Models;

namespace CampusConnect.ViewModels;

public class DashboardViewModel
{
    public string? FullName { get; set; }
    public bool IsAdmin { get; set; }
    public int TotalEventsCount { get; set; }
    public int UpcomingEventsCount { get; set; }
    public int RegistrationCount { get; set; }
    public int MessageCount { get; set; }
    public IEnumerable<Event> UpcomingEvents { get; set; } = Enumerable.Empty<Event>();
    public IEnumerable<Announcement> LatestAnnouncements { get; set; } = Enumerable.Empty<Announcement>();
}
