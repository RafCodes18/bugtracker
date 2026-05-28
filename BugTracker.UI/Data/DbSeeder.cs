using BugTracker.UI.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.UI.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            if (await db.Tickets.AnyAsync())
                return;

            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Title = "Login timeout not clearing session data properly",
                    Description = "Users remain partially authenticated after the session expires, causing inconsistent redirects and stale account state.",
                    Type = "Bug",
                    Status = "Open",
                    Priority = "High",
                    Assignee = "T. Kowalski",
                    Initials = "TK",
                    AvatarClass = "av-tk",
                    UpdatedAt = DateTime.UtcNow,
                    Activities = new List<TicketActivity>
                    {
                        new TicketActivity { Message = "T. Kowalski opened the ticket." },
                        new TicketActivity { Message = "Rafael Parra added reproduction steps." },
                        new TicketActivity { Message = "Priority changed from Medium to High." }
                    }
                },
                new Ticket
                {
                    Title = "Export CSV button unresponsive on reports page",
                    Description = "The export button does not respond after report filters are changed multiple times.",
                    Type = "Bug",
                    Status = "In Progress",
                    Priority = "Medium",
                    Assignee = "Rafael Parra",
                    Initials = "RP",
                    AvatarClass = "av-rp",
                    UpdatedAt = DateTime.UtcNow,
                    Activities = new List<TicketActivity>
                    {
                        new TicketActivity { Message = "Assigned to Rafael Parra." },
                        new TicketActivity { Message = "Status changed to In Progress." },
                        new TicketActivity { Message = "Frontend event binding issue confirmed." }
                    }
                },
                new Ticket
                {
                    Title = "Add pagination to user management grid",
                    Description = "The user management screen needs pagination to prevent long load times for larger teams.",
                    Type = "Feature",
                    Status = "Review",
                    Priority = "Low",
                    Assignee = "M. Chen",
                    Initials = "MC",
                    AvatarClass = "av-mc",
                    UpdatedAt = DateTime.UtcNow.AddDays(-1),
                    Activities = new List<TicketActivity>
                    {
                        new TicketActivity { Message = "M. Chen submitted changes." },
                        new TicketActivity { Message = "Pull request opened." },
                        new TicketActivity { Message = "Waiting for review." }
                    }
                },
                new Ticket
                {
                    Title = "Null reference on report generation",
                    Description = "Generating a report without selecting a project throws a null reference exception.",
                    Type = "Bug",
                    Status = "Open",
                    Priority = "Critical",
                    Assignee = "Rafael Parra",
                    Initials = "RP",
                    AvatarClass = "av-rp",
                    UpdatedAt = DateTime.UtcNow.AddDays(-1),
                    Activities = new List<TicketActivity>
                    {
                        new TicketActivity { Message = "QA reproduced the issue." },
                        new TicketActivity { Message = "Critical priority assigned." },
                        new TicketActivity { Message = "Needs backend validation." }
                    }
                }
            };

            db.Tickets.AddRange(tickets);
            await db.SaveChangesAsync();
        }
    }
}
