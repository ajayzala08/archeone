using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
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
                LeavesListResModel leavesListResModel = new LeavesListResModel();
                var userId = _commonHelper.GetLoggedInUserId();
                var LeaveList = _dbRepo.LeaveLists();
                var LeaveBlanceList = _dbRepo.LeaveBalanceLists();
                var UserList = _dbRepo.UserMstList();
                var LeaveStatusList = _dbRepo.LeaveStatusLists();
                var LeaveTypeList = _dbRepo.LeaveTypeLists();




                leavesListResModel.LeaveDetailsLists = new List<LeaveDetailsList>();

                var hrRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("HR")).ToList();
                var hrroleIdList = hrRoleList.Select(x => x.Id).ToList();

                var isUserExist = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == userId);

                var ReportingManager = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.ReportingManager == userId);

                var employeeRoleList = _dbRepo.RoleMstList().Where(x => !x.RoleCode.Contains("HR")).ToList();
                var employeeroleIdList = employeeRoleList.Select(x => x.Id).ToList();


                var loginUserList = _dbRepo.AllUserMstList().Where(x => x.RoleId != null && x.Id == userId);


                var IsUserHR = loginUserList.Where(x => hrroleIdList.Contains(x.RoleId.Value)).ToList();
                var IsUserEmployee = loginUserList.Where(x => employeeroleIdList.Contains(x.RoleId.Value)).ToList();

                bool IsUserHR1 = IsUserHR.Count > 0;
                bool ReportingManager1 = ReportingManager != null;
                bool IsUserEmployee1 = IsUserEmployee.Count > 0;




                var list = await LeaveList.ToListAsync();


                if (IsUserHR1)
                {

                    list = await LeaveList.OrderBy(x => x.Id).ToListAsync();
                }
                else if (ReportingManager1)
                {

                    list = await LeaveList.OrderBy(x => x.Id).ToListAsync();
                }
                else if (IsUserEmployee1)
                {
                    list = await LeaveList.Where(x => x.AppliedByUserId == userId).OrderBy(x => x.Id).ToListAsync();
                }

                foreach (var item in list)
                {
                    var LeaveBalanceList = new List<LeaveBalanceMst>();


                    if (IsUserHR1)
                    {

                        LeaveBalanceList = await LeaveBlanceList.Where(x => x.UserId == item.AppliedByUserId).OrderBy(x => x.Id).ToListAsync();
                    }
                    else if (ReportingManager1)
                    {
                        LeaveBalanceList = await LeaveBlanceList.Where(x => x.UserId == item.AppliedByUserId).OrderBy(x => x.Id).ToListAsync();
                    }
                    else if (IsUserEmployee1)
                    {
                        LeaveBalanceList = await LeaveBlanceList.Where(x => x.UserId == userId).OrderBy(x => x.Id).ToListAsync();
                    }

                    var BalanceMonth = item.StartDate.ToString("MMMM");
                    var AppliedByUserList1 = await UserList.FirstOrDefaultAsync(x => x.Id == item.AppliedByUserId);
                    var ApprovedByHruserId = await UserList.FirstOrDefaultAsync(x => x.Id == item.ApprovedByHruserId);
                    var ApprovedByReportingUserId = await UserList.FirstOrDefaultAsync(x => x.Id == item.ApprovedByReportingUserId);
                    var LeaveStatusList1 = await LeaveStatusList.FirstOrDefaultAsync(x => x.Id == item.LeaveStatusId);
                    var HrStatus = await LeaveStatusList.FirstOrDefaultAsync(x => x.Id == item.Hrstatus);
                    var ApprovedByReportingStatus = await LeaveStatusList.FirstOrDefaultAsync(x => x.Id == item.ApprovedByReportingStatus);
                    var LeaveTypeList1 = await LeaveTypeList.FirstOrDefaultAsync(x => x.Id == item.LeaveTypeId);
                    var LeaveBalanceList1 = LeaveBalanceList.Where(x => x.BalanceMonth == BalanceMonth).FirstOrDefault();


                    LeaveDetailsList leaveDetailsListModel = new LeaveDetailsList();


                    leaveDetailsListModel.LeaveTypeName = LeaveTypeList1.LeaveTypeName;
                    leaveDetailsListModel.Id = item.Id;
                    leaveDetailsListModel.AppliedByUserName = AppliedByUserList1.UserName;
                    leaveDetailsListModel.ApprovedByReportingUserId = ApprovedByReportingUserId == null ? "" : ApprovedByReportingUserId.UserName;
                    leaveDetailsListModel.ApprovedByHRUserId = ApprovedByHruserId == null ? "" : ApprovedByHruserId.UserName;
                    leaveDetailsListModel.StartDate = item.StartDate;
                    leaveDetailsListModel.EndDate = item.EndDate;
                    leaveDetailsListModel.StartTime = item.StartTime;
                    leaveDetailsListModel.EndTime = item.EndTime;
                    leaveDetailsListModel.OpeningBalance = Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)item.OpeningLeaveBalance));
                    leaveDetailsListModel.ClosingBalance = Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceList1.ClosingLeaveBalance));
                    leaveDetailsListModel.NoOfDays = Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)item.NoOfDays));
                    leaveDetailsListModel.PaidDays = Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)item.PaidDays));
                    leaveDetailsListModel.UnPaidDays = Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)item.UnPaidDays));
                    leaveDetailsListModel.Reason = item.Reason;
                    leaveDetailsListModel.LeaveStatus = LeaveStatusList1 == null ? "Pending" : LeaveStatusList1.LeaveStatus;
                    leaveDetailsListModel.BalanceMonth = LeaveBalanceList1.BalanceMonth;
                    leaveDetailsListModel.BalanceYear = Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceList1.BalanceYear));
                    leaveDetailsListModel.SickLeaveBalance = Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceList1.SickLeaveBalance));
                    leaveDetailsListModel.SickLeaveTaken = Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceList1.SickLeaveTaken));
                    leaveDetailsListModel.CasualLeaveBalance = Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceList1.CasualLeaveBalance));
                    leaveDetailsListModel.CasualLeaveTaken = Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceList1.CasualLeaveTaken));
                    leaveDetailsListModel.EarnedLeaveBalance = Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceList1.EarnedLeaveBalance));
                    leaveDetailsListModel.EarnedLeaveTaken = Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceList1.EarnedLeaveTaken));
                    leaveDetailsListModel.Details = LeaveBalanceList1.Detail;
                    leaveDetailsListModel.LeaveTaken = Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceList1.LeaveTaken));
                    leaveDetailsListModel.HrStatus = HrStatus == null ? "pending" : HrStatus.LeaveStatus;
                    leaveDetailsListModel.ApprovedByReportingStatus = ApprovedByReportingStatus == null ? "pending" : ApprovedByReportingStatus.LeaveStatus;



                    if (leaveDetailsListModel.HrStatus.ToLower() == "approve" && leaveDetailsListModel.ApprovedByReportingStatus.ToLower() == "approve")
                    {
                        leaveDetailsListModel.EditDisable = true;
                    }

                    leavesListResModel.LeaveDetailsLists.Add(leaveDetailsListModel);

                }





                if (leavesListResModel != null)
                {
                    response.Data = leavesListResModel;
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
            return View(response);
        }

        public async Task<IActionResult> AddEditLeave(int Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var userId = _commonHelper.GetLoggedInUserId();
                LeaveAddEditReqModel leaveAddEditReqModel = new LeaveAddEditReqModel();
                leaveAddEditReqModel.leaveDetails = new LeaveDetails();
                leaveAddEditReqModel.leaveTypeList = await _dbRepo.LeaveTypeLists().Where(x => x.IsCurrentYear == true).ToListAsync();
                bool LeaveStatusChangeView = false;


                var joiningDate = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == userId);
                var startTimeList = new List<KeyValueModel>();
                startTimeList.Add(new KeyValueModel { Id = 1, Name = "09:30 AM" });
                startTimeList.Add(new KeyValueModel { Id = 2, Name = "02:00 PM" });
                leaveAddEditReqModel.StartTimeList = startTimeList;

                var hrRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("HR")).ToList();
                var hrroleIdList = hrRoleList.Select(x => x.Id).ToList();

                var loginUserList = _dbRepo.AllUserMstList().Where(x => x.RoleId != null && x.Id == _commonHelper.GetLoggedInUserId());
                var IsUserHR = loginUserList.Where(x => hrroleIdList.Contains(x.RoleId.Value)).ToList();

                if (Id > 0)
                {
                    var leaveDetails = await _dbRepo.LeaveLists().FirstOrDefaultAsync(x => x.Id == Id);
                    var loginId = _commonHelper.GetLoggedInUserId();
                    var reportingManager = new UserDetailsMst();

                    if (leaveDetails.AppliedByUserId == loginId)
                    {
                        reportingManager = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == leaveDetails.AppliedByUserId);

                        if (reportingManager.ReportingManager == loginId)
                        {
                            LeaveStatusChangeView = true;
                        }
                        {
                            LeaveStatusChangeView = false;
                        }
                    }
                    else
                    {
                        reportingManager = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == leaveDetails.AppliedByUserId);
                        if (reportingManager.ReportingManager == loginId)
                        {
                            LeaveStatusChangeView = true;
                        }
                        else
                        {
                            LeaveStatusChangeView = false;
                        }
                    }

                    if (leaveDetails != null)
                    {
                        leaveAddEditReqModel.leaveDetails.Id = leaveDetails.Id;
                        leaveAddEditReqModel.leaveDetails.LeaveTypeId = leaveDetails.LeaveTypeId;
                        leaveAddEditReqModel.leaveDetails.StartDate = leaveDetails.StartDate;
                        leaveAddEditReqModel.leaveDetails.EndDate = leaveDetails.EndDate;
                        leaveAddEditReqModel.leaveDetails.StartTime = leaveDetails.StartTime;
                        leaveAddEditReqModel.leaveDetails.EndTime = leaveDetails.EndTime;
                        leaveAddEditReqModel.leaveDetails.Reason = leaveDetails.Reason;
                        leaveAddEditReqModel.leaveDetails.LeaveStatusId = leaveDetails.LeaveStatusId;
                    }

                    if (IsUserHR.Count > 0)
                    {
                        foreach (var user in IsUserHR)
                        {

                            if (user.Id == loginId)
                            {
                                reportingManager = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == leaveDetails.AppliedByUserId);


                                var employeeRoleList = _dbRepo.RoleMstList().Where(x => !x.RoleCode.Contains("HR")).ToList();
                                var employeeroleIdList = employeeRoleList.Select(x => x.Id).ToList();


                                loginUserList = _dbRepo.AllUserMstList().Where(x => x.RoleId != null && x.Id == leaveDetails.AppliedByUserId);
                                var IsUserEmployee = loginUserList.Where(x => employeeroleIdList.Contains(x.RoleId.Value)).ToList();


                                if (reportingManager.ReportingManager == loginId)
                                {
                                    LeaveStatusChangeView = true;
                                    leaveAddEditReqModel.leaveDetails.ApprovedByReportingStatus = leaveDetails.ApprovedByReportingStatus == null ? 0 : (int)leaveDetails.ApprovedByReportingStatus;
                                }
                                else
                                {
                                    LeaveStatusChangeView = false;
                                }
                                if (IsUserEmployee.Count > 0)
                                {
                                    LeaveStatusChangeView = true;
                                    leaveAddEditReqModel.leaveDetails.HrStatus = leaveDetails.Hrstatus == null ? 0 : (int)leaveDetails.Hrstatus;
                                }
                            }
                            else
                            {
                                reportingManager = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == leaveDetails.AppliedByUserId);
                                if (reportingManager.ReportingManager == loginId)
                                {
                                    LeaveStatusChangeView = true;
                                    leaveAddEditReqModel.leaveDetails.ApprovedByReportingStatus = leaveDetails.ApprovedByReportingStatus == null ? 0 : (int)leaveDetails.ApprovedByReportingStatus;
                                }
                                else
                                {
                                    LeaveStatusChangeView = false;
                                }
                            }


                        }
                    }


                }
                else
                {
                    leaveAddEditReqModel.leaveDetails.StartDate = DateTime.Now;
                    leaveAddEditReqModel.leaveDetails.EndDate = DateTime.Now;


                }
                leaveAddEditReqModel.leaveDetails.JoiningDate = joiningDate == null ? DateTime.Now : joiningDate.JoinDate;
                leaveAddEditReqModel.LeaveStatusChangeView = LeaveStatusChangeView;
                leaveAddEditReqModel.LeaveStatusList = await _dbRepo.LeaveStatusLists().Select(x => new KeyValueModel { Id = x.Id, Name = x.LeaveStatus }).ToListAsync();

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
            return View(commonResponse);
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
                                        //leaveMst.ApprovedByHruserId = userId;
                                        // leaveMst.ApprovedByReportingUserId = userId;
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


                                                        var LeaveType1 = await _dbRepo.LeaveLists().Where(x => x.LeaveTypeId == request.LeaveTypeId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();

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
                                                        tbl.LeaveTaken = noOfDay;
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
                                                        if (BalanceMonth.ToLower() == "january")
                                                        {

                                                            LeaveBalanceMst tbl1 = new LeaveBalanceMst();
                                                            tbl1.UserId = userId;
                                                            tbl1.LeaveTypeId = request.LeaveTypeId;
                                                            tbl1.BalanceMonth = "January";//DateTime.Now.ToString("MMMM");
                                                            tbl1.BalanceYear = DateTime.Now.Year;
                                                            tbl1.OpeningLeaveBalance = 0;
                                                            tbl1.SickLeaveBalance = 0.5m;
                                                            tbl1.CasualLeaveBalance = 0.5m;
                                                            tbl1.EarnedLeaveBalance = 6;
                                                            tbl1.SickLeaveTaken = 0;
                                                            tbl1.CasualLeaveTaken = 0;
                                                            tbl1.EarnedLeaveTaken = 0;
                                                            tbl1.ClosingLeaveBalance = 1.5m;
                                                            tbl1.LeaveTaken = 0;
                                                            tbl1.BalanceDate = DateTime.Now;
                                                            tbl1.Detail = "Monthly Leave Balance";
                                                            tbl1.IsActive = true;
                                                            tbl1.IsDelete = false;
                                                            tbl1.CreatedBy = userId;
                                                            tbl1.UpdatedBy = userId;
                                                            tbl1.CreatedDate = _commonHelper.GetCurrentDateTime();
                                                            tbl1.UpdatedDate = _commonHelper.GetCurrentDateTime();


                                                            await _dbContext.LeaveBalanceMsts.AddAsync(tbl1);
                                                            await _dbContext.SaveChangesAsync();

                                                        }


                                                        var LeaveBalanceList1 = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId).OrderBy(x => x.Id).LastOrDefault();

                                                        LeaveBalanceMst tbl = new LeaveBalanceMst();
                                                        tbl.UserId = userId;
                                                        tbl.LeaveTypeId = request.LeaveTypeId;
                                                        tbl.BalanceMonth = i.Date.ToString("MMMM");
                                                        tbl.BalanceYear = i.Year;
                                                        tbl.NoOfDays = noOfDay;
                                                        tbl.OpeningLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.ClosingLeaveBalance;
                                                        tbl.SickLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.SickLeaveBalance + decimal.Parse("0.5");
                                                        tbl.CasualLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.CasualLeaveBalance + decimal.Parse("0.5");
                                                        tbl.EarnedLeaveBalance = 6;
                                                        tbl.SickLeaveTaken = 0;
                                                        tbl.CasualLeaveTaken = 0;
                                                        tbl.EarnedLeaveTaken = 0;
                                                        tbl.ClosingLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.ClosingLeaveBalance + decimal.Parse("1.5");
                                                        tbl.LeaveTaken = noOfDay;
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
                                            LeaveBalanceMst tbl1 = new LeaveBalanceMst();
                                            tbl1.UserId = userId;
                                            tbl1.LeaveTypeId = request.LeaveTypeId;
                                            tbl1.BalanceMonth = "January";//DateTime.Now.ToString("MMMM");
                                            tbl1.BalanceYear = DateTime.Now.Year;
                                            tbl1.OpeningLeaveBalance = 0;
                                            tbl1.SickLeaveBalance = 0.5m;
                                            tbl1.CasualLeaveBalance = 0.5m;
                                            tbl1.EarnedLeaveBalance = 6;
                                            tbl1.SickLeaveTaken = 0;
                                            tbl1.CasualLeaveTaken = 0;
                                            tbl1.EarnedLeaveTaken = 0;
                                            tbl1.ClosingLeaveBalance = 1.5m;
                                            tbl1.LeaveTaken = 0;
                                            tbl1.BalanceDate = DateTime.Now;
                                            tbl1.Detail = "Monthly Leave Balance";
                                            tbl1.IsActive = true;
                                            tbl1.IsDelete = false;
                                            tbl1.CreatedBy = userId;
                                            tbl1.UpdatedBy = userId;
                                            tbl1.CreatedDate = _commonHelper.GetCurrentDateTime();
                                            tbl1.UpdatedDate = _commonHelper.GetCurrentDateTime();


                                            await _dbContext.LeaveBalanceMsts.AddAsync(tbl1);
                                            await _dbContext.SaveChangesAsync();

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
                                                    tbl.NoOfDays = noOfDay;
                                                    tbl.OpeningLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.ClosingLeaveBalance;
                                                    tbl.SickLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.SickLeaveBalance + decimal.Parse("0.5");
                                                    tbl.CasualLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.CasualLeaveBalance + decimal.Parse("0.5");
                                                    tbl.EarnedLeaveBalance = 6;
                                                    tbl.SickLeaveTaken = 0;
                                                    tbl.CasualLeaveTaken = 0;
                                                    tbl.EarnedLeaveTaken = 0;
                                                    tbl.ClosingLeaveBalance = LeaveBalanceList1 == null ? 0 : LeaveBalanceList1.ClosingLeaveBalance + decimal.Parse("1.5");
                                                    tbl.LeaveTaken = noOfDay;
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

                                        var BalanceList = await _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();

                                        var LeaveType = await _dbRepo.LeaveTypeLists().Where(x => x.Id == request.LeaveTypeId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                                        if (BalanceList != null && LeaveType != null)
                                        {
                                            if (noOfDay >= 4)
                                            {


                                                if (LeaveType.LeaveTypeName.ToLower() == "sickleave")
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
                                                else if (LeaveType.LeaveTypeName.ToLower() == "casualleave")
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
                                                else if (LeaveType.LeaveTypeName.ToLower() == "earnedleave")
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
                                                if (LeaveType.LeaveTypeName.ToLower() == "sickleave")
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
                                                else if (LeaveType.LeaveTypeName.ToLower() == "casualleave")
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
                                                else if (LeaveType.LeaveTypeName.ToLower() == "earnedleave")
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
                                                if (LeaveType.LeaveTypeName.ToLower() == "sickleave")
                                                {
                                                    BalanceList.SickLeaveTaken = leaveMst.PaidDays;
                                                    BalanceList.CasualLeaveTaken = 0;
                                                    BalanceList.EarnedLeaveTaken = 0;
                                                }
                                                else if (LeaveType.LeaveTypeName.ToLower() == "casualleave")
                                                {
                                                    BalanceList.SickLeaveTaken = 0;
                                                    BalanceList.CasualLeaveTaken = leaveMst.PaidDays;
                                                    BalanceList.EarnedLeaveTaken = 0;
                                                }
                                                else if (LeaveType.LeaveTypeName.ToLower() == "earnedleave")
                                                {
                                                    BalanceList.SickLeaveTaken = 0;
                                                    BalanceList.CasualLeaveTaken = 0;
                                                    BalanceList.EarnedLeaveTaken = leaveMst.PaidDays;

                                                }
                                                _dbContext.Entry(BalanceList).State = EntityState.Modified;
                                                await _dbContext.SaveChangesAsync();

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

                                        await _dbContext.LeaveMsts.AddAsync(leaveMst);
                                        await _dbContext.SaveChangesAsync();

                                        var sandwichleave = SandwichLeave(request);



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


                        noOfDay = GetNoOfDays(request.StartDate, request.EndDate, request.StartTime, request.EndTime);
                        var hrRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("HR")).ToList();
                        var hrroleIdList = hrRoleList.Select(x => x.Id).ToList();


                        var managerRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("Project_Manager")).ToList();
                        var managerroleIdList = managerRoleList.Select(x => x.Id).ToList();

                        var employeeRoleList = _dbRepo.RoleMstList().Where(x => !x.RoleCode.Contains("Project_Manager") && !x.RoleCode.Contains("HR")).ToList();
                        var employeeroleIdList = employeeRoleList.Select(x => x.Id).ToList();

                        var loginUserList = _dbRepo.AllUserMstList().Where(x => x.RoleId != null && x.Id == _commonHelper.GetLoggedInUserId());

                        var IsUserHR = loginUserList.Where(x => hrroleIdList.Contains(x.RoleId.Value)).ToList();
                        var IsUserManager = loginUserList.Where(x => managerroleIdList.Contains(x.RoleId.Value)).ToList();
                        var IsUserEmployee = loginUserList.Where(x => employeeroleIdList.Contains(x.RoleId.Value)).ToList();




                        bool IsUserHR1 = IsUserHR.Count > 0 ? true : false;
                        bool IsUserManager1 = IsUserManager.Count > 0 ? true : false;
                        bool IsUserEmployee1 = IsUserEmployee.Count > 0 ? true : false;


                        var userJoiningDate = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == userId);
                        if (userJoiningDate != null)
                        {

                            decimal isProbationPeriodDays = 0;
                            bool isProbationPeriod = false;


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

                            var LeaveDetails = await _dbRepo.LeaveLists().FirstOrDefaultAsync(x => x.Id == request.Id);
                            if (LeaveDetails != null)
                            {
                                LeaveDetails.LeaveTypeId = request.LeaveTypeId;
                                LeaveDetails.StartDate = request.StartDate;
                                LeaveDetails.EndDate = request.EndDate;
                                LeaveDetails.StartTime = Convert.ToDateTime(request.StartTime).TimeOfDay;
                                LeaveDetails.EndTime = Convert.ToDateTime(request.EndTime).TimeOfDay;
                                LeaveDetails.Reason = request.Reason;
                                LeaveDetails.NoOfDays = noOfDay;
                                LeaveDetails.LeaveStatusId = request.LeaveStatusId;
                                LeaveDetails.UpdatedBy = userId;
                                LeaveDetails.UpdatedDate = _commonHelper.GetCurrentDateTime();


                                if (IsUserHR1)
                                {
                                    LeaveDetails.Hrstatus = request.LeaveStatusId;
                                    LeaveDetails.ApprovedByHruserId = userId;

                                }
                                else if (IsUserManager1)
                                {
                                    LeaveDetails.ApprovedByReportingStatus = request.LeaveStatusId;
                                    LeaveDetails.ApprovedByReportingUserId = userId;
                                }

                                _dbContext.Entry(LeaveDetails).State = EntityState.Modified;
                                await _dbContext.SaveChangesAsync();


                                var LeaveStatusApproved = await _dbRepo.LeaveStatusLists().FirstOrDefaultAsync(x => x.Id == LeaveDetails.LeaveStatusId);
                                var HrStatusStatusApproved = await _dbRepo.LeaveStatusLists().FirstOrDefaultAsync(x => x.Id == LeaveDetails.Hrstatus);
                                var ProjectManagerStatusApproved = await _dbRepo.LeaveStatusLists().FirstOrDefaultAsync(x => x.Id == LeaveDetails.ApprovedByReportingStatus);

                                if (LeaveStatusApproved != null && HrStatusStatusApproved != null && ProjectManagerStatusApproved != null)
                                {
                                    if (LeaveStatusApproved.LeaveStatus.ToLower() == "approve" && HrStatusStatusApproved.LeaveStatus.ToLower() == "approve" && ProjectManagerStatusApproved.LeaveStatus.ToLower() == "approve")
                                    {
                                        #region Calculation of deduction leave 




                                        var leaveBalanceList1 = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == LeaveDetails.AppliedByUserId).OrderBy(x => x.Id).LastOrDefault();


                                        if (leaveBalanceList1 != null)
                                        {
                                            if (isProbationPeriod)
                                            {


                                                var leaveType1 = await _dbRepo.LeaveTypeLists().Where(x => x.Id == request.LeaveTypeId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();


                                                //leaveBalanceList1.UserId = userId;
                                                leaveBalanceList1.LeaveTypeId = request.LeaveTypeId;
                                                leaveBalanceList1.BalanceMonth = leaveBalanceList1.BalanceMonth;
                                                leaveBalanceList1.BalanceYear = leaveBalanceList1.BalanceYear;
                                                leaveBalanceList1.NoOfDays = noOfDay;
                                                leaveBalanceList1.OpeningLeaveBalance = leaveBalanceList1 == null ? 0 : leaveBalanceList1.ClosingLeaveBalance;

                                                if (leaveType1.LeaveTypeName.ToLower() == "sickleave")
                                                {
                                                    leaveBalanceList1.SickLeaveBalance = leaveBalanceList1.SickLeaveTaken == 0 ? leaveBalanceList1.SickLeaveBalance : leaveBalanceList1.SickLeaveBalance - leaveBalanceList1.SickLeaveTaken;
                                                    leaveBalanceList1.ClosingLeaveBalance = leaveBalanceList1 == null ? 0 : leaveBalanceList1.ClosingLeaveBalance - leaveBalanceList1.SickLeaveTaken;
                                                    leaveBalanceList1.SickLeaveTaken = noOfDay;
                                                }
                                                else
                                                {
                                                    leaveBalanceList1.SickLeaveBalance = leaveBalanceList1 == null ? 0 : leaveBalanceList1.SickLeaveBalance;
                                                }

                                                if (leaveType1.LeaveTypeName.ToLower() == "casualleave")
                                                {
                                                    leaveBalanceList1.CasualLeaveBalance = leaveBalanceList1.CasualLeaveBalance == 0 ? leaveBalanceList1.CasualLeaveBalance : leaveBalanceList1.CasualLeaveBalance - leaveBalanceList1.CasualLeaveTaken;
                                                    leaveBalanceList1.ClosingLeaveBalance = leaveBalanceList1 == null ? 0 : leaveBalanceList1.ClosingLeaveBalance - leaveBalanceList1.CasualLeaveTaken;
                                                }
                                                else
                                                {
                                                    leaveBalanceList1.CasualLeaveBalance = leaveBalanceList1 == null ? 0 : leaveBalanceList1.CasualLeaveBalance;
                                                }


                                                if (leaveType1.LeaveTypeName.ToLower() == "earnedleave")
                                                {
                                                    leaveBalanceList1.EarnedLeaveBalance = leaveBalanceList1.EarnedLeaveBalance == 0 ? leaveBalanceList1.EarnedLeaveBalance : leaveBalanceList1.EarnedLeaveBalance - leaveBalanceList1.EarnedLeaveTaken;
                                                    leaveBalanceList1.ClosingLeaveBalance = leaveBalanceList1 == null ? 0 : leaveBalanceList1.ClosingLeaveBalance - leaveBalanceList1.EarnedLeaveTaken;
                                                }
                                                else
                                                {
                                                    leaveBalanceList1.EarnedLeaveBalance = leaveBalanceList1 == null ? 0 : leaveBalanceList1.EarnedLeaveBalance;
                                                }


                                                leaveBalanceList1.CasualLeaveTaken = leaveBalanceList1.CasualLeaveTaken;
                                                leaveBalanceList1.EarnedLeaveTaken = leaveBalanceList1.EarnedLeaveTaken;
                                                leaveBalanceList1.LeaveTaken = noOfDay;
                                                leaveBalanceList1.BalanceDate = DateTime.Now;
                                                leaveBalanceList1.Detail = "Monthly Leave Balance";
                                                leaveBalanceList1.IsActive = true;
                                                leaveBalanceList1.IsDelete = false;
                                                leaveBalanceList1.CreatedBy = userId;
                                                leaveBalanceList1.UpdatedBy = userId;
                                                leaveBalanceList1.CreatedDate = _commonHelper.GetCurrentDateTime();
                                                leaveBalanceList1.UpdatedDate = _commonHelper.GetCurrentDateTime();




                                                if (IsUserHR1)
                                                {
                                                    LeaveDetails.Hrstatus = request.LeaveStatusId;
                                                    LeaveDetails.ApprovedByHruserId = userId;

                                                }
                                                else if (IsUserManager1)
                                                {
                                                    LeaveDetails.ApprovedByReportingStatus = request.LeaveStatusId;
                                                    LeaveDetails.ApprovedByReportingUserId = userId;
                                                }

                                                _dbContext.Entry(leaveBalanceList1).State = EntityState.Modified;
                                                await _dbContext.SaveChangesAsync();


                                            }

                                        }
                                        else
                                        {
                                            response.Message = "Data Not Found!";

                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        response.Message = "Your Leave Not Approve";
                                    }
                                }

                            }





                            response.Status = true;
                            response.StatusCode = System.Net.HttpStatusCode.OK;
                            response.Message = "Leave updated successfully!";

                        }
                        else
                        {
                            response.Message = "Leave not found!";
                            response.StatusCode = System.Net.HttpStatusCode.NotFound;
                        }
                        #endregion

                    }
                }
                else
                {
                    response.Message = "Data not found";
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
            decimal noOfDays = 0;
            int userId = _commonHelper.GetLoggedInUserId();
            #region calculate no of days based on start date time and end date time
            if (model.StartDate.Date == model.EndDate.Date) // leave days count for single day
            {
                if ((model.StartTime == "09:30 AM") && (model.EndTime == "06:30 PM"))
                {
                    noOfDays = decimal.Parse("1");
                }
                else if ((model.StartTime == "09:30 AM") && (model.EndTime == "02:00 PM"))
                {
                    noOfDays = decimal.Parse("0.5");
                }
                else if ((model.StartTime == "02:00 PM") && (model.EndTime == "06:30 PM"))
                {
                    noOfDays = decimal.Parse("0.5");
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
                                noOfDays += decimal.Parse("1");
                            }
                            else
                            {
                                noOfDays += decimal.Parse("0.5");
                            }
                        }
                        else if (i.Date == model.EndDate.Date)
                        {
                            if (model.EndTime == "02:00 PM")
                            {
                                noOfDays += decimal.Parse("0.5");
                            }
                            else
                            {
                                noOfDays += decimal.Parse("1");
                            }
                        }
                        else
                        {
                            noOfDays += decimal.Parse("1");
                        }
                    }
                }
            }
            #endregion



            var leaveMaster = _dbRepo.LeaveLists().Where(x => x.AppliedByUserId == userId).FirstOrDefault();
            if (noOfDays > 1)
            {
                if (model.StartDate.DayOfWeek.ToString() == "Friday" && model.StartTime == "09:30 AM" && model.EndDate.DayOfWeek.ToString() == "Monday" && model.EndTime == "06:30 PM")
                {
                    noOfDays = 4;

                    if (noOfDays >= 4)
                    {
                        //code if apply leave for 4 or more than 4 days
                        var BalanceList = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId).OrderByDescending(x => x.Id).FirstOrDefault();

                        if (BalanceList.ClosingLeaveBalance >= noOfDays)
                        {
                            leaveMaster.PaidDays = noOfDays;
                            leaveMaster.UnPaidDays = decimal.Parse("0");
                        }
                        else
                        {
                            leaveMaster.PaidDays = BalanceList.ClosingLeaveBalance;
                            leaveMaster.UnPaidDays = noOfDays - leaveMaster.PaidDays;
                        }
                    }
                }
                response.Data = leaveMaster;

            }

            return response;
        }
    }
}
