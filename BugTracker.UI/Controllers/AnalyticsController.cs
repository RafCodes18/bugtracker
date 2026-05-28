using Microsoft.AspNetCore.Mvc;

namespace BugTracker.UI.Controllers
{
    public class AnalyticsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
