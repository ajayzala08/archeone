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
        public DashboardController(CommonHelper commonHelper, DbRepo dbRepo, IHttpContextAccessor httpContextAccessor)
        {
            _commonHelper = commonHelper;
            _dbRepo = dbRepo;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
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
            }
            catch (Exception) { }

            return View();
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
                                         EmployeeImagePath = x.userMsts.PhotoUrl,
                                         EmployeeName = $"{x.userMsts.FirstName} {x.userMsts.LastName}",
                                         JoinDate = x.userDetails.JoinDate.ToString("M")
                                     }).ToListAsync();
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
