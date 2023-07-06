using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                    byte[] serializedData;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        JsonSerializer.Serialize(memoryStream, permissionList);
                        serializedData = memoryStream.ToArray();
                    }
                    _httpContextAccessor.HttpContext.Session.Set("PermissionList", serializedData);
                }
            }
            catch (Exception ex) { }

            return View();
        }
    }
}
