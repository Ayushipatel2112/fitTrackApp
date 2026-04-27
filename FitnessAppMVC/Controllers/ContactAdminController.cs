using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessAppMVC.Data;
using FitnessAppMVC.Models;

namespace FitnessAppMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ContactAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ContactAdmin - All messages
        public async Task<IActionResult> Index(string? filter)
        {
            var query = _context.ContactMessages.AsQueryable();
            if (filter == "unread") query = query.Where(m => !m.IsRead);
            var messages = await query.OrderByDescending(m => m.SentAt).ToListAsync();
            ViewBag.Filter = filter;
            ViewBag.UnreadCount = await _context.ContactMessages.CountAsync(m => !m.IsRead);
            return View(messages);
        }

        // GET: ContactAdmin/Details/5 (marks as read)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var msg = await _context.ContactMessages.FirstOrDefaultAsync(m => m.Id == id);
            if (msg == null) return NotFound();

            // Mark as read
            if (!msg.IsRead)
            {
                msg.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return View(msg);
        }

        // POST: Mark as read toggle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleRead(int id)
        {
            var msg = await _context.ContactMessages.FindAsync(id);
            if (msg != null)
            {
                msg.IsRead = !msg.IsRead;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ContactAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var msg = await _context.ContactMessages.FirstOrDefaultAsync(m => m.Id == id);
            if (msg == null) return NotFound();
            return View(msg);
        }

        // POST: ContactAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var msg = await _context.ContactMessages.FindAsync(id);
            if (msg != null)
            {
                _context.ContactMessages.Remove(msg);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Message deleted.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
