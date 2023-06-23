﻿using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArcheOne.Controllers
{
    public class ProjectController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly CommonHelper _commonHelper;
        private readonly ArcheOneDbContext _dbContext;

        public ProjectController(DbRepo dbRepo, CommonHelper commonHelper, ArcheOneDbContext dbContext)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
            _dbContext = dbContext;
        }

        public IActionResult Project()
        {
            return View();
        }

        public async Task<CommonResponse> GetProjectList()
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var projectList = await (from project in _dbRepo.ProjectList()
                                         select new
                                         {
                                             project.Id,
                                             project.ProjectName,
                                             project.ProjectStatus,
                                             project.CreatedDate,
                                             project.Resources,
                                             IsEditable = project.ProjectStatus != CommonEnums.ProjectStatus.Completed.ToString(),
                                             IsDeletable = project.ProjectStatus == CommonEnums.ProjectStatus.Completed.ToString(),
                                             ResourcesNames = string.Join(",", _dbContext.UserMsts.Where(x => project.Resources.Contains(x.Id.ToString())).Select(x => x.FirstName + ' ' + x.LastName))
                                         }).ToListAsync();

                if (projectList != null && projectList.Count > 0)
                {
                    response.Data = projectList;
                    response.Status = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = "Data found successfully!";
                }
                else
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = "Data not found!";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<CommonResponse> GetProjectById(int ProjectId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var projectDetails = await _dbRepo.ProjectList().FirstOrDefaultAsync(x => x.Id == ProjectId);
                if (projectDetails != null)
                {
                    response.Data = projectDetails;
                    response.Status = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = "Data found successfully!";
                }
                else
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = "Data not found!";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public CommonResponse GetProjectStatus()
        {
            CommonResponse response = new CommonResponse();
            try
            {
                response.Data = Enum.GetValues(typeof(CommonEnums.ProjectStatus))
                    .Cast<CommonEnums.ProjectStatus>()
                    .Select(e => new { Id = (int)e, Title = e.ToString() }).ToList();

                response.Status = true;
                response.StatusCode = System.Net.HttpStatusCode.OK;
                response.Message = "Data found successfully!";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<CommonResponse> AddUpdateProject([FromBody] AddUpdateProjectReqModel request)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    int userId = _commonHelper.GetLoggedInUserId();
                    if (request.Id == 0) // Add New Project
                    {
                        if (!await _dbRepo.ProjectList().AnyAsync(x => x.ProjectName.ToLower() == request.ProjectName.ToLower()))
                        {
                            ProjectMst projectMst = new ProjectMst()
                            {
                                ProjectName = request.ProjectName,
                                ProjectStatus = request.ProjectStatus,
                                Resources = request.Resources,
                                IsActive = true,
                                IsDelete = false,
                                CreatedBy = userId,
                                UpdatedBy = userId,
                                CreatedDate = _commonHelper.GetCurrentDateTime(),
                                UpdatedDate = _commonHelper.GetCurrentDateTime(),
                            };

                            await _dbContext.AddAsync(projectMst);
                            await _dbContext.SaveChangesAsync();

                            response.Status = true;
                            response.StatusCode = System.Net.HttpStatusCode.OK;
                            response.Message = "Project added successfully!";
                        }
                        else
                        {
                            response.Message = "Project is already exist!";
                        }
                    }
                    else // Update Old Project
                    {
                        var duplicateProject = await _dbRepo.ProjectList().Where(x => x.ProjectName.ToLower() == request.ProjectName.ToLower()).ToListAsync();
                        if (duplicateProject != null && duplicateProject.Count == 1 && duplicateProject[0].Id == request.Id)
                        {
                            if (duplicateProject[0].ProjectStatus != CommonEnums.ProjectStatus.Completed.ToString())
                            {
                                ProjectMst projectMst = duplicateProject[0];
                                projectMst.ProjectName = request.ProjectName;
                                projectMst.ProjectStatus = request.ProjectStatus;
                                projectMst.Resources = request.Resources;
                                projectMst.UpdatedBy = userId;
                                projectMst.UpdatedDate = _commonHelper.GetCurrentDateTime();

                                _dbContext.Entry(projectMst).State = EntityState.Modified;
                                await _dbContext.SaveChangesAsync();

                                response.Status = true;
                                response.StatusCode = System.Net.HttpStatusCode.OK;
                                response.Message = "Project updated successfully!";
                            }
                            else
                            {

                                response.Message = "Project can't be edited if Status is Completed!";
                            }
                        }
                        else
                        {
                            response.Message = "Project is already exist!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<CommonResponse> DeleteProjectById(int ProjectId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var projectMst = await _dbRepo.PermissionList().FirstOrDefaultAsync(x => x.Id == ProjectId);
                if (projectMst != null)
                {
                    projectMst.IsDelete = true;
                    projectMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                    projectMst.UpdatedDate = _commonHelper.GetCurrentDateTime();

                    _dbContext.Entry(projectMst).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();


                    response.Status = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = "Project deleted successfully!";
                }
                else
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = "Data not found!";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
