using Microsoft.AspNetCore.Mvc;

namespace ArcheOne.Controllers
{
	public class LogInController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
