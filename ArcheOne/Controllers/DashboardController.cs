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
            int userId = _commonHelper.GetLoggedInUserId();

            var roleDetails = await (from userMst in _dbRepo.AllUserMstList()
                                     where userMst.Id == userId
                                     join roleMst in _dbRepo.RoleMstList(withSuperAdmin: true) on userMst.RoleId equals roleMst.Id into roleMstGroup
                                     from roleMstItem in roleMstGroup.DefaultIfEmpty()
                                     select new
                                     {
                                         roleMstItem.Id,
                                         roleMstItem.RoleCode,
                                         roleMstItem.RoleName
                                     }).FirstOrDefaultAsync();

            List<IndexDashboardResModel> permissionList;
            if (roleDetails!.RoleCode == CommonEnums.RoleMst.Super_Admin.ToString())
            {
                permissionList = await _dbRepo.PermissionList().Select(x => new IndexDashboardResModel { Id = x.Id, PermissionName = x.PermissionName, PermissionCode = x.PermissionCode }).ToListAsync();
            }
            else
            {
                permissionList = await (from userPermission in _dbRepo.UserPermissionList()
                                        where userPermission.UserId == userId
                                        join permissions in _dbRepo.PermissionList() on userPermission.PermissionId equals permissions.Id into permissionsGroup
                                        from permissionsItem in permissionsGroup.DefaultIfEmpty()
                                        select new IndexDashboardResModel
                                        {
                                            Id = permissionsItem.Id,
                                            PermissionName = permissionsItem.PermissionName,
                                            PermissionCode = permissionsItem.PermissionCode,
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

            return View();
        }
    }
}
