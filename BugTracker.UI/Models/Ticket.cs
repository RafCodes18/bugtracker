namespace BugTracker.UI.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";
        public string Description { get; set; } = "";

        public string Type { get; set; } = "Bug";          // Bug, Feature, Task
        public string Status { get; set; } = "Open";      // Open, In Progress, Review, Resolved
        public string Priority { get; set; } = "Medium";  // Critical, High, Medium, Low

        public string Assignee { get; set; } = "Rafael Parra";
        public string Initials { get; set; } = "RP";
        public string AvatarClass { get; set; } = "av-rp";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<TicketActivity> Activities { get; set; } = new();
    }
}
