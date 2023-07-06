﻿using ArcheOne.Database.Entities;
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
                var userId = _commonHelper.GetLoggedInUserId();
                var roleDetails = await new RoleController(_dbRepo).GetRoleByUserId(userId);
                if (roleDetails != null)
                {
                    bool showPermissionRoutes = false;
                    if (roleDetails.Data.RoleCode == CommonEnums.RoleMst.Super_Admin.ToString())
                    {
                        showPermissionRoutes = true;
                    }

                    var data = await (from permission in _dbRepo.PermissionList()
                                      join defaultPermission in _dbRepo.DefaultPermissionList().Where(x => x.RoleId == RoleId)
                                      on permission.Id equals defaultPermission.PermissionId into defaultPermissionGroup
                                      from defaultPermission in defaultPermissionGroup.DefaultIfEmpty()
                                      select new
                                      {
                                          permission.Id,
                                          permission.PermissionName,
                                          permission.PermissionRoute,
                                          ShowPermissionRoutes = showPermissionRoutes,
                                          IsDefaultPermission = defaultPermission != null
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
                        var defaultPermissionsList = await _dbRepo.DefaultPermissionList().Where(x => x.RoleId == request.RoleId).ToListAsync();
                        _dbContext.DefaultPermissions.RemoveRange(defaultPermissionsList);
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

                        // update permission to existing all users also.
                        if (request.UpdateRoleWithUsers)
                        {
                            var userListWithRole = await _dbRepo.UserMstList().Where(x => x.RoleId == request.RoleId).Select(x => x.Id).ToListAsync();

                            var userPermissionList = (from um in userListWithRole join up in _dbRepo.UserPermissionList() on um equals up.UserId select up).ToList();

                            _dbContext.UserPermissions.RemoveRange(userPermissionList);
                            await _dbContext.SaveChangesAsync();

                            List<UserPermission> userPermissions = new List<UserPermission>();
                            foreach (var item in userListWithRole)
                            {
                                foreach (var subItem in request.PermissionIds)
                                {
                                    userPermissions.Add(new UserPermission
                                    {
                                        UserId = item,
                                        PermissionId = subItem,
                                        IsActive = true,
                                        IsDelete = false,
                                        CreatedBy = request.CreatedBy,
                                        UpdatedBy = request.CreatedBy,
                                        CreatedDate = _commonHelper.GetCurrentDateTime(),
                                        UpdatedDate = _commonHelper.GetCurrentDateTime(),
                                    });
                                }
                            }

                            await _dbContext.UserPermissions.AddRangeAsync(userPermissions);
                            await _dbContext.SaveChangesAsync();
                        }

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

        public IActionResult UserPermission()
        {
            return View();
        }

        public async Task<IActionResult> GetUserPermissions(int UserId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var data = await (from permission in _dbRepo.PermissionList()
                                  join userPermission in _dbRepo.UserPermissionList().Where(x => x.UserId == UserId)
                                  on permission.Id equals userPermission.PermissionId into userPermissionGroup
                                  from userPermission in userPermissionGroup.DefaultIfEmpty()
                                  select new
                                  {
                                      permission.Id,
                                      permission.PermissionName,
                                      IsDefaultPermission = userPermission != null
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

        public async Task<IActionResult> UpdateUserPermission([FromBody] UpdateUserPermissionReqModel request)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                if (ModelState.IsValid && request.UserId != 0 && request.CreatedBy != 0)
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        //Remove all the permissions from UserPermission table with given RoleId
                        var dt = await _dbRepo.UserPermissionList().Where(x => x.UserId == request.UserId).ToListAsync();
                        _dbContext.UserPermissions.RemoveRange(dt);
                        await _dbContext.SaveChangesAsync();

                        //Insert new permissions in UserPermission
                        List<UserPermission> userPermissions = new List<UserPermission>();
                        foreach (var item in request.PermissionIds)
                        {
                            userPermissions.Add(new UserPermission
                            {
                                UserId = request.UserId,
                                PermissionId = item,
                                IsActive = true,
                                IsDelete = false,
                                CreatedBy = request.CreatedBy,
                                UpdatedBy = request.CreatedBy,
                                CreatedDate = _commonHelper.GetCurrentDateTime(),
                                UpdatedDate = _commonHelper.GetCurrentDateTime()
                            });
                        }
                        await _dbContext.UserPermissions.AddRangeAsync(userPermissions);
                        await _dbContext.SaveChangesAsync();

                        scope.Complete();

                        response.Message = "User Permissions updated successfully!";
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

        public async Task<IActionResult> GetPermissionDetailsById(int PermissionId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var permissionDetails = await _dbRepo.PermissionList().Where(x => x.Id == PermissionId).Select(x => new { x.Id, x.PermissionName, x.PermissionCode, x.PermissionRoute }).FirstOrDefaultAsync();
                if (permissionDetails != null)
                {
                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Data found successfully!";
                    response.Data = permissionDetails;
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

        public async Task<IActionResult> UpdatePermissionDetails([FromBody] UpdatePermissionDetailsReqModel request)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var permissionDetails = await _dbRepo.PermissionList().FirstOrDefaultAsync(x => x.Id == request.PermissionId);
                if (permissionDetails != null)
                {
                    permissionDetails.PermissionRoute = request.PermissionRoute;


                    _dbContext.Entry(permissionDetails).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();

                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Permission details updated successfully!";
                    response.Data = permissionDetails;
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
