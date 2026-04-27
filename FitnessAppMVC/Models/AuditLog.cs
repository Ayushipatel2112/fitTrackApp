using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessAppMVC.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        [StringLength(300)]
        public string? UserEmail { get; set; }

        [Required]
        [StringLength(300)]
        public string Action { get; set; } = string.Empty;

        public string? Details { get; set; }

        [StringLength(50)]
        public string? IpAddress { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
