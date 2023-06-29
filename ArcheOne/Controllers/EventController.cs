using Microsoft.AspNetCore.Mvc;

namespace ArcheOne.Controllers
{
    public class EventController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Event()
        {
            return View();
        }
        public IActionResult EventList()
        {
            return View();
        }
        public IActionResult AddEditEvent()
        {
            return View();
        }

    }
}
