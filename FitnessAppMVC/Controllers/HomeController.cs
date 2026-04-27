using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FitnessAppMVC.Models;
using FitnessAppMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace FitnessAppMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();

        public IActionResult Blog()
        {
            var posts = new List<BlogPost>
            {
                new BlogPost { Id = 1, Title = "Mastering Your Morning Routine", Excerpt = "How starting your day with a solid routine can transform your productivity and health.", Category = "Lifestyle", Author = "Sarah Jenkins", Date = "Oct 12, 2025", ReadTime = "5 min" },
                new BlogPost { Id = 2, Title = "Top 5 Strength Training Exercises", Excerpt = "Focus on these foundational movements to build strength and muscle efficiently.", Category = "Training", Author = "Mike Thorne", Date = "Oct 10, 2025", ReadTime = "8 min" },
                new BlogPost { Id = 3, Title = "The Truth About Meal Prepping", Excerpt = "Is meal prepping worth the time? We break down the pros, cons, and best strategies.", Category = "Nutrition", Author = "Elena Rodriguez", Date = "Oct 08, 2025", ReadTime = "6 min" },
                new BlogPost { Id = 4, Title = "Recovery: The Missing Piece", Excerpt = "Why rest days are just as important as your training sessions for long-term growth.", Category = "Recovery", Author = "Chris Evans", Date = "Oct 05, 2025", ReadTime = "4 min" }
            };
            return View(posts);
        }

        public IActionResult BlogArticle(int id)
        {
            // Simple logic for the article page
            var post = new BlogPost { Id = id, Title = "Sample Article " + id, Category = "Fitness", Author = "Admin", Date = "2026", ReadTime = "10 min" };
            return View(post);
        }

        // Hardcoded services to keep the database small (exactly 7 tables)
        public IActionResult Services()
        {
            var services = new List<FitnessService>
            {
                new FitnessService { Title = "Workout Tracking", Description = "Log and analyze your workouts with detailed metrics.", IconName = "dumbbell" },
                new FitnessService { Title = "Meal Planning", Description = "Plan and track your daily nutrition and calorie intake.", IconName = "utensils" },
                new FitnessService { Title = "Sleep Monitor", Description = "Track your sleep patterns and improve your recovery.", IconName = "moon" },
                new FitnessService { Title = "Progress Analytics", Description = "Visualize your fitness journey with charts and trends.", IconName = "trending-up" },
                new FitnessService { Title = "Challenges", Description = "Join fitness challenges and compete with friends.", IconName = "trophy" },
                new FitnessService { Title = "Leaderboard", Description = "See how you rank among other FitTrack users.", IconName = "award" }
            };
            return View(services);
        }

        [HttpGet]
        public IActionResult Contact() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMessage model)
        {
            if (ModelState.IsValid)
            {
                _context.ContactMessages.Add(model);
                await _context.SaveChangesAsync();
                ViewBag.Success = "Your message has been sent successfully!";
                return View();
            }
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
