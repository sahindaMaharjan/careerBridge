namespace careerBridge.Models
{
    public class EventListViewModel
    {
        public List<EventItem> SearchResults { get; set; } = new();
        public List<EventItem> DefaultEvents { get; set; } = new();
    }
}
