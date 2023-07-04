using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Transactions;

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
        [HttpPost]
        public async Task<IActionResult> EndTimeList([FromBody] EndTimeListReqModel endTimeListReqModel)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    DateTime startDate = endTimeListReqModel.StartDate;
                    DateTime endDate = endTimeListReqModel.EndDate;
                    var endTimeList = new List<KeyValueModel>();
                    if (endDate >= startDate)
                    {
                        if (startDate == endDate)
                        {
                            if (endTimeListReqModel.StartTime == "09:30 AM")
                            {
                                endTimeList.Add(new KeyValueModel { Id = 1, Name = "02:00 PM" });
                                endTimeList.Add(new KeyValueModel { Id = 2, Name = "06:30 PM" });
                            }
                            else
                            {
                                endTimeList.Add(new KeyValueModel { Id = 2, Name = "06:30 PM" });
                            }
                        }
                        else
                        {
                            endTimeList.Add(new KeyValueModel { Id = 1, Name = "02:00 PM" });
                            endTimeList.Add(new KeyValueModel { Id = 2, Name = "06:30 PM" });
                        }
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
                    decimal noOfDay = 0;

                    #region MyRegion

                    LeaveMst leaveMst = new LeaveMst();
                    LeaveBalanceMst leaveBalanceMst = new LeaveBalanceMst();
                    int userId = _commonHelper.GetLoggedInUserId();

                    if (request.Id == 0) // Add Leave
                    {

                        var userJoiningDate = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == userId);
                        if (userJoiningDate != null)
                        {
                            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
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

                                    #endregion

                                    if (request.StartDate <= request.EndDate)
                                    {

                                        noOfDay = GetNoOfDays(request.StartDate, request.EndDate, request.StartTime, request.EndTime);

                                        var LeaveBalanceList12 = await _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();

                                        leaveMst.LeaveTypeId = request.LeaveTypeId;
                                        leaveMst.Reason = request.Reason;
                                        leaveMst.NoOfDays = noOfDay;
                                        leaveMst.StartDate = request.StartDate;
                                        leaveMst.StartTime = Convert.ToDateTime(request.StartTime).TimeOfDay;
                                        leaveMst.EndDate = request.EndDate;
                                        leaveMst.EndTime = Convert.ToDateTime(request.EndTime).TimeOfDay;
                                        leaveMst.AppliedByUserId = userId;
                                        leaveMst.ApprovedByUserId = userId;
                                        leaveMst.OpeningLeaveBalance = LeaveBalanceList12 == null ? 0 : LeaveBalanceList12.ClosingLeaveBalance;
                                        leaveMst.LeaveStatusId = 1;
                                        leaveMst.IsActive = true;
                                        leaveMst.IsDelete = false;
                                        leaveMst.CreatedBy = userId;
                                        leaveMst.UpdatedBy = userId;
                                        leaveMst.CreatedDate = _commonHelper.GetCurrentDateTime();
                                        leaveMst.UpdatedDate = _commonHelper.GetCurrentDateTime();





                                        var LeaveBalanceList = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId).OrderBy(x => x.Id).LastOrDefault();


                                        if (LeaveBalanceList != null)
                                        {
                                            if (isProbationPeriod)
                                            {

                                                DateTime dt2 = new DateTime(DateTime.Now.Year, 01, 01);
                                                for (DateTime i = dt2; i <= request.StartDate; i = i.Date.AddMonths(1))
                                                {
                                                    var BalanceMonth = i.Date.ToString("MMMM");
                                                    var LeaveBalance = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId && x.BalanceMonth == BalanceMonth).OrderBy(x => x.Id).LastOrDefault();

                                                    if (LeaveBalance == null)

                                                    {
                                                        var LeaveBalanceList1 = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId).OrderBy(x => x.Id).LastOrDefault();


                                                        var Leavetype1 = await _dbRepo.LeaveLists().Where(x => x.LeaveTypeId == request.LeaveTypeId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();

                                                        LeaveBalanceMst tbl = new LeaveBalanceMst();
                                                        tbl.UserId = userId;
                                                        tbl.LeaveTypeId = request.LeaveTypeId;
                                                        tbl.BalanceMonth = i.Date.ToString("MMMM");
                                                        tbl.BalanceYear = i.Year;
                                                        tbl.NoOfDays = 0;
                                                        tbl.OpeningLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.ClosingLeaveBalance;

                                                        if (Leavetype1.Id == request.LeaveTypeId)
                                                        {
                                                            tbl.SickLeaveBalance = LeaveBalanceList1.SickLeaveTaken == 0 ? LeaveBalanceList1.SickLeaveBalance + decimal.Parse("0.5") : LeaveBalanceList1.SickLeaveBalance - LeaveBalanceList1.SickLeaveTaken;
                                                        }
                                                        else
                                                        {
                                                            tbl.SickLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.SickLeaveBalance + decimal.Parse("0.5");
                                                        }

                                                        if (Leavetype1.Id == request.LeaveTypeId)
                                                        {
                                                            tbl.CasualLeaveBalance = LeaveBalanceList1.CasualLeaveBalance == 0 ? LeaveBalanceList1.CasualLeaveBalance + decimal.Parse("0.5") : LeaveBalanceList1.CasualLeaveBalance - LeaveBalanceList1.CasualLeaveTaken;
                                                        }
                                                        else
                                                        {
                                                            tbl.CasualLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.CasualLeaveBalance + decimal.Parse("0.5");
                                                        }


                                                        if (Leavetype1.Id == request.LeaveTypeId)
                                                        {
                                                            tbl.EarnedLeaveBalance = LeaveBalanceList1.EarnedLeaveBalance == 0 ? LeaveBalanceList1.EarnedLeaveBalance + decimal.Parse("0.5") : LeaveBalanceList1.EarnedLeaveBalance - LeaveBalanceList1.EarnedLeaveTaken;
                                                        }
                                                        else
                                                        {
                                                            tbl.EarnedLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.EarnedLeaveBalance + decimal.Parse("0.5");
                                                        }



                                                        tbl.SickLeaveTaken = 0;
                                                        tbl.CasualLeaveTaken = 0;
                                                        tbl.EarnedLeaveTaken = 0;
                                                        tbl.ClosingLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.ClosingLeaveBalance + decimal.Parse("1.5");
                                                        tbl.LeaveTaken = 0;
                                                        tbl.BalanceDate = DateTime.Now;
                                                        tbl.Detail = "Monthly Leave Balance";
                                                        tbl.IsActive = true;
                                                        tbl.IsDelete = false;
                                                        tbl.CreatedBy = userId;
                                                        tbl.UpdatedBy = userId;
                                                        tbl.CreatedDate = _commonHelper.GetCurrentDateTime();
                                                        tbl.UpdatedDate = _commonHelper.GetCurrentDateTime();

                                                        await _dbContext.LeaveBalanceMsts.AddAsync(tbl);
                                                        _dbContext.SaveChanges();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                DateTime dt2 = new DateTime(DateTime.Now.Year, 01, 01);
                                                for (DateTime i = dt2; i <= request.StartDate; i = i.Date.AddMonths(1))
                                                {
                                                    var BalanceMonth = i.Date.ToString("MMMM");


                                                    var LeaveBalance = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId && x.BalanceMonth == BalanceMonth).OrderBy(x => x.Id).LastOrDefault();

                                                    if (LeaveBalance == null)

                                                    {
                                                        var LeaveBalanceList1 = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId).OrderBy(x => x.Id).LastOrDefault();

                                                        LeaveBalanceMst tbl = new LeaveBalanceMst();
                                                        tbl.UserId = userId;
                                                        tbl.LeaveTypeId = request.LeaveTypeId;
                                                        tbl.BalanceMonth = i.Date.ToString("MMMM");
                                                        tbl.BalanceYear = i.Year;
                                                        tbl.NoOfDays = 0;
                                                        tbl.OpeningLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.ClosingLeaveBalance;
                                                        tbl.SickLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.SickLeaveBalance + decimal.Parse("0.5");
                                                        tbl.CasualLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.CasualLeaveBalance + decimal.Parse("0.5");
                                                        tbl.EarnedLeaveBalance = 6;
                                                        tbl.SickLeaveTaken = 0;
                                                        tbl.CasualLeaveTaken = 0;
                                                        tbl.EarnedLeaveTaken = 0;
                                                        tbl.ClosingLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.ClosingLeaveBalance + decimal.Parse("1.5");
                                                        tbl.LeaveTaken = 0;
                                                        tbl.BalanceDate = DateTime.Now;
                                                        tbl.Detail = "Monthly Leave Balance";
                                                        tbl.IsActive = true;
                                                        tbl.IsDelete = false;
                                                        tbl.CreatedBy = userId;
                                                        tbl.UpdatedBy = userId;
                                                        tbl.CreatedDate = _commonHelper.GetCurrentDateTime();
                                                        tbl.UpdatedDate = _commonHelper.GetCurrentDateTime();

                                                        await _dbContext.LeaveBalanceMsts.AddAsync(tbl);
                                                        await _dbContext.SaveChangesAsync();
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {

                                            DateTime dt2 = new DateTime(DateTime.Now.Year, 01, 01);
                                            for (DateTime i = dt2; i <= request.StartDate; i = i.Date.AddMonths(1))
                                            {
                                                var LeaveBalanceList1 = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId).OrderBy(x => x.Id).LastOrDefault();

                                                LeaveBalanceMst tbl = new LeaveBalanceMst();
                                                tbl.UserId = userId;
                                                tbl.LeaveTypeId = request.LeaveTypeId;
                                                tbl.BalanceMonth = i.Date.ToString("MMMM");
                                                tbl.BalanceYear = i.Year;
                                                tbl.NoOfDays = 0;
                                                tbl.OpeningLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.ClosingLeaveBalance;
                                                tbl.SickLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.SickLeaveBalance + decimal.Parse("0.5");
                                                tbl.CasualLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.CasualLeaveBalance + decimal.Parse("0.5");
                                                tbl.EarnedLeaveBalance = 6;
                                                tbl.SickLeaveTaken = 0;
                                                tbl.CasualLeaveTaken = 0;
                                                tbl.EarnedLeaveTaken = 0;
                                                tbl.ClosingLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.ClosingLeaveBalance + decimal.Parse("1.5");
                                                tbl.LeaveTaken = 0;
                                                tbl.BalanceDate = DateTime.Now;
                                                tbl.Detail = "Monthly Leave Balance";
                                                tbl.IsActive = true;
                                                tbl.IsDelete = false;
                                                tbl.CreatedBy = userId;
                                                tbl.UpdatedBy = userId;
                                                tbl.CreatedDate = _commonHelper.GetCurrentDateTime();
                                                tbl.UpdatedDate = _commonHelper.GetCurrentDateTime();

                                                await _dbContext.LeaveBalanceMsts.AddAsync(tbl);
                                                _dbContext.SaveChanges();
                                            }
                                        }

                                        var BalanceList = await _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();

                                        var Leavetype = await _dbRepo.LeaveTypeLists().Where(x => x.Id == request.LeaveTypeId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                                        if (BalanceList != null && Leavetype != null)
                                        {
                                            if (noOfDay >= 4)
                                            {
                                                //code if apply leave for 4 or more than 4 days


                                                if (Leavetype.LeaveTypeName.ToLower() == "sickleave")
                                                {
                                                    if (BalanceList.SickLeaveBalance >= noOfDay)

                                                    {
                                                        leaveMst.PaidDays = noOfDay;
                                                        leaveMst.UnPaidDays = decimal.Parse("0");

                                                    }
                                                    else
                                                    {
                                                        leaveMst.PaidDays = BalanceList.SickLeaveBalance;
                                                        leaveMst.UnPaidDays = noOfDay - leaveMst.PaidDays;

                                                    }
                                                }
                                                else if (Leavetype.LeaveTypeName.ToLower() == "casualleave")
                                                {

                                                    if (BalanceList.CasualLeaveBalance >= noOfDay)

                                                    {
                                                        leaveMst.PaidDays = noOfDay;
                                                        leaveMst.UnPaidDays = decimal.Parse("0");
                                                    }
                                                    else
                                                    {
                                                        leaveMst.PaidDays = BalanceList.CasualLeaveBalance;
                                                        leaveMst.UnPaidDays = noOfDay - leaveMst.PaidDays;
                                                    }
                                                }
                                                else if (Leavetype.LeaveTypeName.ToLower() == "earnedleave")
                                                {

                                                    if (BalanceList.EarnedLeaveBalance >= noOfDay)

                                                    {
                                                        leaveMst.PaidDays = noOfDay;
                                                        leaveMst.UnPaidDays = decimal.Parse("0");

                                                    }
                                                    else
                                                    {
                                                        leaveMst.PaidDays = BalanceList.EarnedLeaveBalance;
                                                        leaveMst.UnPaidDays = noOfDay - leaveMst.PaidDays;

                                                    }
                                                }


                                            }
                                            else
                                            {
                                                if (Leavetype.LeaveTypeName.ToLower() == "sickleave")
                                                {
                                                    if (BalanceList.SickLeaveBalance >= noOfDay)
                                                    {
                                                        leaveMst.PaidDays = noOfDay;
                                                        leaveMst.UnPaidDays = decimal.Parse("0");

                                                    }
                                                    else
                                                    {
                                                        leaveMst.PaidDays = BalanceList.SickLeaveBalance;
                                                        leaveMst.UnPaidDays = noOfDay - leaveMst.PaidDays;

                                                    }
                                                }
                                                else if (Leavetype.LeaveTypeName.ToLower() == "casualleave")
                                                {

                                                    if (BalanceList.CasualLeaveBalance >= noOfDay)

                                                    {
                                                        leaveMst.PaidDays = noOfDay;
                                                        leaveMst.UnPaidDays = decimal.Parse("0");


                                                    }
                                                    else
                                                    {
                                                        leaveMst.PaidDays = BalanceList.CasualLeaveBalance;
                                                        leaveMst.UnPaidDays = noOfDay - leaveMst.PaidDays;
                                                    }
                                                }
                                                else if (Leavetype.LeaveTypeName.ToLower() == "earnedleave")
                                                {

                                                    if (BalanceList.EarnedLeaveBalance >= noOfDay)

                                                    {
                                                        leaveMst.PaidDays = noOfDay;
                                                        leaveMst.UnPaidDays = decimal.Parse("0");

                                                    }
                                                    else
                                                    {
                                                        leaveMst.PaidDays = BalanceList.EarnedLeaveBalance;
                                                        leaveMst.UnPaidDays = noOfDay - leaveMst.PaidDays;

                                                    }
                                                }

                                            }

                                            if (isProbationPeriod)
                                            {
                                                BalanceList.SickLeaveTaken = leaveMst.PaidDays;
                                                BalanceList.CasualLeaveTaken = leaveMst.PaidDays;
                                                BalanceList.EarnedLeaveTaken = leaveMst.PaidDays;
                                                //_dbContext.Entry(BalanceList).State = EntityState.Modified;
                                                //await _dbContext.SaveChangesAsync();
                                            }
                                        }
                                        else
                                        {
                                            response.Message = "Data Not Found!";

                                        }

                                        if (isProbationPeriod == false)
                                        {
                                            leaveMst.PaidDays = 0;
                                            leaveMst.UnPaidDays = noOfDay;

                                        }

                                        var sandwichleave = SandwichLeave(request);

                                        await _dbContext.LeaveMsts.AddAsync(leaveMst);
                                        await _dbContext.SaveChangesAsync();

                                        transactionScope.Complete();
                                        response.Status = true;
                                        response.Message = "Leave add successfully!";
                                    }
                                    else
                                    {
                                        response.Message = "Please select valid startdate!";

                                    }

                                }
                                else
                                {
                                    response.Message = "Please select valid startdate!";
                                }
                            }
                        }
                    }
                    #endregion

                    else // updated
                    {
                        #region MyRegion

                        //var LeaveDetails = await _dbRepo.LeaveLists().FirstOrDefaultAsync(x => x.Id == request.Id);
                        //if (LeaveDetails != null)
                        //{
                        //    LeaveDetails.LeaveTypeId = request.LeaveTypeId;
                        //    LeaveDetails.StartDate = request.StartDate;
                        //    LeaveDetails.EndDate = request.EndDate;
                        //    //LeaveDetails.StartTime =request.StartTime;
                        //    //LeaveDetails.EndTime = request.EndTime;
                        //    LeaveDetails.Reason = request.Reason;
                        //    LeaveDetails.NoOfDays = 18;
                        //    LeaveDetails.AppliedByUserId = userId;
                        //    LeaveDetails.ApprovedByUserId = userId;
                        //    LeaveDetails.LeaveStatusId = 1;
                        //    LeaveDetails.LeaveBalance = 24;
                        //    LeaveDetails.UpdatedBy = userId;
                        //    LeaveDetails.UpdatedDate = _commonHelper.GetCurrentDateTime();

                        //    _dbContext.Entry(LeaveDetails).State = EntityState.Modified;
                        //    await _dbContext.SaveChangesAsync();

                        //    response.Status = true;
                        //    response.StatusCode = System.Net.HttpStatusCode.OK;
                        //    response.Message = "Leave updated successfully!";
                        //}
                        //else
                        //{
                        //    response.Message = "Leave not found!";
                        //    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                        //} 
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public decimal GetNoOfDays(DateTime StartDate, DateTime EndDate, string StartTime, string EndTime)
        {
            decimal isGetNoOfDays = 0;


            if (StartDate == EndDate) // leave days count for single day
            {
                if ((StartTime == "09:30 AM") && (EndTime == "06:30 PM"))
                {
                    isGetNoOfDays = decimal.Parse("1");
                }
                else if ((StartTime == "09:30 AM") && (EndTime == "02:00 PM"))
                {
                    isGetNoOfDays = decimal.Parse("0.5");
                }
                else if ((StartTime == "02:00 PM") && (EndTime == "06:30 PM"))
                {
                    isGetNoOfDays = decimal.Parse("0.5");
                }
            }
            else // leave days count for multiple days
            {
                if (EndDate > StartDate)
                {
                    for (DateTime i = StartDate; i <= EndDate; i = i.Date.AddDays(1))
                    {
                        if (i.Date == StartDate)
                        {
                            if (StartTime == "09:30 AM")
                            {
                                isGetNoOfDays += decimal.Parse("1");
                            }
                            else
                            {
                                isGetNoOfDays += decimal.Parse("0.5");
                            }
                        }
                        else if (i.Date == EndDate)
                        {
                            if (EndTime == "02:00 PM")
                            {
                                isGetNoOfDays += decimal.Parse("0.5");
                            }
                            else
                            {
                                isGetNoOfDays += decimal.Parse("1");
                            }
                        }
                        else
                        {
                            isGetNoOfDays += decimal.Parse("1");
                        }
                    }
                }
            }

            return isGetNoOfDays;
        }

        public decimal GetProbationPeriod(DateTime StartDate, DateTime EndDate)
        {
            decimal isProbationPeriodDays = 0;
            var span = EndDate - StartDate; //return timespan
            isProbationPeriodDays = span.Days; //return days
            return isProbationPeriodDays;
        }
        private CommonResponse SandwichLeave(AddUpdateLeaveReqModel model)
        {
            CommonResponse response = new CommonResponse();
            decimal noofdays = 0;
            int userId = _commonHelper.GetLoggedInUserId();
            #region calculate no of days based on start date time and end date time
            if (model.StartDate.Date == model.EndDate.Date) // leave days count for single day
            {
                if ((model.StartTime == "09:30 AM") && (model.EndTime == "06:30 PM"))
                {
                    noofdays = decimal.Parse("1");
                }
                else if ((model.StartTime == "09:30 AM") && (model.EndTime == "02:00 PM"))
                {
                    noofdays = decimal.Parse("0.5");
                }
                else if ((model.StartTime == "02:00 PM") && (model.EndTime == "06:30 PM"))
                {
                    noofdays = decimal.Parse("0.5");
                }
            }
            else // leave days count for multiple days
            {
                if (model.EndDate.Date > model.StartDate.Date)
                {
                    for (DateTime i = model.StartDate.Date; i <= model.EndDate.Date; i = i.Date.AddDays(1))
                    {
                        if (i.Date == model.StartDate.Date)
                        {
                            if (model.StartTime == "09:30 AM")
                            {
                                noofdays += decimal.Parse("1");
                            }
                            else
                            {
                                noofdays += decimal.Parse("0.5");
                            }
                        }
                        else if (i.Date == model.EndDate.Date)
                        {
                            if (model.EndTime == "02:00 PM")
                            {
                                noofdays += decimal.Parse("0.5");
                            }
                            else
                            {
                                noofdays += decimal.Parse("1");
                            }
                        }
                        else
                        {
                            noofdays += decimal.Parse("1");
                        }
                    }
                }
            }
            #endregion



            var leavemaster = _dbRepo.LeaveLists().Where(x => x.AppliedByUserId == userId).FirstOrDefault();
            if (noofdays > 1)
            {
                if (model.StartDate.DayOfWeek.ToString() == "Friday" && model.StartTime == "09:30 AM" && model.EndDate.DayOfWeek.ToString() == "Monday" && model.EndTime == "06:30 PM")
                {
                    noofdays = 4;

                    if (noofdays >= 4)
                    {
                        //code if apply leave for 4 or more than 4 days
                        var BalanceList = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId).OrderByDescending(x => x.BalanceDate).Take(1).FirstOrDefault();

                        if (BalanceList.ClosingLeaveBalance >= noofdays)
                        {
                            leavemaster.PaidDays = noofdays;
                            leavemaster.UnPaidDays = decimal.Parse("0");
                        }
                        else
                        {
                            leavemaster.PaidDays = BalanceList.ClosingLeaveBalance;
                            leavemaster.UnPaidDays = noofdays - leavemaster.PaidDays;
                        }
                    }
                }
                response.Data = leavemaster;

            }

            return response;
        }
    }
}
