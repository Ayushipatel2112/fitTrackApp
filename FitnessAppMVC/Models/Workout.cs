using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FitnessAppMVC.Models
{
    public class Workout
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Exercise Name")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Workout Type")]
        public string WorkoutType { get; set; } = "strength";

        [Required]
        [Range(1, 480)]
        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }

        [Range(0, 5000)]
        [Display(Name = "Calories Burned")]
        public int CaloriesBurned { get; set; }

        [Range(1, 100)]
        public int? Sets { get; set; }

        [Range(1, 1000)]
        public int? Reps { get; set; }

        [Range(0, 1000)]
        [Display(Name = "Weight (kg)")]
        public decimal? WeightKg { get; set; }

        public string? Notes { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; } = DateTime.Now;

        public virtual IdentityUser? User { get; set; }
    }
}
