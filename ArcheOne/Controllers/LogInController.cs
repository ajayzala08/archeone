using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Mvc;

namespace ArcheOne.Controllers
{
	public class LogInController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
		[HttpPost]
		public IActionResult LogIn(LoginModel loginModel)
		{

			return View("~/Dashboard/Index");
		}
	}
}
