using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Transactions;

namespace ArcheOne.Controllers
{
    public class PermissionController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly ArcheOneDbContext _dbContext;
        private readonly CommonHelper _commonHelper;
        public PermissionController(ArcheOneDbContext dbContext, DbRepo dbRepo, CommonHelper commonHelper)
        {
            _dbContext = dbContext;
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
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
                var data = await (from pm in _dbRepo.PermissionList()
                                  join dp in _dbRepo.DefaultPermissionList().Where(x => x.RoleId == RoleId)
                                  on pm.Id equals dp.PermissionId into dpGroup
                                  from dp in dpGroup.DefaultIfEmpty()
                                  select new
                                  {
                                      pm.Id,
                                      pm.PermissionName,
                                      IsDefaultPermission = dp != null
                                  }).ToListAsync();

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

        public async Task<IActionResult> UpdateDefaultPermission([FromBody] UpdateDefaultPermissionReqModel request)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                if (ModelState.IsValid && request.RoleId != 0 && request.CreatedBy != 0)
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        //Remove all the permissions from DefaultPermission table with given RoleId
                        var dt = await _dbRepo.DefaultPermissionList().Where(x => x.RoleId == request.RoleId).ToListAsync();
                        _dbContext.DefaultPermissions.RemoveRange(dt);
                        await _dbContext.SaveChangesAsync();

                        //Insert new permissions in DefaultPermission
                        List<DefaultPermission> defaultPermissions = new List<DefaultPermission>();
                        foreach (var item in request.PermissionIds)
                        {
                            defaultPermissions.Add(new DefaultPermission
                            {
                                RoleId = request.RoleId,
                                PermissionId = item,
                                IsActive = true,
                                IsDelete = false,
                                CreatedBy = request.CreatedBy,
                                UpdatedBy = request.CreatedBy,
                                CreatedDate = _commonHelper.GetCurrentDateTime(),
                                UpdatedDate = _commonHelper.GetCurrentDateTime()
                            });
                        }
                        await _dbContext.DefaultPermissions.AddRangeAsync(defaultPermissions);
                        await _dbContext.SaveChangesAsync();

                        scope.Complete();

                        response.Message = "Permissions updated successfully!";
                        response.Status = true;
                        response.StatusCode = HttpStatusCode.OK;
                    };
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
