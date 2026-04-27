using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FitnessAppMVC.Models
{
    public class Meal
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Meal Name")]
        public string MealName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Meal Type")]
        public string MealType { get; set; } = "Breakfast";

        [Range(0, 10000)]
        public int Calories { get; set; } = 0;

        [Range(0, 1000)]
        [Display(Name = "Protein (g)")]
        public decimal? Protein { get; set; }

        [Range(0, 1000)]
        [Display(Name = "Carbs (g)")]
        public decimal? Carbs { get; set; }

        [Range(0, 500)]
        [Display(Name = "Fat (g)")]
        public decimal? Fat { get; set; }

        public string? Notes { get; set; }

        [Display(Name = "Logged At")]
        public DateTime LoggedAt { get; set; } = DateTime.Now;

        public virtual IdentityUser? User { get; set; }
    }
}
