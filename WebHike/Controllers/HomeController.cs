using Microsoft.AspNetCore.Mvc;

namespace WebHike.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
