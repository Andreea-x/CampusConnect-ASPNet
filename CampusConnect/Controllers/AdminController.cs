using CampusConnect.Data;
using CampusConnect.Models;
using CampusConnect.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var events = await _context.Events
            .Include(e => e.Registrations)
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();

        var model = new AdminDashboardViewModel
        {
            TotalEventsCount = await _context.Events.CountAsync(),
            TotalRegistrationsCount = await _context.Registrations.CountAsync(),
            TotalMessagesCount = await _context.Messages.CountAsync(),
            Events = events
        };

        return View(model);
    }

    public IActionResult CreateEvent()
    {
        return View(new EventFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEvent(EventFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var eventItem = new Event
        {
            Title = model.Title,
            Description = model.Description,
            EventDate = DateTime.SpecifyKind(model.EventDate, DateTimeKind.Utc),
            Capacity = model.Capacity
        };

        _context.Events.Add(eventItem);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Event created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> EditEvent(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        var model = new EventFormViewModel
        {
            Id = eventItem.Id,
            Title = eventItem.Title,
            Description = eventItem.Description,
            EventDate = eventItem.EventDate,
            Capacity = eventItem.Capacity
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEvent(int id, EventFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        eventItem.Title = model.Title;
        eventItem.Description = model.Description;
        eventItem.EventDate = DateTime.SpecifyKind(model.EventDate, DateTimeKind.Utc);
        eventItem.Capacity = model.Capacity;

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Event updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DeleteEvent(int id)
    {
        var eventItem = await _context.Events
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eventItem is null)
        {
            return NotFound();
        }

        return View(eventItem);
    }

    [HttpPost, ActionName("DeleteEvent")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEventConfirmed(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        _context.Events.Remove(eventItem);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Event deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ViewRegistrations()
    {
        var registrations = await _context.Registrations
            .Include(r => r.User)
            .Include(r => r.Event)
            .OrderByDescending(r => r.RegistrationDate)
            .Select(r => new AdminRegistrationViewModel
            {
                RegistrationId = r.Id,
                StudentName = r.User!.FullName,
                StudentEmail = r.User.Email ?? string.Empty,
                EventTitle = r.Event!.Title,
                EventDate = r.Event.EventDate,
                RegistrationDate = r.RegistrationDate
            })
            .ToListAsync();

        return View(registrations);
    }

    public async Task<IActionResult> ViewMessages()
    {
        var messages = await _context.Messages
            .Include(m => m.User)
            .OrderByDescending(m => m.SentAt)
            .Select(m => new AdminMessageViewModel
            {
                Id = m.Id,
                StudentName = m.User!.FullName,
                StudentEmail = m.User.Email ?? string.Empty,
                Content = m.Content,
                SentAt = m.SentAt,
                AdminReply = m.AdminReply,
                RepliedAt = m.RepliedAt
            })
            .ToListAsync();

        return View(messages);
    }

    public async Task<IActionResult> ReplyMessage(int id)
    {
        var message = await _context.Messages
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (message is null)
        {
            return NotFound();
        }

        var model = new AdminReplyMessageViewModel
        {
            Id = message.Id,
            StudentName = message.User?.FullName ?? string.Empty,
            StudentEmail = message.User?.Email ?? string.Empty,
            Content = message.Content,
            SentAt = message.SentAt,
            ExistingReply = message.AdminReply,
            ReplyContent = message.AdminReply ?? string.Empty
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReplyMessage(AdminReplyMessageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var message = await _context.Messages
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == model.Id);

        if (message is null)
        {
            return NotFound();
        }

        message.AdminReply = model.ReplyContent;
        message.RepliedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Reply sent successfully.";
        return RedirectToAction(nameof(ViewMessages));
    }
}
