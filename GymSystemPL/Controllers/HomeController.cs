using Microsoft.AspNetCore.Mvc;

namespace GymSystemPL.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
