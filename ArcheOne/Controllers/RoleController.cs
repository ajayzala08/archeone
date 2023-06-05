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
    }
}
