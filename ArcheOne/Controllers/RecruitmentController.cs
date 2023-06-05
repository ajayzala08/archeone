using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using Microsoft.AspNetCore.Mvc;

namespace ArcheOne.Controllers
{
    public class RecruitmentController : Controller
    {
		private readonly DbRepo _dbRepo;
		private readonly CommonHelper _commonHelper;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly ArcheOneDbContext _dbContext;
		public RecruitmentController(DbRepo dbRepo, CommonHelper commonHelper, IWebHostEnvironment webHostEnvironment, ArcheOneDbContext dbContext)
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
    }
}
