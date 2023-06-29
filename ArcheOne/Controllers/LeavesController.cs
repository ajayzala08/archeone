using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ArcheOne.Controllers
{
    public class LeavesController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly CommonHelper _commonHelper;
        private readonly ArcheOneDbContext _dbContext;

        public LeavesController(DbRepo dbRepo, CommonHelper commonHelper, ArcheOneDbContext dbContext)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
            _dbContext = dbContext;

        }
        public IActionResult Leaves()
        {
            return View();
        }
        public async Task<IActionResult> LeavesList()
        {
            CommonResponse response = new CommonResponse();
            try
            {
                dynamic leavesList = "leavesList";


                if (leavesList != null && leavesList.Count > 0)
                {
                    response.Data = leavesList;
                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Data found successfully!";
                }
                else
                {
                    response.Message = "Data not found!";
                    response.StatusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Json(response);
        }

        public async Task<IActionResult> AddEditLeave(int Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                LeaveAddEditReqModel leaveAddEditReqModel = new LeaveAddEditReqModel();
                leaveAddEditReqModel.leaveDetails = new LeaveDetails();
                leaveAddEditReqModel.leaveTypeList = _dbRepo.LeaveTypeLists().Where(x => x.IsCurrentYear == true).ToList();

                var userId = _dbRepo.GetLoggedInUserDetails().RoleId;

                var roleUserId = await _dbRepo.RoleMstList().FirstOrDefaultAsync(x => x.Id == userId);


                var startTimeList = new List<KeyValueModel>();
                startTimeList.Add(new KeyValueModel { Id = 1, Name = "09:30 AM" });
                startTimeList.Add(new KeyValueModel { Id = 2, Name = "02:00 PM" });
                leaveAddEditReqModel.StartTimeList = startTimeList;

                if (Id > 0)
                {
                    var leaveDetails = await _dbRepo.LeaveLists().FirstOrDefaultAsync(x => x.Id == Id);
                    if (leaveDetails != null)
                    {
                        leaveAddEditReqModel.leaveDetails.Id = leaveDetails.Id;
                        leaveAddEditReqModel.leaveDetails.LeaveTypeId = leaveDetails.LeaveTypeId;
                        leaveAddEditReqModel.leaveDetails.StartDate = leaveDetails.StartDate;
                        leaveAddEditReqModel.leaveDetails.EndDate = leaveDetails.EndDate;
                        leaveAddEditReqModel.leaveDetails.StartTime = leaveDetails.StartTime;
                        leaveAddEditReqModel.leaveDetails.EndTime = leaveDetails.EndTime;
                        leaveAddEditReqModel.leaveDetails.Reason = leaveDetails.Reason;

                    }
                }
                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "Success!";
                commonResponse.Data = leaveAddEditReqModel;
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Data = ex;
            }
            return View(commonResponse.Data);
        }
        public async Task<IActionResult> EndTimeList(int id)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var endTimeList = new List<KeyValueModel>();
                if (id == 1)
                {
                    endTimeList.Add(new KeyValueModel { Id = 1, Name = "02:00 PM" });
                    endTimeList.Add(new KeyValueModel { Id = 2, Name = "06:30 PM" });
                }
                else
                {
                    endTimeList.Add(new KeyValueModel { Id = 2, Name = "06:30 PM" });
                }
                if (endTimeList.Count > 0)
                {
                    response.Data = endTimeList;
                    response.Status = true;
                    response.Message = "Data found successfully!";
                }
                else
                {
                    response.Message = "Data not found!";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Json(response);
        }
        [HttpPost]
        public async Task<CommonResponse> SaveUpdateLeave([FromBody] AddUpdateLeaveReqModel request)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    int userId = _commonHelper.GetLoggedInUserId();
                    decimal noOfDay = 0;
                    if (request.Id == 0) // Add Leave
                    {
                        var userJoiningDate = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == userId);
                        if (userJoiningDate != null)
                        {
                            decimal isProbationPeriodDays = 0;
                            bool isProbationPeriod = false;
                            #region ProbationPeriod

                            if (userJoiningDate.JoinDate < request.StartDate)
                            {
                                isProbationPeriodDays = GetProbationPeriod(userJoiningDate.JoinDate, request.StartDate);

                                if (isProbationPeriodDays > 0)
                                {
                                    if (isProbationPeriodDays < Convert.ToDecimal(userJoiningDate.ProbationPeriod))
                                    {
                                        isProbationPeriod = false;
                                    }
                                    else if (isProbationPeriodDays >= Convert.ToDecimal(userJoiningDate.ProbationPeriod))
                                    {
                                        isProbationPeriod = true;
                                    }
                                }
                            }
                            else
                            {
                                response.Message = "Please select valid startdate";
                            }
                            #endregion


                            if (isProbationPeriod)
                            {

                            }
                            else
                            {

                            }
                        }
                        #region ADD

                        //var leaveTypeDetails = await _dbRepo.LeaveTypeLists().FirstOrDefaultAsync(x => x.Id == request.LeaveTypeId);
                        //if (leaveTypeDetails != null)
                        //{
                        //	if (leaveTypeDetails.LeaveTypeName.ToLower() == "sickleave")
                        //	{
                        //		noOfDay = NoOfDaySickLeave();
                        //	}
                        //	else if (leaveTypeDetails.LeaveTypeName.ToLower() == "casualleave")
                        //	{
                        //		noOfDay = NoOfDayCasualLeave();
                        //	}
                        //	else if (leaveTypeDetails.LeaveTypeName.ToLower() == "earnedleave")
                        //	{
                        //		noOfDay = NoOfDayEarnedLeave();
                        //	}



                        //	LeaveMst leaveMst = new LeaveMst()
                        //	{
                        //		LeaveTypeId = request.LeaveTypeId,
                        //		StartDate = request.StartDate,
                        //		EndDate = request.EndDate,
                        //		StartTime = request.StartTime,
                        //		EndTime = request.EndTime,
                        //		Reason = request.Reason,
                        //		NoOfDays = noOfDay,
                        //		AppliedByUserId = userId,
                        //		ApprovedByUserId = userId,
                        //		LeaveStatusId = 1,
                        //		LeaveBalance = 24,
                        //		IsActive = true,
                        //		IsDelete = false,
                        //		CreatedBy = userId,
                        //		UpdatedBy = userId,
                        //		CreatedDate = _commonHelper.GetCurrentDateTime(),
                        //		UpdatedDate = _commonHelper.GetCurrentDateTime()
                        //	};

                        //	//await _dbContext.LeaveMsts.AddAsync(leaveMst);
                        //	//await _dbContext.SaveChangesAsync();

                        //	response.Status = true;
                        //	response.Message = "Leave added successfully!";
                        //}

                        #endregion
                    }
                    else // updated
                    {
                        var LeaveDetails = await _dbRepo.LeaveLists().FirstOrDefaultAsync(x => x.Id == request.Id);
                        if (LeaveDetails != null)
                        {
                            LeaveDetails.LeaveTypeId = request.LeaveTypeId;
                            LeaveDetails.StartDate = request.StartDate;
                            LeaveDetails.EndDate = request.EndDate;
                            //LeaveDetails.StartTime = request.StartTime;
                            //LeaveDetails.EndTime = request.EndTime;
                            LeaveDetails.Reason = request.Reason;
                            LeaveDetails.NoOfDays = 18;
                            LeaveDetails.AppliedByUserId = userId;
                            LeaveDetails.ApprovedByUserId = userId;
                            LeaveDetails.LeaveStatusId = 1;
                            LeaveDetails.LeaveBalance = 24;
                            LeaveDetails.UpdatedBy = userId;
                            LeaveDetails.UpdatedDate = _commonHelper.GetCurrentDateTime();

                            _dbContext.Entry(LeaveDetails).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();

                            response.Status = true;
                            response.StatusCode = System.Net.HttpStatusCode.OK;
                            response.Message = "Leave updated successfully!";
                        }
                        else
                        {
                            response.Message = "Leave not found!";
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

        public decimal NoOfDaySickLeave()
        {
            decimal NoOfDay = 0;

            return NoOfDay;
        }
        public decimal NoOfDayCasualLeave()
        {
            decimal NoOfDay = 0;

            return NoOfDay;
        }
        public decimal NoOfDayEarnedLeave()
        {
            decimal NoOfDay = 0;

            return NoOfDay;
        }



        public decimal GetProbationPeriod(DateTime StartDate, DateTime EndDate)
        {
            decimal isProbationPeriodDays = 0;
            var span = EndDate - StartDate; //return timespan
            isProbationPeriodDays = span.Days; //return days
            return isProbationPeriodDays;
        }
    }
}
