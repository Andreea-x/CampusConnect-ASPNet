using System.Security.Claims;
using CampusConnect.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Controllers;

[Authorize(Roles = "Student")]
public class RegistrationController : Controller
{
    private readonly ApplicationDbContext _context;

    public RegistrationController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> MyRegistrations()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var registrations = await _context.Registrations
            .Where(r => r.UserId == userId)
            .Include(r => r.Event)
            .OrderByDescending(r => r.RegistrationDate)
            .ToListAsync();

        return View(registrations);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unregister(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var registration = await _context.Registrations
            .Include(r => r.Event)
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (registration is null)
        {
            TempData["ErrorMessage"] = "Registration not found or access denied.";
            return RedirectToAction(nameof(MyRegistrations));
        }

        _context.Registrations.Remove(registration);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"You have unregistered from \"{registration.Event?.Title ?? "the event"}\".";
        return RedirectToAction(nameof(MyRegistrations));
    }
}
