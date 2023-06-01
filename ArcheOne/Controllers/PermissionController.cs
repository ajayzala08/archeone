using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ArcheOne.Controllers
{
	public class PermissionController : Controller
	{
		private readonly ArcheOneDbContext _dbContext;
		private readonly DbRepo _dbRepo;
		public PermissionController(ArcheOneDbContext dbContext, DbRepo dbRepo)
		{
			_dbContext = dbContext;
			_dbRepo = dbRepo;
		}

		public IActionResult DefaultPermission()
		{
			return View();
		}

		public IActionResult GetDefaultPermissionList()
		{
			CommonResponse response = new CommonResponse();
			try
			{
				var UserList = _dbRepo.UserMstList().ToList();
				response.Status = true;
				response.StatusCode = HttpStatusCode.OK;
				response.Data = UserList;
			}
			catch (Exception ex)
			{
				response.Message = ex.Message;
			}
			return Json(new { res = response });
		}
	}
}
