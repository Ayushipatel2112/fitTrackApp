using System.ComponentModel.DataAnnotations;

namespace FitnessAppMVC.Models
{
    public class FitnessService
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string IconName { get; set; } // For Lucide icons like "dumbbell", "utensils"
    }
}
