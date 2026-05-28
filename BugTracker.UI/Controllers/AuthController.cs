using Microsoft.AspNetCore.Mvc;

namespace BugTracker.UI.Controllers
{
    public class AuthController : Controller
    {
        private const string DemoEmail = "demo@bugtrack.io";
        private const string DemoPassword = "bugtrack2024";

        private const string AdminEmail = "rafael.parra@bugtrack.io";
        private const string AdminPassword = "Admin@2024!";

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LiveDemo()
        {
            SetDemoSession(
                name: "Rafael Parra",
                role: "Administrator",
                initials: "RP",
                avatarClass: "av-rp",
                mode: "admin"
            );

            return RedirectToAction("Index", "Tickets");
        }

        [HttpPost]
        public IActionResult DemoLogin([FromBody] LoginRequest request)
        {
            var email = request.Email?.Trim() ?? "";
            var password = request.Password ?? "";

            var isDemo = email.Equals(DemoEmail, StringComparison.OrdinalIgnoreCase)
                         && password == DemoPassword;

            var isAdmin = email.Equals(AdminEmail, StringComparison.OrdinalIgnoreCase)
                          && password == AdminPassword;

            if (!isDemo && !isAdmin)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid email or password."
                });
            }

            if (isDemo)
            {
                SetDemoSession(
                    name: "Demo User",
                    role: "Demo Account",
                    initials: "DU",
                    avatarClass: "av-mc",
                    mode: "demo"
                );
            }

            if (isAdmin)
            {
                SetDemoSession(
                    name: "Rafael Parra",
                    role: "Administrator",
                    initials: "RP",
                    avatarClass: "av-rp",
                    mode: "admin"
                );
            }

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("Index", "Tickets")
            });
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("Index", "Home")
            });
        }

        private void SetDemoSession(string name, string role, string initials, string avatarClass, string mode)
        {
            HttpContext.Session.SetString("DemoUserName", name);
            HttpContext.Session.SetString("DemoUserRole", role);
            HttpContext.Session.SetString("DemoUserInitials", initials);
            HttpContext.Session.SetString("DemoUserAvatarClass", avatarClass);
            HttpContext.Session.SetString("DemoUserMode", mode);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}