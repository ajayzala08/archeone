using Microsoft.AspNetCore.Mvc;

namespace ArcheOne.Controllers
{
    public class LeavesController : Controller
    {
        public IActionResult Leaves()
        {
            return View();
        }

    }
}
