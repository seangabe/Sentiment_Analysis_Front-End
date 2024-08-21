using Microsoft.AspNetCore.Mvc;

namespace TWIL1.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
