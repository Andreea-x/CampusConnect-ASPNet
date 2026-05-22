using CampusConnect.Models;

namespace CampusConnect.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalEventsCount { get; set; }
    public int TotalRegistrationsCount { get; set; }
    public int TotalMessagesCount { get; set; }
    public IEnumerable<Event> Events { get; set; } = Enumerable.Empty<Event>();
}
