using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

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

                int userId = _commonHelper.GetLoggedInUserId();

                var roleDetailsResponse = await new RoleController(_dbRepo).GetRoleByUserId(userId);

                dynamic roleDetails = null;
                if (roleDetailsResponse != null) { roleDetails = roleDetailsResponse.Data; }

                List<IndexDashboardResModel> permissionList;
                if (roleDetails!.RoleCode == CommonEnums.RoleMst.Super_Admin.ToString())
                {
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
                    var serializedPermissionList = JsonSerializer.Serialize(permissionList);
                    _httpContextAccessor.HttpContext.Session.SetString("PermissionList", serializedPermissionList);
                }


                #region DashboardShowAndHide
                //Preyansi Code
                DashboardDetailsResModel dashboardDetailsResModel = new DashboardDetailsResModel();
                CommonResponse departmentResponse = await new CommonController(_dbRepo, _dbContext, _commonHelper).GetDeparmentByUserId(userId);

                string departmentCode = string.Empty;

                if (departmentResponse.Status)
                {
                    departmentCode = departmentResponse.Data.DepartmentCode;
                    dashboardDetailsResModel.IsUserHR = departmentCode == CommonEnums.DepartmentMst.Human_Resource.ToString();
                    dashboardDetailsResModel.IsUserSD = departmentCode == CommonEnums.DepartmentMst.Software_Development.ToString();
                    dashboardDetailsResModel.IsUserQA = departmentCode == CommonEnums.DepartmentMst.Quality_Analyst.ToString();
                    dashboardDetailsResModel.IsUserDesigner = departmentCode == CommonEnums.DepartmentMst.Designer.ToString();
                    dashboardDetailsResModel.IsUserSales = departmentCode == CommonEnums.DepartmentMst.Sales.ToString();
                }
                var userListByReportingManagerId = await _dbRepo.UserDetailList().Where(x => x.ReportingManager == userId).ToListAsync();
                var projectList = _dbRepo.ProjectList().Select(x => new { x.Resources, x.ProjectStatus }).ToList();
                dashboardDetailsResModel.InterviewRoundCount = _dbRepo.InterviewList().Count();
                dashboardDetailsResModel.UserCount = _dbRepo.UserDetailList().Count();
                dashboardDetailsResModel.SalesLeadsCount = _dbRepo.SalesLeadList().Count();
                dashboardDetailsResModel.SalesLeadOpportunityCount = _dbRepo.salesLeadFollowUpMst().Where(x => x.SalesLeadStatusId == 5).Count();
                dashboardDetailsResModel.ClosureCount = _dbRepo.ResumeFileUploadDetailList().Where(x => x.ResumeStatus == 3).Count();
                dashboardDetailsResModel.SubmissionCount = _dbRepo.ResumeFileUploadDetailList().Where(x => x.ResumeStatus == 2).Count();
                dashboardDetailsResModel.BDCount = _dbRepo.InterviewList().Where(x => x.HireStatusId == 4).Count();
                dashboardDetailsResModel.InHouseRequirementCount = (from rfl in _dbRepo.RequirementForList()
                                                                    join rl in _dbRepo.RequirementList() on rfl.Id equals rl.RequirementForId into rls
                                                                    from rl in rls.DefaultIfEmpty()
                                                                    where rfl.RequirementForCode.ToLower() == "in_house"
                                                                    select rl).Count();
                dashboardDetailsResModel.ClientRequirementCount = (from rfl in _dbRepo.RequirementForList()
                                                                   join rl in _dbRepo.RequirementList() on rfl.Id equals rl.RequirementForId into rls
                                                                   from rl in rls.DefaultIfEmpty()
                                                                   where rfl.RequirementForCode.ToLower() == "for_client"
                                                                   select rl).Count();
                dashboardDetailsResModel.ActiveRequirementCount = (from rfl in _dbRepo.RequirementStatusList()
                                                                   join rl in _dbRepo.RequirementList() on rfl.Id equals rl.RequirementStatusId into rls
                                                                   from rl in rls.DefaultIfEmpty()
                                                                   where rfl.RequirementStatusCode.ToLower() == "active"
                                                                   select rl).Count();
                dashboardDetailsResModel.InActiveRequirementCount = (from rfl in _dbRepo.RequirementStatusList()
                                                                     join rl in _dbRepo.RequirementList() on rfl.Id equals rl.RequirementStatusId into rls
                                                                     from rl in rls.DefaultIfEmpty()
                                                                     where rfl.RequirementStatusCode.ToLower() == "in_active"
                                                                     select rl).Count();
                dashboardDetailsResModel.OnHoldRequirementCount = (from rfl in _dbRepo.RequirementStatusList()
                                                                   join rl in _dbRepo.RequirementList() on rfl.Id equals rl.RequirementStatusId into rls
                                                                   from rl in rls.DefaultIfEmpty()
                                                                   where rfl.RequirementStatusCode.ToLower() == "on_hold"
                                                                   select rl).Count();
                dashboardDetailsResModel.CloseRequirementCount = (from rfl in _dbRepo.RequirementStatusList()
                                                                  join rl in _dbRepo.RequirementList() on rfl.Id equals rl.RequirementStatusId into rls
                                                                  from rl in rls.DefaultIfEmpty()
                                                                  where rfl.RequirementStatusCode.ToLower() == "closed"
                                                                  select rl).Count();
                dashboardDetailsResModel.TeamCount = (from TL in _dbRepo.TeamList()
                                                      join UM in _dbRepo.UserMstList() on TL.TeamLeadId equals UM.Id
                                                      select TL).Select(TL => TL.TeamLeadId).Distinct().Count();


                #region NewChanges 

                dashboardDetailsResModel.SalesLeadNewCount = (from SlF in _dbRepo.salesLeadFollowUpMst()
                                                              join rl in _dbRepo.SalesLeadStatusList().Where(x => x.SalesLeadStatusName.ToLower() == "new") on SlF.SalesLeadStatusId equals rl.Id
                                                              select SlF).Count();
                dashboardDetailsResModel.NextLevelFollowUpCount = _dbRepo.salesLeadFollowUpMst().Where(x => x.NextFollowUpDateTime == DateTime.Now.Date).Count();
                dashboardDetailsResModel.TotalRequirmentCount = _dbRepo.RequirementList().Count();
                dashboardDetailsResModel.OfferCount = _dbRepo.InterviewList().Where(x => x.OfferStatusId == 2).Count();
                //dashboardDetailsResModel.JoiningCount
                //dashboardDetailsResModel.TaskReportCount
                //dashboardDetailsResModel.AppraisalRatingCompletedCount
                //dashboardDetailsResModel.AppraisalRatingInprogressCount
                //dashboardDetailsResModel.RecentJoiningCount
                //dashboardDetailsResModel.PerviousDayTaskCount



                #endregion




























                if (dashboardDetailsResModel.IsUserHR)
                {


                    dashboardDetailsResModel.ProjectCount = projectList.Count;
                    dashboardDetailsResModel.ProjectCompletedCount = projectList.Where(x => x.ProjectStatus.ToLower() == "completed").Count();
                    dashboardDetailsResModel.ProjectInProgressCount = projectList.Where(x => x.ProjectStatus.ToLower() == "InProgress").Count();
                    dashboardDetailsResModel.ProjectToDoCount = projectList.Where(x => x.ProjectStatus.ToLower() == "ToDo").Count();
                    dashboardDetailsResModel.UncheckedLeave = _dbRepo.LeaveLists().Where(x => x.ApprovedByReportingStatus == null || x.ApprovedByReportingStatus == 0).Count();
                    dashboardDetailsResModel.PendingResumeApprovalCount = _dbRepo.ResumeFileUploadDetailList().Where(x => x.ResumeStatus == 1).Count();

                }
                else if (userListByReportingManagerId.Count > 0)
                {
                    foreach (var item in userListByReportingManagerId)
                    {
                        dashboardDetailsResModel.UncheckedLeave = _dbRepo.LeaveLists().Where(x => (x.ApprovedByReportingStatus == null || x.ApprovedByReportingStatus == 0) && (x.AppliedByUserId == userId) || x.AppliedByUserId == item.UserId).Count();
                    }
                }
                else
                {
                    dashboardDetailsResModel.ProjectCount = projectList.Where(x => x.Resources == Convert.ToString(userId)).Count();
                    dashboardDetailsResModel.ProjectCompletedCount = projectList.Where(x => x.ProjectStatus.ToLower() == "completed" && x.Resources == Convert.ToString(userId)).Count();
                    dashboardDetailsResModel.ProjectInProgressCount = projectList.Where(x => x.ProjectStatus.ToLower() == "InProgress" && x.Resources == Convert.ToString(userId)).Count();
                    dashboardDetailsResModel.ProjectToDoCount = projectList.Where(x => x.ProjectStatus.ToLower() == "ToDo" && x.Resources == Convert.ToString(userId)).Count();
                    //dashboardDetailsResModel.TeamCount = (from TL in _dbRepo.TeamList()
                    //                                      join UM in _dbRepo.UserMstList() on TL.TeamLeadId equals UM.Id
                    //                                      select TL).Select(TL => TL.TeamLeadId).Distinct().Count();
                    dashboardDetailsResModel.UncheckedLeave = _dbRepo.LeaveLists().Where(x => (x.ApprovedByReportingStatus == null || x.ApprovedByReportingStatus == 0) && (x.AppliedByUserId == userId)).Count();
                    dashboardDetailsResModel.PendingResumeApprovalCount = _dbRepo.ResumeFileUploadDetailList().Where(x => x.ResumeStatus == 1).Count();
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


    }
}
