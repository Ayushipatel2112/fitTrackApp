namespace FitnessAppMVC.Models
{
    /// <summary>
    /// ViewModel for the Profile page.
    /// Data is stored directly in the Users table via shadow properties.
    /// No separate UserProfiles table needed.
    /// </summary>
    public class UserProfileViewModel
    {
        public string?  FullName      { get; set; }
        public string?  Phone         { get; set; }
        public int?     Age           { get; set; }
        public decimal? HeightCm      { get; set; }
        public decimal? WeightKg      { get; set; }
        public string?  FitnessGoal   { get; set; }
        public string?  ActivityLevel { get; set; }
    }
}
