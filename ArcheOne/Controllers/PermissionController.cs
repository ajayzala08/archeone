using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ArcheOne.Controllers
{
    public class PermissionController : Controller
    {
        private readonly ArcheOneDbContext _dbContext;
        private readonly DbRepo _dbRepo;
        public PermissionController(ArcheOneDbContext dbContext, DbRepo dbRepo)
        {
            _dbContext = dbContext;
            _dbRepo = dbRepo;
        }

        public IActionResult DefaultPermission()
        {
            return View();
        }

        public async Task<IActionResult> GetDefaultPermissionList(int RoleId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var data = await (from dpl in _dbRepo.DefaultPermissionList()
                                  where dpl.RoleId == RoleId
                                  join pm in _dbRepo.PermissionList() on dpl.PermissionId equals pm.Id into PM
                                  from final in PM.DefaultIfEmpty()
                                  select new
                                  {
                                      final.Id,
                                      final.PermissionName,
                                      final.PermissionCode
                                  }).ToListAsync();


                //var data = await _dbRepo.DefaultPermissionList().Where(x => x.RoleId == RoleId).ToListAsync();
                if (data != null && data.Count > 0)
                {
                    response.Status = true;
                    response.Message = "Data found successfully!";
                    response.StatusCode = HttpStatusCode.OK;
                    response.Data = data;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "Data not found!";
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
