using Microsoft.AspNetCore.Mvc;

namespace TWIL1.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
