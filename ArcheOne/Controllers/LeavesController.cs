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

                                    if (request.StartDate < request.EndDate)
                                    {

                                        noOfDay = GetNoOfDays(request.StartDate, request.EndDate, request.StartTime, request.EndTime);


                                        leaveMst.LeaveTypeId = request.LeaveTypeId;
                                        leaveMst.Reason = request.Reason;
                                        leaveMst.StartDate = request.StartDate;
                                        leaveMst.StartTime = Convert.ToDateTime(request.StartTime).TimeOfDay;
                                        leaveMst.EndDate = request.EndDate;
                                        leaveMst.EndTime = Convert.ToDateTime(request.EndTime).TimeOfDay;
                                        leaveMst.AppliedByUserId = userId;
                                        leaveMst.ApprovedByUserId = userId;
                                        leaveMst.LeaveStatusId = 1;
                                        leaveMst.IsActive = true;
                                        leaveMst.IsDelete = false;
                                        leaveMst.CreatedBy = userId;
                                        leaveMst.UpdatedBy = userId;
                                        leaveMst.CreatedDate = _commonHelper.GetCurrentDateTime();
                                        leaveMst.UpdatedDate = _commonHelper.GetCurrentDateTime();






                                        var LeaveBalanceList = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId).ToList();
                                        if (LeaveBalanceList.Count > 0)
                                        {

                                        }
                                        else
                                        {
                                            LeaveBalanceMst tbl = new LeaveBalanceMst();
                                            tbl.UserId = userId;
                                            tbl.LeaveTypeId = request.LeaveTypeId;
                                            tbl.BalanceMonth = "January";//DateTime.Now.ToString("MMMM");
                                            tbl.BalanceYear = DateTime.Now.Year;
                                            tbl.NoOfDays = 0;
                                            tbl.OpeningLeaveBalance = 0;
                                            tbl.SickLeaveBalance = 6;
                                            tbl.CasualLeaveBalance = 6;
                                            tbl.EarnedLeaveBalance = 6;
                                            tbl.SickLeaveTaken = 0;
                                            tbl.CasualLeaveTaken = 0;
                                            tbl.EarnedLeaveTaken = 0;
                                            tbl.ClosingLeaveBalance = 0;
                                            tbl.LeaveTaken = 0;
                                            tbl.BalanceDate = DateTime.Now;
                                            tbl.Detail = "Detail";
                                            tbl.IsActive = true;
                                            tbl.IsDelete = false;
                                            tbl.CreatedBy = userId;
                                            tbl.UpdatedBy = userId;
                                            tbl.CreatedDate = _commonHelper.GetCurrentDateTime();
                                            tbl.UpdatedDate = _commonHelper.GetCurrentDateTime();

                                            await _dbContext.LeaveBalanceMsts.AddAsync(tbl);
                                            _dbContext.SaveChanges();
                                        }


                                        string currentmonth = DateTime.Now.ToString("MMMM");
                                        int currentyear = DateTime.Now.Year;

                                        var IsEarnAdded = await _dbRepo.LeaveBalanceLists().FirstOrDefaultAsync(x => x.BalanceMonth == currentmonth && x.BalanceYear == currentyear && x.UserId == userId && x.Detail == "Monthly Leave Balance");
                                        if (IsEarnAdded == null)
                                        {
                                            var FirstBalanceList = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId).OrderByDescending(x => x.BalanceDate).Take(1).FirstOrDefault();
                                            if (FirstBalanceList != null)
                                            {

                                                LeaveBalanceMst tbl = new LeaveBalanceMst();
                                                tbl.BalanceMonth = currentmonth;
                                                tbl.BalanceYear = currentyear;
                                                tbl.OpeningLeaveBalance = FirstBalanceList.ClosingLeaveBalance; ;
                                                tbl.SickLeaveBalance = 6;
                                                tbl.CasualLeaveBalance = 6;
                                                tbl.EarnedLeaveBalance = 6;
                                                tbl.SickLeaveTaken = 0;
                                                tbl.CasualLeaveTaken = 0;
                                                tbl.EarnedLeaveTaken = 0;
                                                tbl.ClosingLeaveBalance = FirstBalanceList.ClosingLeaveBalance + decimal.Parse("1.5");
                                                tbl.BalanceDate = DateTime.Now;
                                                tbl.LeaveTaken = decimal.Parse("0");
                                                tbl.UserId = userId;
                                                tbl.LeaveTypeId = request.LeaveTypeId;
                                                tbl.NoOfDays = 0;
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





                                        if (noOfDay >= 4)
                                        {
                                            //code if apply leave for 4 or more than 4 days
                                            var BalanceList = await _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId).OrderByDescending(x => x.BalanceDate).FirstOrDefaultAsync();

                                            if (BalanceList.ClosingLeaveBalance >= noOfDay)
                                            {
                                                leaveMst.PaidDays = noOfDay;
                                                leaveMst.UnPaidDays = decimal.Parse("0");
                                            }
                                            else
                                            {
                                                leaveMst.PaidDays = BalanceList.ClosingLeaveBalance;
                                                leaveMst.UnPaidDays = noOfDay - leaveMst.PaidDays;
                                            }
                                        }
                                        else
                                        {
                                            string cMonth = DateTime.Now.ToString("MMMM");
                                            int cYear = DateTime.Now.Year;
                                            string pMonth = DateTime.Now.AddMonths(-1).ToString("MMMM");
                                            int pYear = DateTime.Now.AddMonths(-1).Year;

                                            //code if apply leave for less than 4 days

                                            decimal paidleave = 0, unpaidleave = 0;
                                            var CurrentMonthBalance = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId && x.BalanceMonth == cMonth && x.BalanceYear == cYear && x.Detail == "Use").OrderByDescending(x => x.BalanceDate).Take(1).FirstOrDefault();


                                            if (CurrentMonthBalance != null)
                                            {
                                                decimal currentmonthleavebalance = decimal.Parse("1.5") - decimal.Parse(CurrentMonthBalance.LeaveTaken.ToString());
                                                if (currentmonthleavebalance > 0)
                                                {
                                                    if (currentmonthleavebalance >= noOfDay)
                                                    {
                                                        paidleave = currentmonthleavebalance;

                                                        unpaidleave = noOfDay - paidleave;
                                                        if (unpaidleave > 0)
                                                        {
                                                            var PreviousMonthBalance = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId && x.BalanceMonth == pMonth && x.BalanceYear == pYear && x.Detail == "Monthly Leave Balance").OrderByDescending(x => x.BalanceDate).Take(1).FirstOrDefault();
                                                            if (PreviousMonthBalance != null)
                                                            {
                                                                decimal previousmonthleavebalance = decimal.Parse("1.5") - decimal.Parse(PreviousMonthBalance.LeaveTaken.ToString());
                                                                if (previousmonthleavebalance >= unpaidleave)
                                                                {
                                                                    paidleave = paidleave + unpaidleave;
                                                                    unpaidleave = noOfDay - paidleave;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var PreviousMonthBalance = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId && x.BalanceMonth == pMonth && x.BalanceYear == pYear && x.Detail == "Monthly Leave Balance").OrderByDescending(x => x.BalanceDate).Take(1).FirstOrDefault();
                                                        if (PreviousMonthBalance != null)
                                                        {
                                                            decimal previousmonthleavebalance = decimal.Parse("1.5") - decimal.Parse(PreviousMonthBalance.LeaveTaken.ToString());
                                                            if (previousmonthleavebalance >= noOfDay)
                                                            {
                                                                paidleave = noOfDay;
                                                                unpaidleave = decimal.Parse("0");
                                                            }
                                                            else
                                                            {
                                                                paidleave = previousmonthleavebalance;
                                                                unpaidleave = noOfDay - previousmonthleavebalance;
                                                            }

                                                        }
                                                    }
                                                }
                                                else
                                                {

                                                }
                                            }

                                            else
                                            {
                                                var CurrentMonthBalanceEarn = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId && x.BalanceMonth == cMonth && x.BalanceYear == cYear && x.Detail == "Monthly Leave Balance").OrderByDescending(x => x.BalanceDate).Take(1).FirstOrDefault();
                                                if (CurrentMonthBalanceEarn != null)
                                                {
                                                    if (CurrentMonthBalanceEarn.ClosingLeaveBalance >= noOfDay)
                                                    {
                                                        paidleave = noOfDay;
                                                        unpaidleave = decimal.Parse("0");
                                                    }
                                                    else
                                                    {
                                                        paidleave = decimal.Parse(CurrentMonthBalanceEarn.ClosingLeaveBalance.ToString());
                                                        unpaidleave = noOfDay - paidleave;
                                                        if (unpaidleave > 0)
                                                        {
                                                            var previousmonthusebalance = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId && x.BalanceMonth == pMonth && x.BalanceYear == pYear && x.Detail == "Use").OrderByDescending(x => x.BalanceDate).Take(1).FirstOrDefault();
                                                            if (previousmonthusebalance != null)
                                                            {
                                                                if (previousmonthusebalance.ClosingLeaveBalance >= unpaidleave)
                                                                {
                                                                    paidleave = paidleave + unpaidleave;
                                                                    unpaidleave = decimal.Parse("0");
                                                                }
                                                                else
                                                                {
                                                                    paidleave = paidleave + decimal.Parse(previousmonthusebalance.ClosingLeaveBalance.ToString());
                                                                    unpaidleave = unpaidleave - decimal.Parse(previousmonthusebalance.ClosingLeaveBalance.ToString());
                                                                }
                                                            }
                                                            else
                                                            {

                                                            }
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    var CurrentMonthBalanceUsed = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId && x.BalanceMonth == cMonth && x.BalanceYear == cYear && x.Detail == "Use").OrderByDescending(x => x.BalanceDate).Take(1).FirstOrDefault();
                                                    if (CurrentMonthBalanceUsed != null)
                                                    {
                                                        if (CurrentMonthBalanceUsed.ClosingLeaveBalance > noOfDay)
                                                        {
                                                            paidleave = noOfDay;
                                                            unpaidleave = decimal.Parse("0");
                                                        }
                                                        else
                                                        {
                                                            paidleave = decimal.Parse(CurrentMonthBalanceUsed.ClosingLeaveBalance.ToString());
                                                            unpaidleave = noOfDay - decimal.Parse(CurrentMonthBalanceUsed.ClosingLeaveBalance.ToString());
                                                            if (unpaidleave > 0)
                                                            {
                                                                var previousmonthbalanceUsed = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId && x.BalanceMonth == pMonth && x.BalanceYear == pYear && x.Detail == "Use").OrderByDescending(x => x.BalanceDate).Take(1).FirstOrDefault();
                                                                if (previousmonthbalanceUsed != null)
                                                                {
                                                                    if (previousmonthbalanceUsed.ClosingLeaveBalance >= unpaidleave)
                                                                    {
                                                                        paidleave = paidleave + unpaidleave;
                                                                        unpaidleave = decimal.Parse("0");
                                                                    }
                                                                    else
                                                                    {
                                                                        paidleave = paidleave + decimal.Parse(previousmonthbalanceUsed.ClosingLeaveBalance.ToString());
                                                                        unpaidleave = unpaidleave - decimal.Parse(previousmonthbalanceUsed.ClosingLeaveBalance.ToString());
                                                                    }
                                                                }
                                                                else
                                                                {

                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var currentMonthBalaceEarn = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId && x.BalanceMonth == cMonth && x.BalanceYear == cYear && x.Detail == "Monthly Leave Balance").OrderByDescending(x => x.BalanceDate).Take(1).FirstOrDefault();
                                                        if (currentMonthBalaceEarn != null)
                                                        {
                                                            if (currentMonthBalaceEarn.ClosingLeaveBalance >= unpaidleave)
                                                            {
                                                                paidleave = paidleave + unpaidleave;
                                                                unpaidleave = decimal.Parse("0");
                                                            }
                                                            else
                                                            {
                                                                paidleave = paidleave + decimal.Parse(currentMonthBalaceEarn.ClosingLeaveBalance.ToString());
                                                                unpaidleave = unpaidleave - decimal.Parse(currentMonthBalaceEarn.ClosingLeaveBalance.ToString());
                                                                if (unpaidleave > 0)
                                                                {
                                                                    var previousMonthBalaceEarn = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId && x.BalanceMonth == pMonth && x.BalanceYear == pYear && x.Detail == "Monthly Leave Balance").OrderByDescending(x => x.BalanceDate).Take(1).FirstOrDefault();
                                                                    if (previousMonthBalaceEarn != null)
                                                                    {
                                                                        if (previousMonthBalaceEarn.ClosingLeaveBalance >= unpaidleave)
                                                                        {
                                                                            paidleave = paidleave + unpaidleave;
                                                                            unpaidleave = decimal.Parse("0");
                                                                        }
                                                                        else
                                                                        {

                                                                            paidleave = paidleave + decimal.Parse(previousMonthBalaceEarn.ClosingLeaveBalance.ToString());
                                                                            unpaidleave = noOfDay - paidleave;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                }
                                            }

                                            leaveMst.PaidDays = paidleave;
                                            leaveMst.UnPaidDays = unpaidleave;
                                        }



                                        var sandwichleave = SandwichLeave(request);


                                        if (isProbationPeriod == false)
                                        {
                                            leaveMst.PaidDays = 0;
                                            leaveMst.UnPaidDays = noOfDay;

                                        }

                                        transactionScope.Complete();
                                        await _dbContext.LeaveMsts.AddAsync(leaveMst);
                                        await _dbContext.SaveChangesAsync();

                                    }

                                }
                                else
                                {
                                    response.Message = "Please select valid startdate";
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
                if ((model.StartTime == "09:30") && (model.EndTime == "06:30"))
                {
                    noofdays = decimal.Parse("1");
                }
                else if ((model.StartTime == "09:30") && (model.EndTime == "02:00"))
                {
                    noofdays = decimal.Parse("0.5");
                }
                else if ((model.StartTime == "02:00") && (model.EndTime == "06:30"))
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
                            if (model.StartTime == "09:30")
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
                            if (model.EndTime == "02:00")
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
                if (model.StartDate.DayOfWeek.ToString() == "Friday" && model.StartTime == "09:30" && model.EndDate.DayOfWeek.ToString() == "Monday" && model.EndTime == "06:30")
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
