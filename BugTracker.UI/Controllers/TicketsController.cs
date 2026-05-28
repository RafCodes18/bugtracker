using BugTracker.UI.Data;
using BugTracker.UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.UI.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TicketsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var tickets = await _db.Tickets
                .Include(t => t.Activities)
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();

            return View(tickets);
        }

        public IActionResult Users()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TicketRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
                return BadRequest("Title and description are required.");

            var currentUser = CurrentDemoUserName();
            var avatar = GetAssigneeAvatar(request.Assignee);

            var ticket = new Ticket
            {
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                Type = request.Type,
                Status = request.Status,
                Priority = request.Priority,
                Assignee = request.Assignee,
                Initials = avatar.Initials,
                AvatarClass = avatar.AvatarClass,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Activities = new List<TicketActivity>
                {
                    new TicketActivity
                    {
                        Message = $"Ticket created by {currentUser}.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new TicketActivity
                    {
                        Message = $"Assigned to {request.Assignee}.",
                        CreatedAt = DateTime.UtcNow
                    }
                }
            };

            _db.Tickets.Add(ticket);
            await _db.SaveChangesAsync();

            return Json(ToTicketDto(ticket));
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] TicketRequest request)
        {
            var ticket = await _db.Tickets
                .Include(t => t.Activities)
                .FirstOrDefaultAsync(t => t.Id == request.Id);

            if (ticket == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
                return BadRequest("Title and description are required.");

            var currentUser = CurrentDemoUserName();
            var avatar = GetAssigneeAvatar(request.Assignee);

            ticket.Title = request.Title.Trim();
            ticket.Description = request.Description.Trim();
            ticket.Type = request.Type;
            ticket.Status = request.Status;
            ticket.Priority = request.Priority;
            ticket.Assignee = request.Assignee;
            ticket.Initials = avatar.Initials;
            ticket.AvatarClass = avatar.AvatarClass;
            ticket.UpdatedAt = DateTime.UtcNow;

            ticket.Activities.Add(new TicketActivity
            {
                Message = $"Ticket details updated by {currentUser}.",
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();

            return Json(ToTicketDto(ticket));
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] TicketIdRequest request)
        {
            var ticket = await _db.Tickets
                .Include(t => t.Activities)
                .FirstOrDefaultAsync(t => t.Id == request.Id);

            if (ticket == null)
                return NotFound();

            _db.Tickets.Remove(ticket);
            await _db.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus([FromBody] TicketStatusRequest request)
        {
            var ticket = await _db.Tickets
                .Include(t => t.Activities)
                .FirstOrDefaultAsync(t => t.Id == request.Id);

            if (ticket == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(request.Status))
                return BadRequest("Status is required.");

            var currentUser = CurrentDemoUserName();
            var oldStatus = ticket.Status;

            ticket.Status = request.Status;
            ticket.UpdatedAt = DateTime.UtcNow;

            ticket.Activities.Add(new TicketActivity
            {
                Message = $"{currentUser} changed status from {oldStatus} to {request.Status}.",
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();

            return Json(ToTicketDto(ticket));
        }

        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] TicketCommentRequest request)
        {
            var ticket = await _db.Tickets
                .Include(t => t.Activities)
                .FirstOrDefaultAsync(t => t.Id == request.Id);

            if (ticket == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(request.Comment))
                return BadRequest("Comment cannot be empty.");

            var currentUser = CurrentDemoUserName();

            ticket.UpdatedAt = DateTime.UtcNow;

            ticket.Activities.Add(new TicketActivity
            {
                Message = $"{currentUser} commented: \"{request.Comment.Trim()}\"",
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();

            return Json(ToTicketDto(ticket));
        }

        private string CurrentDemoUserName()
        {
            return HttpContext.Session.GetString("DemoUserName") ?? "Rafael Parra";
        }

        private static object ToTicketDto(Ticket t)
        {
            return new
            {
                dbId = t.Id,
                id = $"#{1000 + t.Id}",
                title = t.Title,
                description = t.Description,
                type = t.Type,
                status = t.Status,
                priority = t.Priority,
                assignee = t.Assignee,
                initials = t.Initials,
                avClass = t.AvatarClass,
                updated = RelativeTime(t.UpdatedAt),
                activity = t.Activities
                    .OrderBy(a => a.CreatedAt)
                    .Select(a => a.Message)
                    .ToList()
            };
        }

        private static string RelativeTime(DateTime date)
        {
            var diff = DateTime.UtcNow - date;

            if (diff.TotalMinutes < 2) return "Just now";
            if (diff.TotalHours < 24) return "Today";
            if (diff.TotalDays < 2) return "Yesterday";

            return $"{Math.Floor(diff.TotalDays)} days ago";
        }

        private static (string Initials, string AvatarClass) GetAssigneeAvatar(string assignee)
        {
            return assignee switch
            {
                "Rafael Parra" => ("RP", "av-rp"),
                "M. Chen" => ("MC", "av-mc"),
                "S. Patel" => ("SP", "av-sp"),
                "T. Kowalski" => ("TK", "av-tk"),
                "Demo User" => ("DU", "av-mc"),
                _ => ("?", "")
            };
        }
    }

    public class TicketRequest
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";
        public string Description { get; set; } = "";

        public string Type { get; set; } = "Bug";
        public string Status { get; set; } = "Open";
        public string Priority { get; set; } = "Medium";

        public string Assignee { get; set; } = "Rafael Parra";
    }

    public class TicketIdRequest
    {
        public int Id { get; set; }
    }

    public class TicketStatusRequest
    {
        public int Id { get; set; }
        public string Status { get; set; } = "Open";
    }

    public class TicketCommentRequest
    {
        public int Id { get; set; }
        public string Comment { get; set; } = "";
    }
}