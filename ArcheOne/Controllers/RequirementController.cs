using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ArcheOne.Controllers
{
    public class RequirementController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly CommonHelper _commonHelper;
        private readonly ArcheOneDbContext _dbContext;
        public RequirementController(DbRepo dbRepo, CommonHelper commonHelper, ArcheOneDbContext dbContext)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Requirement()
        {
            List<SelectListItem> list = new List<SelectListItem>().ToList();
            //List<CompanyReqModel> companyReqModel = new List<CompanyReqModel>();
            var clientList = _dbRepo.ClientList().Select(x => new SelectListItem { Text = x.ClientName, Value = x.Id.ToString() }).ToList();
            ViewBag.Client = clientList;

            var positionList = _dbRepo.positionTypeList().Select(x => new SelectListItem { Text = x.PositionTypeName, Value = x.Id.ToString() }).ToList();
            ViewBag.Position = positionList;

            var requirementTypeList = _dbRepo.RequirementTypeList().Select(x => new SelectListItem { Text = x.RequirementTypeName, Value = x.Id.ToString() }).ToList();
            ViewBag.RequirementType = requirementTypeList;

            var employmentTypeList = _dbRepo.EmploymentTypeList().Select(x => new SelectListItem { Text = x.EmploymentTypeName, Value = x.Id.ToString() }).ToList();
            ViewBag.TypeOfEmployment = employmentTypeList;



            //List<CompanyReqModel> companyReqModel = new List<CompanyReqModel>();
            //var clientList = _dbRepo.ClientList().Select(x => new SelectListItem { Text = x.ClientName, Value = x.Id.ToString() }).ToList();
            //ViewBag.Client = clientList;

            //List<RoleReqModel> roleReqModel = new List<RoleReqModel>();
            //List<RoleMst> roles = new List<RoleMst>();
            //roles.Add(new RoleMst { Id = 0, RoleName = "---Select---" });
            //var roleList = _dbRepo.RoleMstList().Select(x => new RoleMst { Id = x.Id, RoleName = x.RoleName }).ToList();
            //roles.AddRange(roleList);

            ////var roles = _dbRepo.RoleMstList().Select(x => new SelectListItem { Text = x.RoleName, Value = x.Id.ToString() }).ToList();
            //ViewBag.Role = roles;
            return View();
        }
    }
}
