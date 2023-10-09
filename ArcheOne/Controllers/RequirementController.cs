using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<IActionResult> RequirementList(RequirementListReqModel getRequirementListReqModel)
        {
            CommonResponse response = new CommonResponse();
            RequirementListResModel requirementListResModel = new RequirementListResModel();
            try
            {
                var requirementList = _dbRepo.RequirementList();
                var requirementForList = _dbRepo.RequirementForList();
                var clientList = _dbRepo.ClientList();
                var positionTypeList = _dbRepo.PositionTypeList();
                var requirementTypeList = _dbRepo.RequirementTypeList();
                var employmentTypeList = _dbRepo.EmploymentTypeList();
                var requirementStatusList = _dbRepo.RequirementStatusList();

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
                    requirementList = requirementList.Where(x => x.CreatedDate.Date >= x.CreatedDate.Date.AddDays(3));
                }

                var list = await requirementList.ToListAsync();
                requirementListResModel.RequirementList = new List<RequirementListModel>();
                requirementListResModel.RequirementStatusList = await _dbRepo.RequirementStatusList().Select(x => new KeyValueModel { Id = x.Id, Name = x.RequirementStatusName }).ToListAsync();

                foreach (var item in list)
                {
                    var requirementForDetail = await requirementForList.FirstOrDefaultAsync(x => x.Id == item.RequirementForId);
                    var clientDetail = await clientList.FirstOrDefaultAsync(x => x.Id == item.ClientId);
                    var positionTypeDetail = await positionTypeList.FirstOrDefaultAsync(x => x.Id == item.PositionTypeId);
                    var requirementTypeDetail = await requirementTypeList.FirstOrDefaultAsync(x => x.Id == item.RequirementTypeId);
                    var employmentTypeDetail = await employmentTypeList.FirstOrDefaultAsync(x => x.Id == item.EmploymentTypeId);
                    // var requirementStatusDetail = await requirementStatusList.FirstOrDefaultAsync(x => x.Id == item.RequirementStatusId);
                    bool requirementStatusDetail = item.CreatedDate.Date.AddDays(3) >= _commonHelper.GetCurrentDateTime().Date;

                    RequirementListModel requirementListModel = new RequirementListModel();
                    requirementListModel.RequirementId = item.Id;
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
                    requirementListModel.RequirementStatusId = requirementStatusDetail == true ? "Active" : "InActive";
                    //requirementListModel.RequirementStatusName = requirementStatusDetail != null ? requirementStatusDetail.RequirementStatusName : "";
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

                response.Status = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Success!";
                response.Data = requirementListResModel;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> AddEditRequirement(int RequirementId)
        {
            CommonResponse response = new CommonResponse();
            AddEditRequirementResModel addEditRequirementResModel = new AddEditRequirementResModel();
            try
            {
                AddEditRequirementDetail addEditRequirementDetail = new AddEditRequirementDetail();
                var requirementDetail = await _dbRepo.RequirementList().FirstOrDefaultAsync(x => x.Id == RequirementId);
                if (requirementDetail != null)
                {
                    //Edit Mode
                    addEditRequirementDetail.RequirementId = requirementDetail.Id;
                    addEditRequirementDetail.RequirementForId = requirementDetail.RequirementForId;
                    addEditRequirementDetail.ClientId = requirementDetail.ClientId;
                    addEditRequirementDetail.JobCode = requirementDetail.JobCode;
                    addEditRequirementDetail.MainSkill = requirementDetail.MainSkill;
                    addEditRequirementDetail.NoOfPosition = requirementDetail.NoOfPosition;
                    addEditRequirementDetail.Location = requirementDetail.Location;
                    addEditRequirementDetail.EndClient = requirementDetail.EndClient;
                    addEditRequirementDetail.TotalMinExperience = requirementDetail.TotalMinExperience;
                    addEditRequirementDetail.TotalMaxExperience = requirementDetail.TotalMaxExperience;
                    addEditRequirementDetail.RelevantMinExperience = requirementDetail.RelevantMinExperience;
                    addEditRequirementDetail.RelevantMaxExperience = requirementDetail.RelevantMaxExperience;
                    addEditRequirementDetail.ClientBillRate = requirementDetail.ClientBillRate;
                    addEditRequirementDetail.CandidatePayRate = requirementDetail.CandidatePayRate;
                    addEditRequirementDetail.PositionTypeId = requirementDetail.PositionTypeId;
                    addEditRequirementDetail.RequirementTypeId = requirementDetail.RequirementTypeId;
                    addEditRequirementDetail.EmploymentTypeId = requirementDetail.EmploymentTypeId;
                    addEditRequirementDetail.Pocname = requirementDetail.Pocname;
                    addEditRequirementDetail.MandatorySkills = requirementDetail.MandatorySkills;
                    addEditRequirementDetail.JobDescription = requirementDetail.JobDescription;
                    addEditRequirementDetail.AssignedUserIds = !string.IsNullOrWhiteSpace(requirementDetail.AssignedUserIds) ? requirementDetail.AssignedUserIds.Split(',').Select(int.Parse).ToList() : new List<int>();
                    addEditRequirementDetail.RequirementStatusId = requirementDetail.RequirementStatusId;
                    addEditRequirementDetail.IsActive = requirementDetail.IsActive;
                }
                addEditRequirementResModel.RequirementDetail = addEditRequirementDetail;

                addEditRequirementResModel.RequirementForList = await _dbRepo.RequirementForList().Select(x => new KeyValueModel { Id = x.Id, Name = x.RequirementForName }).ToListAsync();
                addEditRequirementResModel.ClientList = await _dbRepo.ClientList().Select(x => new KeyValueModel { Id = x.Id, Name = x.ClientName }).ToListAsync();
                addEditRequirementResModel.PositionTypeList = await _dbRepo.PositionTypeList().Select(x => new KeyValueModel { Id = x.Id, Name = x.PositionTypeName }).ToListAsync();
                addEditRequirementResModel.RequirementTypeList = await _dbRepo.RequirementTypeList().Select(x => new KeyValueModel { Id = x.Id, Name = x.RequirementTypeName }).ToListAsync();
                addEditRequirementResModel.EmploymentTypeList = await _dbRepo.EmploymentTypeList().Select(x => new KeyValueModel { Id = x.Id, Name = x.EmploymentTypeName }).ToListAsync();
                addEditRequirementResModel.RequirementStatusList = await _dbRepo.RequirementStatusList().Select(x => new KeyValueModel { Id = x.Id, Name = x.RequirementStatusName }).ToListAsync();
                addEditRequirementResModel.UserList = await _dbRepo.UserMstList().Select(x => new KeyValueModel { Id = x.Id, Name = x.FirstName + x.LastName }).ToListAsync();

                response.Data = addEditRequirementResModel;
                response.Message = "Success!";
                response.StatusCode = HttpStatusCode.OK;
                response.Status = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> SaveUpdateRequirement([FromBody] SaveUpdateRequirementReqModel saveUpdateRequirementReqModel)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                RequirementMst requirementMst = new RequirementMst();
                int loggedInUserId = _commonHelper.GetLoggedInUserId();
                DateTime currentDateTime = _commonHelper.GetCurrentDateTime();
                var requirementDetail = await _dbRepo.RequirementList().FirstOrDefaultAsync(x => x.Id == saveUpdateRequirementReqModel.RequirementId);
                if (requirementDetail != null)
                {
                    requirementMst = requirementDetail;
                }

                requirementMst.RequirementForId = saveUpdateRequirementReqModel.RequirementForId;
                requirementMst.ClientId = saveUpdateRequirementReqModel.ClientId;
                requirementMst.JobCode = saveUpdateRequirementReqModel.JobCode;
                requirementMst.MainSkill = saveUpdateRequirementReqModel.MainSkill;
                requirementMst.NoOfPosition = saveUpdateRequirementReqModel.NoOfPosition;
                requirementMst.Location = saveUpdateRequirementReqModel.Location;
                requirementMst.EndClient = saveUpdateRequirementReqModel.EndClient;
                requirementMst.TotalMinExperience = saveUpdateRequirementReqModel.TotalMinExperience;
                requirementMst.TotalMaxExperience = saveUpdateRequirementReqModel.TotalMaxExperience;
                requirementMst.RelevantMinExperience = saveUpdateRequirementReqModel.RelevantMinExperience;
                requirementMst.RelevantMaxExperience = saveUpdateRequirementReqModel.RelevantMaxExperience;
                requirementMst.ClientBillRate = saveUpdateRequirementReqModel.ClientBillRate;
                requirementMst.CandidatePayRate = saveUpdateRequirementReqModel.CandidatePayRate;
                requirementMst.PositionTypeId = saveUpdateRequirementReqModel.PositionTypeId;
                requirementMst.RequirementTypeId = saveUpdateRequirementReqModel.RequirementTypeId;
                requirementMst.EmploymentTypeId = saveUpdateRequirementReqModel.EmploymentTypeId;
                requirementMst.Pocname = saveUpdateRequirementReqModel.Pocname;
                requirementMst.MandatorySkills = saveUpdateRequirementReqModel.MandatorySkills;
                requirementMst.JobDescription = saveUpdateRequirementReqModel.JobDescription;
                requirementMst.AssignedUserIds = string.Join(",", saveUpdateRequirementReqModel.AssignedUserIds);
                requirementMst.RequirementStatusId = saveUpdateRequirementReqModel.RequirementStatusId;
                requirementMst.IsActive = saveUpdateRequirementReqModel.IsActive;

                if (requirementDetail != null)
                {
                    //Update Mode
                    requirementMst.UpdatedBy = loggedInUserId;
                    requirementMst.UpdatedDate = currentDateTime;

                    _dbContext.Entry(requirementMst).State = EntityState.Modified;
                    response.Message = "Requirement Updated Successfully!";
                }
                else
                {
                    //Save Mode
                    requirementMst.IsDelete = false;
                    requirementMst.CreatedBy = loggedInUserId;
                    requirementMst.UpdatedBy = loggedInUserId;
                    requirementMst.CreatedDate = currentDateTime;
                    requirementMst.UpdatedDate = currentDateTime;

                    await _dbContext.AddAsync(requirementMst);
                    response.Message = "Requirement Added Successfully!";
                }
                await _dbContext.SaveChangesAsync();

                response.Data = requirementMst.Id;
                response.StatusCode = HttpStatusCode.OK;
                response.Status = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Json(response);
        }

        public async Task<IActionResult> DeleteRequirement(int RequirementId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var requirementDetail = await _dbRepo.RequirementList().FirstOrDefaultAsync(x => x.Id == RequirementId);
                if (requirementDetail != null)
                {
                    RequirementMst requirementMst = new RequirementMst();
                    requirementMst = requirementDetail;
                    requirementMst.IsDelete = true;
                    requirementMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
                    requirementMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                    _dbContext.Entry(requirementMst).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();

                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Requirement deleted successfully!";
                    response.Data = requirementMst.Id;
                }
                else
                {
                    response.Message = "Data not found!";
                    response.StatusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Json(response);
        }

        public async Task<IActionResult> GetJobCode(int ClientId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                string jobCode = "";
                var clientDetail = await _dbRepo.ClientList().FirstOrDefaultAsync(x => x.Id == ClientId);
                int requirementCount = await _dbRepo.RequirementList().CountAsync() + 1;
                if (clientDetail != null)
                {
                    jobCode = jobCode + clientDetail.ClientName.ToUpper().Substring(0, 2) + "-" + requirementCount;
                }

                response.Status = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Success!";
                response.Data = jobCode;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Json(response);
        }

        public async Task<IActionResult> ChangeStatus(int RequirementId, int RequirementStatusId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var requirementDetail = await _dbRepo.RequirementList().FirstOrDefaultAsync(x => x.Id == RequirementId);
                if (requirementDetail != null)
                {
                    RequirementMst requirementMst = new RequirementMst();
                    requirementMst = requirementDetail;
                    requirementMst.RequirementStatusId = RequirementStatusId;
                    requirementMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
                    requirementMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                    _dbContext.Entry(requirementMst).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();

                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Requirement status updated successfully!";
                    response.Data = requirementMst.Id;
                }
                else
                {
                    response.Message = "Data not found!";
                    response.StatusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Json(response);
        }
    }
}
