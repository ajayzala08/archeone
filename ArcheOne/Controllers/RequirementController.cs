using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;

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


        public async Task<IActionResult> RequirementList(RequirementListReqModel getRequirementListReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            RequirementListResModel requirementListResModel = new RequirementListResModel();
            try
            {
                var requirementList = _dbRepo.GetRequirementList();
                var requirementForList = _dbRepo.GetRequirementForList();
                var clientList = _dbRepo.GetClientList();
                var positionTypeList = _dbRepo.GetPositionTypeList();
                var requirementTypeList = _dbRepo.GetRequirementTypeList();
                var employmentTypeList = _dbRepo.GetEmploymentTypeList();
                var requirementStatusList = _dbRepo.GetRequirementStatusList();

                if (getRequirementListReqModel.RequirementForId > 0)
                {
                    requirementList = requirementList.Where(x => x.RequirementForId == getRequirementListReqModel.RequirementForId);
                }
                if (getRequirementListReqModel.ClientId > 0)
                {
                    requirementList = requirementList.Where(x => x.ClientId == getRequirementListReqModel.ClientId);
                }
                if (getRequirementListReqModel.PositionTypeId > 0)
                {
                    requirementList = requirementList.Where(x => x.PositionTypeId == getRequirementListReqModel.PositionTypeId);
                }
                if (getRequirementListReqModel.RequirementTypeId > 0)
                {
                    requirementList = requirementList.Where(x => x.RequirementTypeId == getRequirementListReqModel.RequirementTypeId);
                }
                if (getRequirementListReqModel.EmploymentTypeId > 0)
                {
                    requirementList = requirementList.Where(x => x.EmploymentTypeId == getRequirementListReqModel.EmploymentTypeId);
                }
                if (getRequirementListReqModel.RequirementStatusId > 0)
                {
                    requirementList = requirementList.Where(x => x.RequirementStatusId == getRequirementListReqModel.RequirementStatusId);
                }

                requirementListResModel.RequirementList = new List<RequirementListModel>();
                foreach (var item in requirementList)
                {
                    var requirementForDetail = await requirementForList.FirstOrDefaultAsync(x => x.Id == item.RequirementForId);
                    var clientDetail = await clientList.FirstOrDefaultAsync(x => x.Id == item.ClientId);
                    var positionTypeDetail = await positionTypeList.FirstOrDefaultAsync(x => x.Id == item.PositionTypeId);
                    var requirementTypeDetail = await requirementTypeList.FirstOrDefaultAsync(x => x.Id == item.RequirementTypeId);
                    var employmentTypeDetail = await employmentTypeList.FirstOrDefaultAsync(x => x.Id == item.EmploymentTypeId);
                    var requirementStatusDetail = await requirementStatusList.FirstOrDefaultAsync(x => x.Id == item.RequirementStatusId);

                    RequirementListModel requirementListModel = new RequirementListModel();
                    requirementListModel.Id = item.Id;
                    requirementListModel.JobCode = item.JobCode;
                    requirementListModel.RequirementForId = requirementForDetail != null ? requirementForDetail.Id : 0;
                    requirementListModel.RequirementForName = requirementForDetail != null ? requirementForDetail.RequirementForName : "";
                    requirementListModel.ClientId = clientDetail != null ? clientDetail.Id : 0;
                    requirementListModel.ClientName = clientDetail != null ? clientDetail.ClientName : "";
                    requirementListModel.PositionTypeId = positionTypeDetail != null ? positionTypeDetail.Id : 0;
                    requirementListModel.PositionTypeName = positionTypeDetail != null ? positionTypeDetail.PositionTypeName : "";
                    requirementListModel.RequirementTypeId = requirementTypeDetail != null ? requirementTypeDetail.Id : 0;
                    requirementListModel.RequirementTypeName = requirementTypeDetail != null ? requirementTypeDetail.RequirementTypeName : "";
                    requirementListModel.EmploymentTypeId = employmentTypeDetail != null ? employmentTypeDetail.Id : 0;
                    requirementListModel.EmploymentTypeName = employmentTypeDetail != null ? employmentTypeDetail.EmploymentTypeName : "";
                    requirementListModel.RequirementStatusId = requirementStatusDetail != null ? requirementStatusDetail.Id : 0;
                    requirementListModel.RequirementStatusName = requirementStatusDetail != null ? requirementStatusDetail.RequirementStatusName : "";
                    requirementListModel.MainSkill = item.MainSkill;
                    requirementListModel.MandatorySkills = item.MandatorySkills;
                    requirementListModel.TotalMinExperience = item.TotalMinExperience;
                    requirementListModel.TotalMaxExperience = item.TotalMaxExperience;
                    requirementListModel.RelevantMinExperience = item.RelevantMinExperience;
                    requirementListModel.RelevantMaxExperience = item.RelevantMaxExperience;
                    requirementListModel.NoOfPosition = item.NoOfPosition;
                    requirementListModel.Location = item.Location;
                    requirementListModel.JobDescription = item.JobDescription;
                    requirementListModel.EndClient = item.EndClient;
                    requirementListModel.Pocname = item.Pocname;
                    requirementListModel.ClientBillRate = item.ClientBillRate;
                    requirementListModel.CandidatePayRate = item.CandidatePayRate;
                    requirementListModel.IsActive = item.IsActive;
                    requirementListModel.AssignedUserIds = item.AssignedUserIds;
                    requirementListModel.AssignedUserNames = item.AssignedUserIds;

                    requirementListResModel.RequirementList.Add(requirementListModel);
                }

                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "Success!";
                commonResponse.Data = requirementListResModel;
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Data = ex.StackTrace;
            }
            return View(commonResponse);
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
