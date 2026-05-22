using CampusConnect.Data;
using CampusConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Controllers;

public class AnnouncementsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AnnouncementsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var announcements = await _context.Announcements
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return View(announcements);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View(new Announcement());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Announcement model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.CreatedAt = DateTime.UtcNow;
        _context.Announcements.Add(model);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Announcement created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement is null)
        {
            return NotFound();
        }

        return View(announcement);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Announcement model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement is null)
        {
            return NotFound();
        }

        announcement.Title = model.Title;
        announcement.Content = model.Content;

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Announcement updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement is null)
        {
            return NotFound();
        }

        return View(announcement);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement is null)
        {
            return NotFound();
        }

        _context.Announcements.Remove(announcement);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Announcement deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
