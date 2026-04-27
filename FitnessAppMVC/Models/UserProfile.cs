using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessAppMVC.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [Range(10, 120)]
        public int? Age { get; set; }

        [Range(50, 300)]
        [Display(Name = "Height (cm)")]
        public decimal? HeightCm { get; set; }

        [Range(20, 500)]
        [Display(Name = "Weight (kg)")]
        public decimal? WeightKg { get; set; }

        [StringLength(200)]
        [Display(Name = "Fitness Goal")]
        public string? FitnessGoal { get; set; }

        [StringLength(50)]
        [Display(Name = "Activity Level")]
        public string? ActivityLevel { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
