using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArcheOne.Controllers
{
    public class TaskController : Controller
    {
        readonly ArcheOneDbContext _dbContext;
        readonly CommonHelper _commonHelper;
        readonly DbRepo _dbRepo;
        public TaskController(ArcheOneDbContext dbContext, CommonHelper commonHelper, DbRepo dbRepo)
        {
            _dbRepo = dbRepo;
            _dbContext = dbContext;
            _commonHelper = commonHelper;
        }

        public IActionResult Task()
        {
            return View();
        }

        public async Task<CommonResponse> GetTaskList()
        {
            CommonResponse response = new CommonResponse();
            try
            {
                int userId = _commonHelper.GetLoggedInUserId();
                CommonResponse roleDetailsResponse = await new RoleController(_dbRepo).GetRoleByUserId(userId);

                if (roleDetailsResponse.Status)
                {
                    bool showUserName = true;
                    if (roleDetailsResponse.Data.RoleCode == CommonEnums.RoleMst.Super_Admin.ToString())
                    {
                        showUserName = false;
                    }
                    var taskList = await (from a in _dbRepo.DailyTaskList()
                                          join b in _dbRepo.ProjectList() on a.ProjectId equals b.Id into bGroup
                                          from bItem in bGroup.DefaultIfEmpty()
                                          join c in _dbRepo.AllUserMstList() on a.CreatedBy equals c.Id into cGroup
                                          from cItem in cGroup.DefaultIfEmpty()
                                          select new
                                          {
                                              a.Id,
                                              bItem.ProjectName,
                                              a.TaskDate,
                                              a.TaskStatus,
                                              a.TimeSpent,
                                              a.TaskModule,
                                              a.TaskDescription,
                                              a.CreatedBy,
                                              CreatedByName = $"{cItem.FirstName ?? ""} {cItem.LastName ?? ""}",
                                              a.CreatedDate,
                                              IsEditable = a.CreatedDate != _commonHelper.GetCurrentDateTime(),
                                              ShowUserName = showUserName
                                          }).ToListAsync();

                    if (taskList != null && taskList.Count > 0)
                    {
                        response.Data = taskList;
                        response.Message = "Data found successfully!";
                        response.Status = true;
                        response.StatusCode = System.Net.HttpStatusCode.OK;
                    }
                    else
                    {
                        response.Message = "Data not found!";
                        response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<CommonResponse> GetTaskById(int TaskId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var dailyTaskDetails = await _dbRepo.DailyTaskList().FirstOrDefaultAsync(x => x.Id == TaskId);
                if (dailyTaskDetails != null)
                {
                    response.Data = dailyTaskDetails;
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

        public async Task<CommonResponse> AddUpdateTask([FromBody] AddUpdateTaskReqModel request)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    int userId = _commonHelper.GetLoggedInUserId();
                    if (request.Id == 0) // Add Daily Task
                    {
                        DailyTaskMst dailyTaskMst = new DailyTaskMst()
                        {
                            ProjectId = request.ProjectId,
                            TaskDate = request.TaskDate,
                            TaskStatus = request.TaskStatus,
                            TaskModule = request.TaskModule,
                            TaskDescription = request.TaskDescription,
                            TimeSpent = $"{request.TimeSpentHH}:{request.TimeSpentMM}",
                            IsActive = true,
                            IsDelete = false,
                            CreatedBy = userId,
                            UpdatedBy = userId,
                            CreatedDate = _commonHelper.GetCurrentDateTime(),
                            UpdatedDate = _commonHelper.GetCurrentDateTime()
                        };

                        await _dbContext.AddAsync(dailyTaskMst);
                        await _dbContext.SaveChangesAsync();

                        response.Status = true;
                        response.StatusCode = System.Net.HttpStatusCode.OK;
                        response.Message = "Task added successfully!";
                    }
                    else // updated
                    {
                        var dailyTask = await _dbRepo.DailyTaskList().FirstOrDefaultAsync(x => x.Id == request.Id);
                        if (dailyTask != null)
                        {
                            dailyTask.ProjectId = request.ProjectId;
                            dailyTask.TaskDate = request.TaskDate;
                            dailyTask.TaskStatus = request.TaskStatus;
                            dailyTask.TaskModule = request.TaskModule;
                            dailyTask.TaskDescription = request.TaskDescription;
                            dailyTask.TimeSpent = $"{request.TimeSpentHH}:{request.TimeSpentMM}";
                            dailyTask.UpdatedBy = userId;
                            dailyTask.UpdatedDate = _commonHelper.GetCurrentDateTime();

                            _dbContext.Entry(dailyTask).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();

                            response.Status = true;
                            response.StatusCode = System.Net.HttpStatusCode.OK;
                            response.Message = "Task updated successfully!";
                        }
                        else
                        {
                            response.Message = "Task not found!";
                            response.StatusCode = System.Net.HttpStatusCode.NotFound;
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

        public async Task<CommonResponse> DeleteTaskById(int TaskId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var dailyTaskMst = await _dbRepo.DailyTaskList().FirstOrDefaultAsync(x => x.Id == TaskId);
                if (dailyTaskMst != null)
                {
                    dailyTaskMst.IsDelete = true;
                    dailyTaskMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                    dailyTaskMst.UpdatedDate = _commonHelper.GetCurrentDateTime();

                    _dbContext.Entry(dailyTaskMst).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();


                    response.Status = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = "Task deleted successfully!";
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
