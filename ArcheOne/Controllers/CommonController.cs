using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ArcheOne.Controllers
{
    public class CommonController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly ArcheOneDbContext _dbContext;
        private readonly CommonHelper _commonHelper;
        public CommonController(DbRepo dbRepo, ArcheOneDbContext dbContext, CommonHelper commonHelper)
        {
            _dbRepo = dbRepo;
            _dbContext = dbContext;
            _commonHelper = commonHelper;
        }

        public async Task<CommonResponse> GetInterviewRoundTypeList()
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var data = await _dbRepo.InterviewRoundTypeList().Select(x => new { x.Id, x.InterviewRoundTypeName, x.InterviewRoundTypeCode }).ToListAsync();
                if (data != null && data.Count > 0)
                {
                    response.Data = data;
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
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<IActionResult> GetRequirementForList()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var requirementList = await _dbRepo.RequirementForList().Select(x => new { x.Id, x.RequirementForName }).ToListAsync();
                if (requirementList.Count > 0)
                {
                    commonResponse.Data = requirementList;
                    commonResponse.Message = "Data found successfully!";
                    commonResponse.Status = true;
                    commonResponse.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    commonResponse.Message = "Data not found!";
                    commonResponse.StatusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return Json(commonResponse);
        }

        public async Task<IActionResult> GetClientList()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var clientList = await _dbRepo.ClientList().Select(x => new { x.Id, x.ClientName }).ToListAsync();
                if (clientList.Count > 0)
                {
                    commonResponse.Data = clientList;
                    commonResponse.Message = "Data found successfully!";
                    commonResponse.Status = true;
                    commonResponse.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    commonResponse.Message = "Data not found!";
                    commonResponse.StatusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return Json(commonResponse);
        }

        public async Task<IActionResult> GetPositionTypeList()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var positionTypeList = await _dbRepo.PositionTypeList().Select(x => new { x.Id, x.PositionTypeName }).ToListAsync();
                if (positionTypeList.Count > 0)
                {
                    commonResponse.Data = positionTypeList;
                    commonResponse.Message = "Data found successfully!";
                    commonResponse.Status = true;
                    commonResponse.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    commonResponse.Message = "Data not found!";
                    commonResponse.StatusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return Json(commonResponse);
        }

        public async Task<IActionResult> GetRequirementTypeList()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var requirementTypeList = await _dbRepo.RequirementTypeList().Select(x => new { x.Id, x.RequirementTypeName }).ToListAsync();
                if (requirementTypeList.Count > 0)
                {
                    commonResponse.Data = requirementTypeList;
                    commonResponse.Message = "Data found successfully!";
                    commonResponse.Status = true;
                    commonResponse.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    commonResponse.Message = "Data not found!";
                    commonResponse.StatusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return Json(commonResponse);
        }

        public async Task<IActionResult> GetEmploymentTypeList()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var employmentTypeList = await _dbRepo.EmploymentTypeList().Select(x => new { x.Id, x.EmploymentTypeName }).ToListAsync();
                if (employmentTypeList.Count > 0)
                {
                    commonResponse.Data = employmentTypeList;
                    commonResponse.Message = "Data found successfully!";
                    commonResponse.Status = true;
                    commonResponse.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    commonResponse.Message = "Data not found!";
                    commonResponse.StatusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return Json(commonResponse);
        }

        public async Task<IActionResult> GetRequirementStatusList()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var requirementStatusList = await _dbRepo.RequirementStatusList().Select(x => new { x.Id, x.RequirementStatusName }).ToListAsync();
                if (requirementStatusList.Count > 0)
                {
                    commonResponse.Data = requirementStatusList;
                    commonResponse.Message = "Data found successfully!";
                    commonResponse.Status = true;
                    commonResponse.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    commonResponse.Message = "Data not found!";
                    commonResponse.StatusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return Json(commonResponse);
        }

        public async Task<CommonResponse> GetDepartmentByUserId(int UserId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var data = await (from userMst in _dbRepo.AllUserMstList()
                                  where userMst.Id == UserId
                                  join department in _dbRepo.DepartmentList() on userMst.DepartmentId equals department.Id into departmentGroup
                                  from departmentItem in departmentGroup.DefaultIfEmpty()
                                  select new
                                  {
                                      departmentItem.Id,
                                      departmentItem.DepartmentName,
                                      departmentItem.DepartmentCode
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

        public async Task<IActionResult> AddEvent(AddEventReqModel addEvent)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                if (addEvent != null)
                {
                    EventMst eventMst = new EventMst();
                    eventMst.Subject = addEvent.Subject;
                    eventMst.StartDate = addEvent.Start;
                    eventMst.EndDate = addEvent.End;
                    eventMst.Description = addEvent.Description;
                    eventMst.IsFullDay = addEvent.IsFullDay;
                    eventMst.ThemeColour = addEvent.TheamColor;
                    eventMst.IsFullDay = addEvent.IsFullDay;
                    eventMst.EventType = addEvent.EventType;
                    eventMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                    eventMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                    eventMst.CreatedDate = DateTime.Now;
                    eventMst.UpdatedDate = DateTime.Now;
                    eventMst.IsActive = true;
                    eventMst.IsDelete = false;
                    _dbContext.EventMsts.Add(eventMst);
                    await _dbContext.SaveChangesAsync();
                    commonResponse.Status = true;
                    commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
                    commonResponse.Message = "Add Event Successfully";

                    return Json(commonResponse);
                }
                else
                {
                    commonResponse.Status = false;
                    commonResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                    commonResponse.Message = "Please Enter Valid Data";
                    return Json(commonResponse);
                }


            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message.ToString();
                commonResponse.Data = ex.ToString();
            }
            return (IActionResult)commonResponse;
        }
    }
}
