using System.Security.Claims;
using CampusConnect.Data;
using CampusConnect.Models;
using CampusConnect.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Controllers;

[Authorize(Roles = "Student")]
public class MessageController : Controller
{
    private readonly ApplicationDbContext _context;

    public MessageController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var messages = await _context.Messages
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

        return View(messages);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MessageCreateViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var message = new Message
        {
            UserId = userId,
            Content = model.Content,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Message sent successfully.";
        return RedirectToAction(nameof(Index));
    }
}
