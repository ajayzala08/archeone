using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArcheOne.Controllers
{
    public class RoleController : Controller
    {
        private readonly DbRepo _dbRepo;
        public RoleController(DbRepo dbRepo)
        {
            _dbRepo = dbRepo;
        }

        public async Task<IActionResult> RoleList()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var data = await _dbRepo.RoleMstList().Select(x => new { x.Id, x.RoleName }).ToListAsync();
                if (data != null && data.Count > 0)
                {
                    commonResponse.Data = data;
                    commonResponse.Status = true;
                    commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
                    commonResponse.Message = "Data found successfully!";

                }
                else
                {
                    commonResponse.Message = "Data not found!";
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message.ToString();
            }
            return Json(commonResponse);
        }

        public async Task<CommonResponse> GetRoleByUserId(int UserId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var data = await (from a in _dbRepo.AllUserMstList()
                                  where a.Id == UserId
                                  join b in _dbRepo.RoleMstList(withSuperAdmin: true) on a.RoleId equals b.Id into bGroup
                                  from bItem in bGroup.DefaultIfEmpty()
                                  select new
                                  {
                                      bItem.Id,
                                      bItem.RoleName,
                                      bItem.RoleCode
                                  }).FirstOrDefaultAsync();

                if (data != null)
                {
                    response.Data = data;
                    response.Status = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = "Data found successfully!";

                }
                else
                {
                    response.Message = "Data not found!";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
            }
            return response;
        }
    }
}
