using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ArcheOne.Controllers
{
	public class TeamController : Controller
	{
		private readonly DbRepo _dbRepo;
		private readonly CommonHelper _commonHelper;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly ArcheOneDbContext _dbContext;
		public TeamController(DbRepo dbRepo, CommonHelper commonHelper, IWebHostEnvironment webHostEnvironment, ArcheOneDbContext dbContext)
		{
			_dbRepo = dbRepo;
			_commonHelper = commonHelper;
			_webHostEnvironment = webHostEnvironment;
			_dbContext = dbContext;
		}
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Team()
		{
			List<SelectListItem> list = new List<SelectListItem>().ToList();
			var teamLeadList = _dbRepo.UserMstList().Select(x => new SelectListItem { Text = x.UserName, Value = x.Id.ToString()}).ToList();
			ViewBag.TeamLead = teamLeadList;

         

            List<SelectListItem> list1 = new List<SelectListItem>().ToList();
			var teamMemberList = _dbRepo.UserMstList().Select(x => new SelectListItem { Text = x.UserName, Value = x.Id.ToString() }).ToList();
			ViewBag.teamMember = teamMemberList;

            List<SelectListItem> list2 = new List<SelectListItem>().ToList();
            var clientList = _dbRepo.ClientList().Select(x => new SelectListItem { Text = x.ClientName, Value = x.Id.ToString() }).ToList();
            ViewBag.Client = clientList;


            return View();
		}
	}
}
