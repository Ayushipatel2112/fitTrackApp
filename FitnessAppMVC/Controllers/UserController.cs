using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessAppMVC.Data;
using FitnessAppMVC.Models;

namespace FitnessAppMVC.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<string> GetCurrentUserId()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.Id ?? string.Empty;
        }

        // Helper: get display name from Users table shadow property
        private async Task<string> GetUserDisplayName(string userId)
        {
            var entry = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new {
                    FullName = EF.Property<string>(u, "FullName"),
                    Email    = u.Email
                })
                .FirstOrDefaultAsync();

            return (!string.IsNullOrEmpty(entry?.FullName))
                ? entry.FullName
                : (entry?.Email ?? "User");
        }

        public IActionResult Index() => RedirectToAction("Dashboard");

        // ──────────────────────────────────────────
        // DASHBOARD
        // ──────────────────────────────────────────
        public async Task<IActionResult> Dashboard()
        {
            var userId = await GetCurrentUserId();
            ViewBag.UserName = await GetUserDisplayName(userId);
            ViewBag.TotalWorkouts = await _context.Workouts.CountAsync(w => w.UserId == userId);
            ViewBag.TotalMeals    = await _context.Meals.CountAsync(m => m.UserId == userId);
            ViewBag.TotalSleep    = await _context.SleepLogs.CountAsync(s => s.UserId == userId);

            var todayStart = DateTime.Today;
            ViewBag.TodayCalories = await _context.Meals
                .Where(m => m.UserId == userId && m.LoggedAt >= todayStart)
                .SumAsync(m => (int?)m.Calories) ?? 0;

            ViewBag.RecentWorkouts = await _context.Workouts
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .Take(5)
                .ToListAsync();

            return View();
        }

        // ──────────────────────────────────────────
        // WORKOUTS CRUD
        // ──────────────────────────────────────────
        public async Task<IActionResult> Workouts(string? type)
        {
            var userId = await GetCurrentUserId();
            var query  = _context.Workouts.Where(w => w.UserId == userId);
            if (!string.IsNullOrEmpty(type)) query = query.Where(w => w.WorkoutType == type);
            ViewBag.FilterType = type;
            return View(await query.OrderByDescending(w => w.Date).ToListAsync());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWorkout(Workout model)
        {
            model.UserId = await GetCurrentUserId();
            if (string.IsNullOrEmpty(model.UserId)) return RedirectToAction("Login", "Account");
            _context.Workouts.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Workout logged successfully!";
            return RedirectToAction(nameof(Workouts));
        }

        [HttpGet]
        public async Task<IActionResult> EditWorkout(int id)
        {
            var userId  = await GetCurrentUserId();
            var workout = await _context.Workouts.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
            if (workout == null) return NotFound();
            return View(workout);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWorkout(int id, Workout model)
        {
            var userId  = await GetCurrentUserId();
            var workout = await _context.Workouts.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
            if (workout == null) return NotFound();

            workout.Title           = model.Title;
            workout.Description     = model.Description;
            workout.WorkoutType     = model.WorkoutType;
            workout.DurationMinutes = model.DurationMinutes;
            workout.CaloriesBurned  = model.CaloriesBurned;
            workout.Sets            = model.Sets;
            workout.Reps            = model.Reps;
            workout.WeightKg        = model.WeightKg;
            workout.Notes           = model.Notes;
            workout.Date            = model.Date;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Workout updated!";
            return RedirectToAction(nameof(Workouts));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWorkout(int id)
        {
            var userId  = await GetCurrentUserId();
            var workout = await _context.Workouts.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
            if (workout != null) { _context.Workouts.Remove(workout); await _context.SaveChangesAsync(); }
            TempData["Success"] = "Workout deleted.";
            return RedirectToAction(nameof(Workouts));
        }

        // ──────────────────────────────────────────
        // MEALS CRUD
        // ──────────────────────────────────────────
        public async Task<IActionResult> Meals(string? mealType)
        {
            var userId    = await GetCurrentUserId();
            var query     = _context.Meals.Where(m => m.UserId == userId);
            if (!string.IsNullOrEmpty(mealType)) query = query.Where(m => m.MealType == mealType);
            var todayStart = DateTime.Today;

            ViewBag.TodayCalories = await _context.Meals
                .Where(m => m.UserId == userId && m.LoggedAt >= todayStart)
                .SumAsync(m => (int?)m.Calories) ?? 0;
            ViewBag.TodayProtein = await _context.Meals
                .Where(m => m.UserId == userId && m.LoggedAt >= todayStart)
                .SumAsync(m => m.Protein) ?? 0;
            ViewBag.FilterType = mealType;

            return View(await query.OrderByDescending(m => m.LoggedAt).ToListAsync());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMeal(Meal model)
        {
            model.UserId = await GetCurrentUserId();
            _context.Meals.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Meal logged successfully!";
            return RedirectToAction(nameof(Meals));
        }

        [HttpGet]
        public async Task<IActionResult> EditMeal(int id)
        {
            var userId = await GetCurrentUserId();
            var meal   = await _context.Meals.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (meal == null) return NotFound();
            return View(meal);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMeal(int id, Meal model)
        {
            var userId = await GetCurrentUserId();
            var meal   = await _context.Meals.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (meal == null) return NotFound();

            meal.MealName = model.MealName;
            meal.MealType = model.MealType;
            meal.Calories = model.Calories;
            meal.Protein  = model.Protein;
            meal.Carbs    = model.Carbs;
            meal.Fat      = model.Fat;
            meal.Notes    = model.Notes;
            meal.LoggedAt = model.LoggedAt;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Meal updated!";
            return RedirectToAction(nameof(Meals));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMeal(int id)
        {
            var userId = await GetCurrentUserId();
            var meal   = await _context.Meals.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (meal != null) { _context.Meals.Remove(meal); await _context.SaveChangesAsync(); }
            TempData["Success"] = "Meal deleted.";
            return RedirectToAction(nameof(Meals));
        }

        // ──────────────────────────────────────────
        // SLEEP CRUD (uses SleepLogs table)
        // ──────────────────────────────────────────
        public async Task<IActionResult> Sleep()
        {
            var userId  = await GetCurrentUserId();
            var records = await _context.SleepLogs
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.SleepDate)
                .ToListAsync();

            ViewBag.AvgHours = records.Any()
                ? Math.Round(records.Take(7).Average(r => (double)r.HoursSlept), 1) : 0;
            return View(records);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSleep(SleepRecord model)
        {
            model.UserId = await GetCurrentUserId();
            _context.SleepLogs.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Sleep record logged!";
            return RedirectToAction(nameof(Sleep));
        }

        [HttpGet]
        public async Task<IActionResult> EditSleep(int id)
        {
            var userId = await GetCurrentUserId();
            var record = await _context.SleepLogs.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (record == null) return NotFound();
            return View(record);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSleep(int id, SleepRecord model)
        {
            var userId = await GetCurrentUserId();
            var record = await _context.SleepLogs.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (record == null) return NotFound();

            record.SleepDate  = model.SleepDate;
            record.BedTime    = model.BedTime;
            record.WakeTime   = model.WakeTime;
            record.HoursSlept = model.HoursSlept;
            record.Quality    = model.Quality;
            record.Notes      = model.Notes;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Sleep record updated!";
            return RedirectToAction(nameof(Sleep));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSleep(int id)
        {
            var userId = await GetCurrentUserId();
            var record = await _context.SleepLogs.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (record != null) { _context.SleepLogs.Remove(record); await _context.SaveChangesAsync(); }
            TempData["Success"] = "Sleep record deleted.";
            return RedirectToAction(nameof(Sleep));
        }

        // ──────────────────────────────────────────
        // PROGRESS
        // ──────────────────────────────────────────
        public async Task<IActionResult> Progress()
        {
            var userId   = await GetCurrentUserId();
            var workouts = await _context.Workouts.Where(w => w.UserId == userId).OrderBy(w => w.Date).ToListAsync();
            var meals    = await _context.Meals.Where(m => m.UserId == userId).OrderBy(m => m.LoggedAt).ToListAsync();
            var sleep    = await _context.SleepLogs.Where(s => s.UserId == userId).OrderByDescending(s => s.SleepDate).Take(7).ToListAsync();

            ViewBag.TotalCalBurned  = workouts.Sum(w => w.CaloriesBurned);
            ViewBag.TotalCalConsumed = meals.Sum(m => m.Calories);
            ViewBag.AvgSleep        = sleep.Any() ? Math.Round(sleep.Average(s => (double)s.HoursSlept), 1) : 0;
            ViewBag.Workouts        = workouts;
            ViewBag.Meals           = meals;
            ViewBag.Sleep           = sleep;
            return View();
        }

        // ──────────────────────────────────────────
        // LEADERBOARD (reads name from Users table)
        // ──────────────────────────────────────────
        public async Task<IActionResult> Leaderboard()
        {
            var leaderboard = await _context.Workouts
                .GroupBy(w => w.UserId)
                .Select(g => new {
                    UserId        = g.Key,
                    WorkoutCount  = g.Count(),
                    TotalCalories = g.Sum(w => w.CaloriesBurned),
                    TotalMinutes  = g.Sum(w => w.DurationMinutes)
                })
                .OrderByDescending(x => x.TotalCalories)
                .Take(10)
                .ToListAsync();

            var result = new List<(string name, int workouts, int calories, int minutes, int rank)>();
            int rank   = 1;
            foreach (var entry in leaderboard)
            {
                var name = await GetUserDisplayName(entry.UserId);
                result.Add((name, entry.WorkoutCount, entry.TotalCalories, entry.TotalMinutes, rank++));
            }

            ViewBag.Leaderboard    = result;
            ViewBag.CurrentUserId  = await GetCurrentUserId();
            return View();
        }

        // ──────────────────────────────────────────
        // CHALLENGES (static page)
        // ──────────────────────────────────────────
        public IActionResult Challenges() => View();

        // ──────────────────────────────────────────
        // PROFILE (reads/writes shadow props on Users)
        // ──────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Read shadow properties from the Users table
            var entry = await _context.Users
                .Where(u => u.Id == user.Id)
                .Select(u => new UserProfileViewModel
                {
                    FullName      = EF.Property<string>(u, "FullName"),
                    Phone         = EF.Property<string>(u, "Phone"),
                    Age           = EF.Property<int?>(u, "Age"),
                    HeightCm      = EF.Property<decimal?>(u, "HeightCm"),
                    WeightKg      = EF.Property<decimal?>(u, "WeightKg"),
                    FitnessGoal   = EF.Property<string>(u, "FitnessGoal"),
                    ActivityLevel = EF.Property<string>(u, "ActivityLevel")
                })
                .FirstOrDefaultAsync() ?? new UserProfileViewModel();

            ViewBag.Email = user.Email ?? "";
            return View(entry);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Update shadow properties on Users table
            var dbUser = await _context.Users.FindAsync(user.Id);
            if (dbUser != null)
            {
                _context.Entry(dbUser).Property("FullName").CurrentValue      = model.FullName;
                _context.Entry(dbUser).Property("Phone").CurrentValue         = model.Phone;
                _context.Entry(dbUser).Property("Age").CurrentValue           = model.Age;
                _context.Entry(dbUser).Property("HeightCm").CurrentValue      = model.HeightCm;
                _context.Entry(dbUser).Property("WeightKg").CurrentValue      = model.WeightKg;
                _context.Entry(dbUser).Property("FitnessGoal").CurrentValue   = model.FitnessGoal;
                _context.Entry(dbUser).Property("ActivityLevel").CurrentValue = model.ActivityLevel;
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Profile));
        }

        // ──────────────────────────────────────────
        // CHANGE PASSWORD
        // ──────────────────────────────────────────
        [HttpGet]
        public IActionResult ChangePassword() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "New passwords do not match.";
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                TempData["Success"] = "Password changed successfully!";
                return RedirectToAction(nameof(Profile));
            }

            ViewBag.Error = string.Join(" ", result.Errors.Select(e => e.Description));
            return View();
        }
    }
}
