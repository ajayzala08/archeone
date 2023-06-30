using Microsoft.AspNetCore.Mvc;

namespace ArcheOne.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult PageNotFound()
        {
            return View();
        }
        public IActionResult Forbidden()
        {
            return View();
        }
    }
}
