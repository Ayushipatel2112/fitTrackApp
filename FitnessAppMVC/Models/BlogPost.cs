using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FitnessAppMVC.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string Excerpt { get; set; }
        public string Author { get; set; }
        public string Date { get; set; }
        public string ReadTime { get; set; }
        
        public string Content { get; set; }
        public string Takeaways { get; set; }

        [NotMapped]
        public List<string> ContentList => string.IsNullOrEmpty(Content) ? new List<string>() : Content.Split("\n").ToList();

        [NotMapped]
        public List<string> TakeawaysList => string.IsNullOrEmpty(Takeaways) ? new List<string>() : Takeaways.Split("\n").ToList();
    }
}
