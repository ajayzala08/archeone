using Microsoft.AspNetCore.Mvc;

namespace ArcheOne.Controllers
{
	public class DashboardController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
