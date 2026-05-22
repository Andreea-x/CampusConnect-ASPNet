using System.Security.Claims;
using CampusConnect.Data;
using CampusConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Controllers;

public class EventController : Controller
{
    private readonly ApplicationDbContext _context;

    public EventController(ApplicationDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var events = await _context.Events
            .Include(e => e.Registrations)
            .OrderBy(e => e.EventDate)
            .ToListAsync();

        if (User.IsInRole("Student"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var registeredEventIds = await _context.Registrations
                .Where(r => r.UserId == userId)
                .Select(r => r.EventId)
                .ToListAsync();

            ViewBag.RegisteredEventIds = registeredEventIds;
        }

        return View(events);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var eventItem = await _context.Events
            .Include(e => e.Registrations)
            .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eventItem is null)
        {
            return NotFound();
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.IsRegistered = await _context.Registrations.AnyAsync(r => r.EventId == id && r.UserId == userId);
        }

        return View(eventItem);
    }

    [Authorize(Roles = "Student")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var eventItem = await _context.Events
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eventItem is null || string.IsNullOrWhiteSpace(userId))
        {
            return NotFound();
        }

        // Prevent duplicate registrations for the same student and event.
        var alreadyRegistered = await _context.Registrations
            .AnyAsync(r => r.EventId == id && r.UserId == userId);

        if (alreadyRegistered)
        {
            TempData["ErrorMessage"] = "You are already registered for this event.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // Capacity is enforced in the controller so students cannot exceed available seats.
        if (eventItem.Registrations.Count >= eventItem.Capacity)
        {
            TempData["ErrorMessage"] = "This event has reached capacity.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var registration = new Registration
        {
            EventId = id,
            UserId = userId,
            RegistrationDate = DateTime.UtcNow
        };

        _context.Registrations.Add(registration);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Registered successfully.";
        return RedirectToAction("MyRegistrations", "Registration");
    }
}
