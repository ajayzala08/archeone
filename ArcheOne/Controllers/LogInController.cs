using ArcheOne.Helper;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Mvc;

namespace ArcheOne.Controllers
{
	public class LogInController : Controller
	{
		private readonly DbRepo _dbRepo;
		public LogInController(DbRepo dbRepo)
		{
			_dbRepo = dbRepo;
		}
		public IActionResult Index()
		{
			return View();
		}
		[HttpPost]
		public IActionResult LogIn(LoginModel loginModel)
		{
			bool isUserExist = false;
			isUserExist = _dbRepo.UserMstList().Where(x => x.UserName == loginModel.UserName && x.Password == loginModel.Password).ToList().Any();
			return View("~/Dashboard/Index");
		}
	}
}
