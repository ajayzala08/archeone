using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ArcheOne.Controllers
{
    public class DashboardController : Controller
    {
        private readonly CommonHelper _commonHelper;
        private readonly DbRepo _dbRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ArcheOneDbContext _dbContext;
        public DashboardController(CommonHelper commonHelper, DbRepo dbRepo, IHttpContextAccessor httpContextAccessor, ArcheOneDbContext dbContext)
        {
            _commonHelper = commonHelper;
            _dbRepo = dbRepo;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                DashboardDetailsResModel dashboardDetailsResModel = new DashboardDetailsResModel();
                int userId = _commonHelper.GetLoggedInUserId();
                var roleDetailsResponse = await new RoleController(_dbRepo).GetRoleByUserId(userId);
                dynamic roleDetails = null;
                if (roleDetailsResponse != null) { roleDetails = roleDetailsResponse.Data; }

                List<IndexDashboardResModel> permissionList;
                if (roleDetails!.RoleCode == CommonEnums.RoleMst.Super_Admin.ToString())
                {
                    dashboardDetailsResModel.ISSuperAdmin = true;
                    permissionList = await _dbRepo.PermissionList().Select(x => new IndexDashboardResModel { PermissionCode = x.PermissionCode, PermissionRoute = x.PermissionRoute }).ToListAsync();
                }
                else
                {
                    permissionList = await (from userPermission in _dbRepo.UserPermissionList()
                                            where userPermission.UserId == userId
                                            join permissions in _dbRepo.PermissionList() on userPermission.PermissionId equals permissions.Id into permissionsGroup
                                            from permissionsItem in permissionsGroup.DefaultIfEmpty()
                                            select new IndexDashboardResModel
                                            {
                                                PermissionCode = permissionsItem.PermissionCode ?? "",
                                                PermissionRoute = permissionsItem.PermissionRoute ?? ""
                                            }).ToListAsync();
                }

                if (permissionList.Count > 0)
                {
                    var serializedPermissionList = System.Text.Json.JsonSerializer.Serialize(permissionList);
                    _httpContextAccessor.HttpContext.Session.SetString("PermissionList", serializedPermissionList);
                }

                #region DashboardShowAndHide
                //Preyansi Code
                CommonResponse departmentResponse = await new CommonController(_dbRepo, _dbContext, _commonHelper).GetDepartmentByUserId(userId);
                string departmentCode = string.Empty;
                if (departmentResponse.Status)
                {
                    departmentCode = departmentResponse.Data.DepartmentCode;

                    dashboardDetailsResModel.ISUserAdmin = departmentCode == CommonEnums.DepartmentMst.Administration.ToString();
                    dashboardDetailsResModel.IsUserHR = departmentCode == CommonEnums.DepartmentMst.Human_Resource.ToString();
                    dashboardDetailsResModel.IsUserSD = departmentCode == CommonEnums.DepartmentMst.Software_Development.ToString();
                    dashboardDetailsResModel.IsUserQA = departmentCode == CommonEnums.DepartmentMst.Quality_Analyst.ToString();
                    dashboardDetailsResModel.IsUserDesigner = departmentCode == CommonEnums.DepartmentMst.Designer.ToString();
                    dashboardDetailsResModel.IsUserSales = departmentCode == CommonEnums.DepartmentMst.Sales.ToString();
                    dashboardDetailsResModel.IsUserRecruitment = departmentCode == CommonEnums.DepartmentMst.Recruitment.ToString();
                }
                #region DashBoardCount
                var userListByReportingManagerId = await _dbRepo.UserDetailList().Where(x => x.ReportingManager == userId).ToListAsync();
                var projectList = _dbRepo.ProjectList().Select(x => new { x.Resources, x.ProjectStatus }).ToList();
                dashboardDetailsResModel.InterviewRoundCount = _dbRepo.InterviewList().Count();
                dashboardDetailsResModel.UserCount = _dbRepo.UserDetailList().Count();
                dashboardDetailsResModel.SalesLeadsCount = _dbRepo.SalesLeadList().Count();
                dashboardDetailsResModel.SalesLeadOpportunityCount = _dbRepo.salesLeadFollowUpMst().Where(x => x.SalesLeadStatusId == 5).Count();
                dashboardDetailsResModel.ClosureCount = _dbRepo.ResumeFileUploadDetailList().Where(x => x.ResumeStatus == 3).Count();
                dashboardDetailsResModel.SubmissionCount = _dbRepo.ResumeFileUploadDetailList().Where(x => x.ResumeStatus == 2).Count();
                dashboardDetailsResModel.BDCount = _dbRepo.InterviewList().Where(x => x.HireStatusId == 4).Count();
                dashboardDetailsResModel.InHouseRequirementCount = (from rfl1 in _dbRepo.RequirementForList().Where(x => x.RequirementForCode.ToLower() == "in_house")
                                                                    join rl1 in _dbRepo.RequirementList() on rfl1.Id equals rl1.RequirementForId
                                                                    select rl1).Count();
                dashboardDetailsResModel.ClientRequirementCount = (from rfl in _dbRepo.RequirementForList().Where(x => x.RequirementForCode.ToLower() == "for_client")
                                                                   join rl in _dbRepo.RequirementList() on rfl.Id equals rl.RequirementForId
                                                                   select rl).Count();
                dashboardDetailsResModel.ActiveRequirementCount = (from rfl2 in _dbRepo.RequirementStatusList().Where(x => x.RequirementStatusCode.ToLower() == "active")
                                                                   join rl2 in _dbRepo.RequirementList() on rfl2.Id equals rl2.RequirementStatusId
                                                                   select rl2).Count();

                dashboardDetailsResModel.InActiveRequirementCount = (from rfl3 in _dbRepo.RequirementStatusList().Where(x => x.RequirementStatusCode.ToLower() == "in_active")
                                                                     join rl3 in _dbRepo.RequirementList() on rfl3.Id equals rl3.RequirementStatusId
                                                                     select rl3).Count();

                dashboardDetailsResModel.OnHoldRequirementCount = (from rfl4 in _dbRepo.RequirementStatusList().Where(x => x.RequirementStatusCode.ToLower() == "on_hold")
                                                                   join rl4 in _dbRepo.RequirementList() on rfl4.Id equals rl4.RequirementStatusId
                                                                   select rl4).Count();

                dashboardDetailsResModel.CloseRequirementCount = (from rfl5 in _dbRepo.RequirementStatusList().Where(x => x.RequirementStatusCode.ToLower() == "closed")
                                                                  join rl5 in _dbRepo.RequirementList() on rfl5.Id equals rl5.RequirementStatusId
                                                                  select rl5).Count();
                dashboardDetailsResModel.TeamCount = (from TL in _dbRepo.TeamList()
                                                      join UM in _dbRepo.UserMstList() on TL.TeamLeadId equals UM.Id
                                                      select TL).Select(TL => TL.TeamLeadId).Distinct().Count();
                #endregion

                dashboardDetailsResModel.SalesLeadNewCount = (from SlF in _dbRepo.salesLeadFollowUpMst()
                                                              join rl in _dbRepo.SalesLeadStatusList().Where(x => x.SalesLeadStatusName.ToLower() == "new") on SlF.SalesLeadStatusId equals rl.Id
                                                              select SlF).Count();
                dashboardDetailsResModel.NextFollowUpCount = _dbRepo.salesLeadFollowUpMst().Where(x => x.NextFollowUpDateTime == DateTime.Now.Date).Count();
                dashboardDetailsResModel.TotalRequirementCount = _dbRepo.RequirementList().Count();
                dashboardDetailsResModel.OfferCount = _dbRepo.InterviewList().Where(x => x.OfferStatusId == 2).Count();
                dashboardDetailsResModel.JoiningCount = _dbRepo.InterviewList().Where(x => x.HireStatusId == 1).Count();
                dashboardDetailsResModel.AppraisalRatingCompletedCount = _dbRepo.AppraisalList().Where(x => x.IsApprove == true).Count();
                dashboardDetailsResModel.AppraisalRatingInprogressCount = _dbRepo.AppraisalList().Where(x => x.IsApprove == false).Count();
                dashboardDetailsResModel.RecentJoiningCount = _dbRepo.UserDetailList().Where(x => x.JoinDate.Date <= DateTime.Now.AddMonths(-1).Date).Count();


                dashboardDetailsResModel.SalesLeadInProgressCount = (from SlF in _dbRepo.salesLeadFollowUpMst()
                                                                     join rl in _dbRepo.SalesLeadStatusList().Where(x => x.SalesLeadStatusName.ToLower() == "inprogress") on SlF.SalesLeadStatusId equals rl.Id
                                                                     select SlF).Count();
                dashboardDetailsResModel.SalesLeadDNCCount = (from SlF in _dbRepo.salesLeadFollowUpMst()
                                                              join rl in _dbRepo.SalesLeadStatusList().Where(x => x.SalesLeadStatusName.ToLower() == "dnc") on SlF.SalesLeadStatusId equals rl.Id
                                                              select SlF).Count();
                dashboardDetailsResModel.SalesLeadNotInterestedCount = (from SlF in _dbRepo.salesLeadFollowUpMst()
                                                                        join rl in _dbRepo.SalesLeadStatusList().Where(x => x.SalesLeadStatusName.ToLower() == "notinterested") on SlF.SalesLeadStatusId equals rl.Id
                                                                        select SlF).Count();
                dynamic roledata222 = roleDetailsResponse.Data;

                if (dashboardDetailsResModel.IsUserSD || dashboardDetailsResModel.IsUserQA || dashboardDetailsResModel.IsUserDesigner)
                {
                    dashboardDetailsResModel.PerviousDayTaskCount = (from DTL in _dbRepo.DailyTaskList().Where(x => x.CreatedBy == userId)
                                                                     join UML in _dbRepo.UserMstList() on DTL.CreatedBy equals UML.Id
                                                                     select new { DTL, UML }).Select(x => new TaskDetails
                                                                     {
                                                                         Task = x.DTL.TaskDescription,
                                                                         UserName = x.UML.FirstName + " " + x.UML.LastName,
                                                                         Date = x.DTL.TaskDate.ToString("dd MMM.yyyy"),
                                                                     }).ToList();
                }

                if (dashboardDetailsResModel.IsUserHR || dashboardDetailsResModel.IsUserRecruitment)
                {
                    dashboardDetailsResModel.ProjectCount = projectList.Count;
                    dashboardDetailsResModel.ProjectCompletedCount = projectList.Where(x => x.ProjectStatus.ToLower() == "completed").Count();
                    dashboardDetailsResModel.ProjectInProgressCount = projectList.Where(x => x.ProjectStatus.ToLower() == "inprogress").Count();
                    dashboardDetailsResModel.ProjectToDoCount = projectList.Where(x => x.ProjectStatus.ToLower() == "todo").Count();
                    dashboardDetailsResModel.UncheckedLeave = _dbRepo.LeaveLists().Where(x => x.ApprovedByReportingStatus == null || x.ApprovedByReportingStatus == 0 || x.Hrstatus == null || x.Hrstatus == 0).Count();
                    dashboardDetailsResModel.PendingResumeApprovalCount = _dbRepo.ResumeFileUploadDetailList().Where(x => x.ResumeStatus == 1).Count();

                }
                else if (userListByReportingManagerId.Count > 0)
                {
                    foreach (var item in userListByReportingManagerId)
                    {
                        dashboardDetailsResModel.UncheckedLeave = _dbRepo.LeaveLists().Where(x => (x.ApprovedByReportingStatus == null || x.ApprovedByReportingStatus == 0 || x.Hrstatus == null || x.Hrstatus == 0) && (x.AppliedByUserId == userId) || x.AppliedByUserId == item.UserId).Count();
                    }
                }
                else
                {
                    var ProjectList = projectList.Select(x => new { Resources = x.Resources.Split(',').Select(id => id.Trim()), ProjectStatus = x.ProjectStatus }).ToList();

                    var ProjectCompletedCount = ProjectList.Where(x => x.Resources.Any(y => y == userId.ToString()) && x.ProjectStatus.ToLower() == "completed").Count();
                    var ProjectInProgressCount = ProjectList.Where(x => x.Resources.Any(y => y == userId.ToString()) && x.ProjectStatus.ToLower() == "inprogress").Count();
                    var ProjectToDoCount = ProjectList.Where(x => x.Resources.Any(y => y == userId.ToString()) && x.ProjectStatus.ToLower() == "todo").Count();

                    dashboardDetailsResModel.ProjectCount = ProjectList.Count;
                    dashboardDetailsResModel.ProjectCompletedCount = ProjectCompletedCount;
                    dashboardDetailsResModel.ProjectInProgressCount = ProjectInProgressCount;
                    dashboardDetailsResModel.ProjectToDoCount = ProjectToDoCount;
                    dashboardDetailsResModel.UncheckedLeave = _dbRepo.LeaveLists().Where(x => (x.ApprovedByReportingStatus == null || x.ApprovedByReportingStatus == 0) && (x.AppliedByUserId == userId)).Count();
                    dashboardDetailsResModel.PendingResumeApprovalCount = _dbRepo.ResumeFileUploadDetailList().Where(x => x.ResumeStatus == 1).Count();
                    dashboardDetailsResModel.AppraisalRatingCompletedCount = _dbRepo.AppraisalList().Where(x => x.IsApprove == true && x.EmployeeId == userId).Count();
                    dashboardDetailsResModel.AppraisalRatingInprogressCount = _dbRepo.AppraisalList().Where(x => x.IsApprove == false && x.EmployeeId == userId).Count();
                }

                #endregion

                if (dashboardDetailsResModel != null)
                {
                    commonResponse.Status = true;
                    commonResponse.Data = dashboardDetailsResModel;
                }
            }
            catch (Exception) { }

            return View(commonResponse);
        }

        public async Task<IActionResult> GetBirthdayWorkAniversaryHoliday()
        {
            CommonResponse commonResponse = new CommonResponse();
            GetBirthdayWorkAnniversaryHolidayResModel anniversaryHolidayResModel = new();
            try
            {
                anniversaryHolidayResModel.WorkAnniversaries = await WorkAnniversaries();
                anniversaryHolidayResModel.Birthdays = await Birthdays();
                anniversaryHolidayResModel.Holidays = await Holidays();


                commonResponse.Status = true;
                commonResponse.Message = "Data found successfully";
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Data = anniversaryHolidayResModel;
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message.ToString();
                commonResponse.Data = ex.ToString();

            }
            return Json(commonResponse);
        }

        private async Task<List<WorkAnniversary>> WorkAnniversaries()
        {
            List<WorkAnniversary> workAnniversaries = new List<WorkAnniversary>();
            workAnniversaries = await (from userDetails in _dbRepo.UserDetailList()
                                       where userDetails.JoinDate.Month == _commonHelper.GetCurrentDateTime().Month
                                       join userMsts in _dbRepo.AllUserMstList() on userDetails.UserId equals userMsts.Id
                                       select new { userDetails, userMsts }
                                     ).Select(x => new WorkAnniversary
                                     {

                                         EmployeeName = $"{x.userMsts.FirstName} {x.userMsts.LastName}",
                                         JoinDate = x.userDetails.JoinDate.ToString("M"),
                                         EmployeeImagePath = System.IO.File.Exists(Path.Combine(_commonHelper.GetPhysicalRootPath(false), x.userMsts.PhotoUrl)) ? Path.Combine(@"\", x.userMsts.PhotoUrl) : Path.Combine(_commonHelper.GetPhysicalRootPath(false), @"\Theme\Logo\UserDefault.jpg"),
                                     }).ToListAsync();


            //EmployeeImagePath = x.userMsts.PhotoUrl != "" ? Path.Combine(@"\", x.userMsts.PhotoUrl) :
            //@"\Theme\Logo\default_user_profile.png",

            return workAnniversaries;
        }

        private async Task<List<Birthday>> Birthdays()
        {
            List<Birthday> birthdays = new List<Birthday>();
            birthdays = await (from userDetails in _dbRepo.UserDetailList()
                               where userDetails.Dob.Month == _commonHelper.GetCurrentDateTime().Month
                               join userMsts in _dbRepo.AllUserMstList() on userDetails.UserId equals userMsts.Id
                               select new { userDetails, userMsts }
                                     ).Select(x => new Birthday
                                     {
                                         EmployeeImagePath = System.IO.File.Exists(Path.Combine(_commonHelper.GetPhysicalRootPath(false), x.userMsts.PhotoUrl)) ? Path.Combine(@"\", x.userMsts.PhotoUrl) : Path.Combine(_commonHelper.GetPhysicalRootPath(false), @"\Theme\Logo\UserDefault.jpg"),
                                         EmployeeName = $"{x.userMsts.FirstName} {x.userMsts.LastName}",
                                         Birthdate = x.userDetails.Dob.ToString("M")
                                     }).ToListAsync();
            return birthdays;
        }

        private async Task<List<Holiday>> Holidays()
        {
            List<Holiday> holidays = new List<Holiday>();
            holidays = await _dbRepo.HolidayDayList().Where(x => x.HolidayDate.Month == _commonHelper.GetCurrentDateTime().Month).Select(x => new Holiday
            {
                HolidayName = x.HolidayName,
                HolidayDate = x.HolidayDate.ToString("M")
            }).ToListAsync();
            return holidays;
        }
        [HttpGet]
        public async Task<IActionResult> HRChart()
        {
            CommonResponse commonResponse = new CommonResponse();
            DashboardDetailsResModel dashboardDetailsResModel = new DashboardDetailsResModel();
            dashboardDetailsResModel.InHouseRequirementCount = (from rfl1 in _dbRepo.RequirementForList().Where(x => x.RequirementForCode.ToLower() == "in_house")
                                                                join rl1 in _dbRepo.RequirementList() on rfl1.Id equals rl1.RequirementForId
                                                                select rl1).Count();
            dashboardDetailsResModel.ClientRequirementCount = (from rfl in _dbRepo.RequirementForList().Where(x => x.RequirementForCode.ToLower() == "for_client")
                                                               join rl in _dbRepo.RequirementList() on rfl.Id equals rl.RequirementForId
                                                               select rl).Count();
            dashboardDetailsResModel.ActiveRequirementCount = (from rfl2 in _dbRepo.RequirementStatusList().Where(x => x.RequirementStatusCode.ToLower() == "active")
                                                               join rl2 in _dbRepo.RequirementList() on rfl2.Id equals rl2.RequirementStatusId
                                                               select rl2).Count();

            dashboardDetailsResModel.InActiveRequirementCount = (from rfl3 in _dbRepo.RequirementStatusList().Where(x => x.RequirementStatusCode.ToLower() == "in_active")
                                                                 join rl3 in _dbRepo.RequirementList() on rfl3.Id equals rl3.RequirementStatusId
                                                                 select rl3).Count();

            dashboardDetailsResModel.OnHoldRequirementCount = (from rfl4 in _dbRepo.RequirementStatusList().Where(x => x.RequirementStatusCode.ToLower() == "on_hold")
                                                               join rl4 in _dbRepo.RequirementList() on rfl4.Id equals rl4.RequirementStatusId
                                                               select rl4).Count();

            dashboardDetailsResModel.CloseRequirementCount = (from rfl5 in _dbRepo.RequirementStatusList().Where(x => x.RequirementStatusCode.ToLower() == "closed")
                                                              join rl5 in _dbRepo.RequirementList() on rfl5.Id equals rl5.RequirementStatusId
                                                              select rl5).Count();

            dashboardDetailsResModel.TotalRequirementCount = _dbRepo.RequirementList().Count();
            List<DataPoint> dataPoints = new List<DataPoint>()
            {
                new DataPoint { LegendText = "Total Requirement",Label = "Total Requirement", Y = dashboardDetailsResModel.TotalRequirementCount},
                new DataPoint { LegendText = "Active Requirement", Label = "Active Requirement", Y = dashboardDetailsResModel.ActiveRequirementCount},
                new DataPoint { LegendText = "InActive Requirement", Label = "InActive Requirement", Y = dashboardDetailsResModel.InActiveRequirementCount},
                new DataPoint { LegendText = "Client Requirement", Label = "Client Requirement", Y = dashboardDetailsResModel.ClientRequirementCount},
                new DataPoint { LegendText = "Close Requirement", Label = "Close Requirement", Y = dashboardDetailsResModel.CloseRequirementCount},
                new DataPoint { LegendText = "InHouse Requirement", Label = "InHouse Requirement", Y = dashboardDetailsResModel.InHouseRequirementCount},
                new DataPoint { LegendText = "OnHold Requirement", Label = "OnHold Requirement", Y = dashboardDetailsResModel.OnHoldRequirementCount},
            };


            commonResponse.Data = dataPoints;
            commonResponse.Status = true;
            return Json(commonResponse);
        }

        [HttpGet]
        public async Task<IActionResult> SDChart()
        {
            CommonResponse commonResponse = new CommonResponse();
            DashboardDetailsResModel dashboardDetailsResModel = new DashboardDetailsResModel();
            var projectList = await _dbRepo.ProjectList().Select(x => new { x.Resources, x.ProjectStatus }).ToListAsync();
            dashboardDetailsResModel.ProjectCount = projectList.Count;
            dashboardDetailsResModel.ProjectCompletedCount = projectList.Where(x => x.ProjectStatus.ToLower() == "completed").Count();
            dashboardDetailsResModel.ProjectInProgressCount = projectList.Where(x => x.ProjectStatus.ToLower() == "inprogress").Count();
            dashboardDetailsResModel.ProjectToDoCount = projectList.Where(x => x.ProjectStatus.ToLower() == "todo").Count();
            List<DataPoint> dataPoints = new List<DataPoint>()
            {
                new DataPoint { LegendText = "Project",Label = "Project", Y = dashboardDetailsResModel.ProjectCount},
                new DataPoint { LegendText = "Project Completed", Label = "Project Completed", Y = dashboardDetailsResModel.ProjectCompletedCount},
                new DataPoint { LegendText = "Project ToDo", Label = "Project ToDo", Y = dashboardDetailsResModel.ProjectToDoCount},
                new DataPoint { LegendText = "Project InProgress", Label = "Project InProgress", Y = dashboardDetailsResModel.ProjectInProgressCount},
            };


            commonResponse.Data = dataPoints;
            commonResponse.Status = true;
            return Json(commonResponse);
        }

        [HttpGet]
        public async Task<IActionResult> SalesChart()
        {
            CommonResponse commonResponse = new CommonResponse();
            DashboardDetailsResModel dashboardDetailsResModel = new DashboardDetailsResModel();
            dashboardDetailsResModel.SalesLeadInProgressCount = (from SlF in _dbRepo.salesLeadFollowUpMst()
                                                                 join rl in _dbRepo.SalesLeadStatusList().Where(x => x.SalesLeadStatusName.ToLower() == "inprogress") on SlF.SalesLeadStatusId equals rl.Id
                                                                 select SlF).Count();
            dashboardDetailsResModel.SalesLeadDNCCount = (from SlF in _dbRepo.salesLeadFollowUpMst()
                                                          join rl in _dbRepo.SalesLeadStatusList().Where(x => x.SalesLeadStatusName.ToLower() == "dnc") on SlF.SalesLeadStatusId equals rl.Id
                                                          select SlF).Count();
            dashboardDetailsResModel.SalesLeadNotInterestedCount = (from SlF in _dbRepo.salesLeadFollowUpMst()
                                                                    join rl in _dbRepo.SalesLeadStatusList().Where(x => x.SalesLeadStatusName.ToLower() == "notinterested") on SlF.SalesLeadStatusId equals rl.Id
                                                                    select SlF).Count();

            dashboardDetailsResModel.SalesLeadNewCount = (from SlF in _dbRepo.salesLeadFollowUpMst()
                                                          join rl in _dbRepo.SalesLeadStatusList().Where(x => x.SalesLeadStatusName.ToLower() == "new") on SlF.SalesLeadStatusId equals rl.Id
                                                          select SlF).Count();
            dashboardDetailsResModel.SalesLeadOpportunityCount = _dbRepo.salesLeadFollowUpMst().Where(x => x.SalesLeadStatusId == 5).Count();
            List<DataPoint> dataPoints = new List<DataPoint>()
            {
            new DataPoint { LegendText = "Opportunity SalesLead", Label = "Opportunity SalesLead", Y = dashboardDetailsResModel.SalesLeadOpportunityCount},
                new DataPoint { LegendText = "InProgress SalesLead", Label = "InProgress SalesLead", Y = dashboardDetailsResModel.SalesLeadInProgressCount},
                new DataPoint { LegendText = "NotInterested SalesLead", Label = "NotInterested SalesLead", Y = dashboardDetailsResModel.SalesLeadNotInterestedCount},
                new DataPoint { LegendText = "DNC", Label = "DNC", Y = dashboardDetailsResModel.SalesLeadDNCCount},
                new DataPoint { LegendText = "New SalesLead", Label = "New SalesLead", Y = dashboardDetailsResModel.SalesLeadNewCount},
            };


            commonResponse.Data = dataPoints;
            commonResponse.Status = true;
            return Json(commonResponse);
        }

        [HttpGet]
        public IActionResult CalenderPopupData(string name)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                GetAllEventResModel getAllEventResModel = new GetAllEventResModel();
                var list = _dbRepo.EventList().FirstOrDefault(x => x.Subject == name);
                if (list != null)
                {
                    getAllEventResModel.title = list.Subject;
                    getAllEventResModel.description = list.Description;
                    getAllEventResModel.start = list.StartDate.Value;
                    getAllEventResModel.end = list.EndDate.Value;
                    getAllEventResModel.color = list.ThemeColour;
                    getAllEventResModel.allDay = list.IsFullDay.Value;
                }
                commonResponse.Status = true;
                commonResponse.Message = "Success!";
                commonResponse.Data = getAllEventResModel;

            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Data = ex.StackTrace;
            }
            return Json(commonResponse);
        }

    }
}
