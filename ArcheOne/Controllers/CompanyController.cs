using System.Net;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using Microsoft.AspNetCore.Mvc;

namespace ArcheOne.Controllers
{
	public class CompanyController : Controller
	{
		private readonly DbRepo _dbRepo;
		public CompanyController(DbRepo dbRepo)
		{
			_dbRepo = dbRepo;
		}
		public IActionResult Company()
		{
			CommonResponse commonResponse = new CommonResponse();
			var companies = _dbRepo.CompanyMstList().ToList();
			if (companies.Count > 0)
			{
				commonResponse.Data = companies;
				commonResponse.Status = true;
				commonResponse.StatusCode = HttpStatusCode.OK;
				commonResponse.Message = "Data found successfully !";
			}
			else
			{
				commonResponse.Message = "Data not found !";
				commonResponse.StatusCode = HttpStatusCode.NotFound;
			}
			ViewBag.Company = commonResponse.Data;
			return Json(companies);
		}
	}
}
