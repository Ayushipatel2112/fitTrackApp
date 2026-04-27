using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FitnessAppMVC.Models
{
    public class SleepRecord
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Sleep Date")]
        [DataType(DataType.Date)]
        public DateTime SleepDate { get; set; } = DateTime.Today;

        [Required]
        [StringLength(10)]
        [Display(Name = "Bed Time")]
        public string BedTime { get; set; } = "22:00";

        [Required]
        [StringLength(10)]
        [Display(Name = "Wake Time")]
        public string WakeTime { get; set; } = "06:00";

        [Required]
        [Range(0, 24)]
        [Display(Name = "Hours Slept")]
        public decimal HoursSlept { get; set; }

        [Required]
        [Display(Name = "Sleep Quality")]
        public string Quality { get; set; } = "Good";

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual IdentityUser? User { get; set; }
    }
}
