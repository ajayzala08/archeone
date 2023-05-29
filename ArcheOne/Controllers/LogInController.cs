using ArcheOne.Helper.CommonHelpers;
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
			if (isUserExist)
			{
				return View("~/Dashboard/Index");
			}
			else
			{
				ViewBag.msg = "UserName Password Not Match!!";
				return View();
			}
		}
	}
}
