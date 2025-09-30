namespace TaDaa.WebUI.ViewModels
{
    internal class DayDetailViewModel
    {
        public DateTime Date { get; set; }
        public string DayName { get; set; }
        public string Emoji { get; set; }
        public List<TaskEntryViewModel> Tasks { get; set; }
        public double AverageRating { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool HasData { get; set; }
    }
}