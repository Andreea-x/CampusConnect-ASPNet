using System.Diagnostics;
using CampusConnect.Data;
using CampusConnect.Models;
using CampusConnect.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(
        ApplicationDbContext context,
        ILogger<HomeController> logger,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var latestAnnouncements = await _context.Announcements
            .OrderByDescending(a => a.CreatedAt)
            .Take(3)
            .ToListAsync();

        if (User.Identity?.IsAuthenticated != true)
        {
            var publicDashboard = new DashboardViewModel
            {
                TotalEventsCount = await _context.Events.CountAsync(),
                UpcomingEventsCount = await _context.Events.CountAsync(e => e.EventDate >= DateTime.UtcNow),
                UpcomingEvents = await _context.Events
                    .Where(e => e.EventDate >= DateTime.UtcNow)
                    .OrderBy(e => e.EventDate)
                    .Take(3)
                    .ToListAsync(),
                LatestAnnouncements = latestAnnouncements
            };

            return View(publicDashboard);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        var model = new DashboardViewModel
        {
            FullName = user.FullName,
            IsAdmin = isAdmin,
            TotalEventsCount = await _context.Events.CountAsync(),
            UpcomingEventsCount = await _context.Events.CountAsync(e => e.EventDate >= DateTime.UtcNow),
            RegistrationCount = isAdmin
                ? await _context.Registrations.CountAsync()
                : await _context.Registrations.CountAsync(r => r.UserId == user.Id),
            MessageCount = isAdmin
                ? await _context.Messages.CountAsync()
                : await _context.Messages.CountAsync(m => m.UserId == user.Id),
            UpcomingEvents = await _context.Events
                .Where(e => e.EventDate >= DateTime.UtcNow)
                .OrderBy(e => e.EventDate)
                .Take(4)
                .ToListAsync(),
            LatestAnnouncements = latestAnnouncements
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
