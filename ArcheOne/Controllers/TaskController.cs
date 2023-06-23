using Microsoft.AspNetCore.Mvc;

namespace ArcheOne.Controllers
{
    public class TaskController : Controller
    {
        public IActionResult Task()
        {
            return View();
        }
    }
}
