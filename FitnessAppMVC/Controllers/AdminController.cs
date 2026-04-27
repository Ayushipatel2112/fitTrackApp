using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessAppMVC.Data;

namespace FitnessAppMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalUsers     = await _context.Users.CountAsync();
            ViewBag.ActiveUsers    = await _context.Users.CountAsync(u => !u.LockoutEnd.HasValue || u.LockoutEnd < DateTimeOffset.Now);
            ViewBag.TotalWorkouts  = await _context.Workouts.CountAsync();
            ViewBag.UnreadMessages = await _context.ContactMessages.CountAsync(m => !m.IsRead);

            ViewBag.RecentUsers = await _context.Users
                .OrderByDescending(u => EF.Property<DateTime>(u, "JoinedDate"))
                .Take(5)
                .ToListAsync();

            // Recent workouts act as activity feed (no AuditLogs table)
            ViewBag.RecentActivity = await _context.Workouts
                .OrderByDescending(w => w.Date)
                .Take(8)
                .ToListAsync();

            return View();
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Users(string? search)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(u =>
                    u.Email!.Contains(search) ||
                    EF.Property<string>(u, "FullName").Contains(search));

            var users = await query
                .OrderByDescending(u => EF.Property<DateTime>(u, "JoinedDate"))
                .ToListAsync();

            // Build list with roles + profile info from shadow properties
            var userList = new List<(IdentityUser user, IList<string> roles, string? fullName, string? goal)>();
            foreach (var u in users)
            {
                var roles    = await _userManager.GetRolesAsync(u);
                var fullName = EF.Property<string?>(_context.Entry(u).CurrentValues.ToObject() is IdentityUser uu ? uu : u, "FullName");
                // Read shadow property from tracked entity
                var fn   = _context.Entry(u).Property<string>("FullName").CurrentValue;
                var goal = _context.Entry(u).Property<string>("FitnessGoal").CurrentValue;
                userList.Add((u, roles, fn, goal));
            }

            ViewBag.Search   = search;
            ViewBag.UserList = userList;
            return View();
        }

        // POST: /Admin/DeleteUser
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                TempData["Success"] = $"User {user.Email} deleted.";
            }
            return RedirectToAction(nameof(Users));
        }

        // POST: /Admin/ToggleUserStatus
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                if (await _userManager.IsLockedOutAsync(user))
                {
                    await _userManager.SetLockoutEndDateAsync(user, null);
                    TempData["Success"] = "User account unlocked.";
                }
                else
                {
                    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                    TempData["Success"] = "User account locked.";
                }
            }
            return RedirectToAction(nameof(Users));
        }

        // GET: /Admin/Data
        public async Task<IActionResult> Data()
        {
            ViewBag.TotalWorkouts = await _context.Workouts.CountAsync();
            ViewBag.TotalMeals    = await _context.Meals.CountAsync();
            ViewBag.TotalSleep    = await _context.SleepLogs.CountAsync();
            ViewBag.TotalCalories = await _context.Meals.SumAsync(m => (long?)m.Calories) ?? 0;

            ViewBag.WorkoutStats = await _context.Workouts
                .GroupBy(w => w.WorkoutType)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToListAsync();

            ViewBag.RecentWorkouts = await _context.Workouts
                .Include(w => w.User)
                .OrderByDescending(w => w.Date)
                .Take(10)
                .ToListAsync();

            return View();
        }

        // GET: /Admin/AuditLogs – now shows recent user registrations/logins instead
        public async Task<IActionResult> AuditLogs(string? filter, int page = 1)
        {
            const int pageSize = 20;
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
                query = query.Where(u => u.Email!.Contains(filter));

            var total = await query.CountAsync();
            var users = await query
                .OrderByDescending(u => EF.Property<DateTime>(u, "JoinedDate"))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Users    = users;
            ViewBag.Total    = total;
            ViewBag.Page     = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Filter   = filter;
            return View();
        }
    }
}
