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
                var BalanceList = await GetPerMonthBalanceAsync();


                /*var userId = _commonHelper.GetLoggedInUserId();
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


                var IsUserHR = loginUserList.Where(x => hrroleIdList.Contains(x.RoleId)).ToList();
                var IsUserEmployee = loginUserList.Where(x => employeeroleIdList.Contains(x.RoleId)).ToList();

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
                }*/
                var userId = _commonHelper.GetLoggedInUserId();

                CommonResponse departmentResponse = await new CommonController(_dbRepo, _dbContext, _commonHelper).GetDepartmentByUserId(userId);

                string departmentCode = string.Empty;
                bool IsUserHR = false;
                if (departmentResponse.Status)
                {
                    departmentCode = departmentResponse.Data.DepartmentCode;
                    IsUserHR = departmentCode == CommonEnums.DepartmentMst.Human_Resource.ToString();
                }

                var userListByReportingManagerId = await _dbRepo.UserDetailList().Where(x => x.ReportingManager == userId).Select(x => x.UserId).ToListAsync();

                var leaveList = new List<LeaveMst>();
                if (IsUserHR)
                {
                    leaveList = await _dbRepo.LeaveLists().OrderBy(x => x.Id).ToListAsync();
                }
                else if (userListByReportingManagerId.Count > 0)
                {
                    leaveList = await _dbRepo.LeaveLists().Where(x => x.AppliedByUserId == userId || userListByReportingManagerId.Contains(x.AppliedByUserId)).ToListAsync();
                }
                else
                {
                    leaveList = await _dbRepo.LeaveLists().Where(x => x.AppliedByUserId == userId).OrderBy(x => x.Id).ToListAsync();
                }
                if (leaveList.Count > 0)
                {
                    leavesListResModel.LeaveDetailsLists = new List<LeaveDetailsList>();
                    foreach (var item in leaveList)
                    {
                        var LeaveBalanceList = new List<LeaveBalanceMst>();
                        // who can check leave balance? manager
                        if (userListByReportingManagerId.Count > 0)
                        {
                            LeaveBalanceList = await _dbRepo.LeaveBalanceLists().Where(x => x.UserId == item.AppliedByUserId).OrderByDescending(x => x.Id).ToListAsync();
                        }
                        else if (IsUserHR)
                        {
                            LeaveBalanceList = await _dbRepo.LeaveBalanceLists().Where(x => x.UserId == item.AppliedByUserId).OrderByDescending(x => x.Id).ToListAsync();
                        }
                        else
                        {
                            LeaveBalanceList = await _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId).OrderByDescending(x => x.Id).ToListAsync();
                        }


                        var BalanceMonth = item.StartDate.ToString("MMMM");
                        var AppliedByUser = await _dbRepo.UserMstList().FirstOrDefaultAsync(x => x.Id == item.AppliedByUserId);
                        var ApprovedByHrUserName = await _dbRepo.UserMstList().FirstOrDefaultAsync(x => x.Id == item.ApprovedByHruserId);
                        var ApprovedByReportingUserName = await _dbRepo.UserMstList().FirstOrDefaultAsync(x => x.Id == item.ApprovedByReportingUserId);
                        var LeaveStatusName = await _dbRepo.LeaveStatusLists().FirstOrDefaultAsync(x => x.Id == item.LeaveStatusId);
                        var CancelLeaveStatus = await _dbRepo.LeaveStatusLists().FirstOrDefaultAsync(x => x.Id == item.LeaveStatusId);
                        var HrStatus = await _dbRepo.LeaveStatusLists().FirstOrDefaultAsync(x => x.Id == item.Hrstatus);
                        var ApprovedByReportingStatus = await _dbRepo.LeaveStatusLists().FirstOrDefaultAsync(x => x.Id == item.ApprovedByReportingStatus);
                        var LeaveTypeName = await _dbRepo.LeaveTypeLists().FirstOrDefaultAsync(x => x.Id == item.LeaveTypeId);
                        var LeaveBalanceDetails = LeaveBalanceList.OrderByDescending(x => x.Id).FirstOrDefault(x => x.BalanceMonth == BalanceMonth && x.LeaveTypeId == item.LeaveTypeId);

                        LeaveDetailsList leaveDetailsListModel = new LeaveDetailsList();

                        leaveDetailsListModel.LeaveTypeName = LeaveTypeName.LeaveTypeName;
                        leaveDetailsListModel.Id = item.Id;
                        leaveDetailsListModel.AppliedByUserName = AppliedByUser.UserName;
                        leaveDetailsListModel.ApprovedByReportingUserId = ApprovedByReportingUserName == null ? "" : ApprovedByReportingUserName.UserName;
                        leaveDetailsListModel.ApprovedByHRUserId = ApprovedByHrUserName == null ? "" : ApprovedByHrUserName.UserName;
                        leaveDetailsListModel.StartDate = item.StartDate.ToString("dd-MM-yyyy");
                        leaveDetailsListModel.EndDate = item.EndDate.ToString("dd-MM-yyyy");
                        leaveDetailsListModel.StartTime = Convert.ToString(item.StartTime);
                        leaveDetailsListModel.EndTime = Convert.ToString(item.EndTime);
                        leaveDetailsListModel.OpeningBalance = Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)item.OpeningLeaveBalance));
                        leaveDetailsListModel.ClosingBalance = LeaveBalanceDetails == null ? 0 : Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceDetails.ClosingLeaveBalance));
                        leaveDetailsListModel.NoOfDays = item.NoOfDays == null ? 0 : Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)item.NoOfDays));
                        leaveDetailsListModel.PaidDays = Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)item.PaidDays));
                        leaveDetailsListModel.UnPaidDays = Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)item.UnPaidDays));
                        leaveDetailsListModel.Reason = item.Reason;
                        leaveDetailsListModel.LeaveStatus = LeaveStatusName == null ? "Pending" : LeaveStatusName.LeaveStatus;
                        leaveDetailsListModel.BalanceMonth = LeaveBalanceDetails == null ? " " : LeaveBalanceDetails.BalanceMonth;
                        leaveDetailsListModel.BalanceYear = LeaveBalanceDetails == null ? 0 : Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceDetails.BalanceYear));
                        leaveDetailsListModel.SickLeaveBalance = LeaveBalanceDetails == null ? 0 : Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceDetails.SickLeaveBalance));
                        leaveDetailsListModel.SickLeaveTaken = LeaveBalanceDetails == null ? 0 : Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceDetails.SickLeaveTaken));
                        leaveDetailsListModel.CasualLeaveBalance = LeaveBalanceDetails == null ? 0 : Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceDetails.CasualLeaveBalance));
                        leaveDetailsListModel.CasualLeaveTaken = LeaveBalanceDetails == null ? 0 : Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceDetails.CasualLeaveTaken));
                        leaveDetailsListModel.EarnedLeaveBalance = LeaveBalanceDetails == null ? 0 : Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceDetails.EarnedLeaveBalance));
                        leaveDetailsListModel.EarnedLeaveTaken = LeaveBalanceDetails == null ? 0 : Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceDetails.EarnedLeaveTaken));
                        leaveDetailsListModel.Details = LeaveBalanceDetails == null ? " " : LeaveBalanceDetails.Detail;
                        leaveDetailsListModel.LeaveTaken = LeaveBalanceDetails == null ? 0 : Convert.ToDecimal(_commonHelper.GetFormattedDecimal((decimal)LeaveBalanceDetails.LeaveTaken));
                        leaveDetailsListModel.HrStatus = HrStatus == null ? "pending" : HrStatus.LeaveStatus;
                        leaveDetailsListModel.ApprovedByReportingStatus = ApprovedByReportingStatus == null ? "pending" : ApprovedByReportingStatus.LeaveStatus;

                        if (leaveDetailsListModel.HrStatus.ToLower() == "approve" && leaveDetailsListModel.ApprovedByReportingStatus.ToLower() == "approve")
                        {
                            leaveDetailsListModel.EditDisable = true;

                        }
                        if (IsUserHR)
                        {
                            if (leaveDetailsListModel.ApprovedByReportingStatus.ToLower() == "approve")
                            {
                                leaveDetailsListModel.HREditDisable = false;

                            }
                            else
                            {
                                leaveDetailsListModel.HREditDisable = true;
                            }
                        }
                        if (item.AppliedByUserId == userId)
                        {
                            if (CancelLeaveStatus.LeaveStatus.ToLower() == "cancel")
                            {
                                if (_commonHelper.GetCurrentDateTime().Date <= item.EndDate)
                                {
                                    leaveDetailsListModel.CancelbtnDisable = true;
                                }
                            }
                            else
                            {
                                if (_commonHelper.GetCurrentDateTime().Date <= item.EndDate)
                                {
                                    leaveDetailsListModel.CancelbtnDisable = false;
                                }
                                else
                                {
                                    leaveDetailsListModel.CancelbtnDisable = true;
                                }
                            }
                        }
                        else
                        {
                            leaveDetailsListModel.CancelbtnDisable = true;
                        }
                        leavesListResModel.LeaveDetailsLists.Add(leaveDetailsListModel);
                    }
                }

                if (leavesListResModel != null)
                {
                    response.Data = leavesListResModel;
                    response.Status = true;
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
            CommonResponse response = new CommonResponse();
            try
            {
                var userId = _commonHelper.GetLoggedInUserId();
                LeaveAddEditResModel leaveAddEditResModel = new LeaveAddEditResModel();
                leaveAddEditResModel.LeaveDetails = new LeaveDetails();
                leaveAddEditResModel.LeaveTypeList = await _dbRepo.LeaveTypeLists().Where(x => x.IsCurrentYear == true).ToListAsync();
                bool LeaveStatusChangeView = false;
                var joiningDate = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == userId);

                var startTimeList = new List<KeyValueModel>
                {
                    new KeyValueModel { Id = 1, Name = "09:30 AM" },
                    new KeyValueModel { Id = 2, Name = "02:00 PM" }
                };
                leaveAddEditResModel.StartTimeList = startTimeList;

                CommonResponse departmentResponse = await new CommonController(_dbRepo, _dbContext, _commonHelper).GetDepartmentByUserId(userId);

                string departmentCode = string.Empty;
                bool IsUserHR = false;
                if (departmentResponse.Status)
                {
                    departmentCode = departmentResponse.Data.DepartmentCode;
                    IsUserHR = departmentCode == CommonEnums.DepartmentMst.Human_Resource.ToString();
                }

                if (Id > 0)
                {
                    var leaveDetails = await _dbRepo.LeaveLists().FirstOrDefaultAsync(x => x.Id == Id);
                    if (leaveDetails != null)
                    {
                        var appliedByUserDetails = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == leaveDetails.AppliedByUserId);
                        LeaveStatusChangeView = appliedByUserDetails != null && appliedByUserDetails.ReportingManager == userId;

                        leaveAddEditResModel.LeaveDetails.Id = leaveDetails.Id;
                        leaveAddEditResModel.LeaveDetails.LeaveTypeId = leaveDetails.LeaveTypeId;
                        leaveAddEditResModel.LeaveDetails.StartDate = leaveDetails.StartDate;
                        leaveAddEditResModel.LeaveDetails.EndDate = leaveDetails.EndDate;
                        leaveAddEditResModel.LeaveDetails.StartTime = leaveDetails.StartTime;
                        leaveAddEditResModel.LeaveDetails.EndTime = leaveDetails.EndTime;
                        leaveAddEditResModel.LeaveDetails.Reason = leaveDetails.Reason;
                        leaveAddEditResModel.LeaveDetails.LeaveStatusId = leaveDetails.LeaveStatusId;

                        if (IsUserHR)
                        {
                            leaveDetails.ApprovedByReportingStatus = leaveDetails.ApprovedByReportingStatus == null ? 0 : leaveDetails.ApprovedByReportingStatus;
                            var CheckApprovedByReportingStatus = await _dbRepo.LeaveStatusLists().FirstOrDefaultAsync(x => x.Id == leaveDetails.ApprovedByReportingStatus);
                            if (CheckApprovedByReportingStatus != null && CheckApprovedByReportingStatus.LeaveStatus.ToLower() == "approve")
                            {
                                var IsUserNonHR = await _dbRepo.AllUserMstList().AnyAsync(x => x.Id == leaveDetails.AppliedByUserId && x.DepartmentId != Convert.ToInt32(CommonEnums.DepartmentMst.Human_Resource));

                                if (appliedByUserDetails != null && appliedByUserDetails.ReportingManager == userId)
                                {
                                    LeaveStatusChangeView = true;
                                    leaveAddEditResModel.LeaveDetails.ApprovedByReportingStatus = leaveDetails.ApprovedByReportingStatus == null ? 0 : (int)leaveDetails.ApprovedByReportingStatus;
                                }
                                else
                                {
                                    LeaveStatusChangeView = false;
                                }
                                if (!IsUserNonHR)
                                {
                                    LeaveStatusChangeView = true;
                                    leaveAddEditResModel.LeaveDetails.HrStatus = leaveDetails.Hrstatus == null ? 0 : (int)leaveDetails.Hrstatus;
                                }
                            }
                            else
                            {
                                LeaveStatusChangeView = false;
                            }
                        }
                    }
                }
                else
                {
                    leaveAddEditResModel.LeaveDetails.StartDate = _commonHelper.GetCurrentDateTime();
                    leaveAddEditResModel.LeaveDetails.EndDate = _commonHelper.GetCurrentDateTime();
                }
                leaveAddEditResModel.LeaveDetails.JoiningDate = joiningDate == null ? _commonHelper.GetCurrentDateTime() : joiningDate.JoinDate;
                leaveAddEditResModel.LeaveStatusChangeView = LeaveStatusChangeView;
                leaveAddEditResModel.LeaveStatusList = await _dbRepo.LeaveStatusLists().Select(x => new KeyValueModel { Id = x.Id, Name = x.LeaveStatus }).ToListAsync();

                response.Status = true;
                response.Message = "Data found successfully!";
                response.Data = leaveAddEditResModel;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return View(response);
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

                    if (request.StartDate >= _commonHelper.GetCurrentDateTime().Date)
                    {
                        decimal noOfDay = 0;
                        var requestmonth = request.StartDate.ToString("MMMM");

                        LeaveMst leaveMst = new LeaveMst();
                        LeaveBalanceMst leaveBalanceMst = new LeaveBalanceMst();
                        int userId = _commonHelper.GetLoggedInUserId();

                        if (request.Id == 0) // Add Leave
                        {
                            #region Add Region
                            var userJoiningDate = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == userId);
                            if (userJoiningDate != null)
                            {
                                if (userJoiningDate.JoinDate < _commonHelper.GetCurrentDateTime().Date)
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
                                                leaveMst.LeaveStatusId = 3;
                                                leaveMst.IsActive = true;
                                                leaveMst.IsDelete = false;
                                                leaveMst.CreatedBy = userId;
                                                leaveMst.UpdatedBy = userId;
                                                leaveMst.CreatedDate = _commonHelper.GetCurrentDateTime();
                                                leaveMst.UpdatedDate = _commonHelper.GetCurrentDateTime();

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
                                                        if (LeaveType.LeaveTypeName.ToLower() == "sickleave" || LeaveType.LeaveTypeName.ToLower() == "casualleave" || LeaveType.LeaveTypeName.ToLower() == "earnedleave")
                                                        {
                                                            BalanceList.SickLeaveTaken = 0;
                                                            BalanceList.CasualLeaveTaken = 0;
                                                            BalanceList.EarnedLeaveTaken = 0;
                                                        }
                                                        BalanceList.LeaveTypeId = request.LeaveTypeId;
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
                                                    if (BalanceList.ClosingLeaveBalance > BalanceList.LeaveTaken)
                                                    {
                                                        leaveMst.PaidDays = 0;
                                                        leaveMst.UnPaidDays = noOfDay;
                                                    }
                                                    else
                                                    {
                                                        leaveMst.PaidDays = 0;
                                                        leaveMst.UnPaidDays = noOfDay;
                                                    }
                                                }
                                                leaveMst.OpeningLeaveBalance = BalanceList == null ? 0 : BalanceList.ClosingLeaveBalance;
                                                await _dbContext.LeaveMsts.AddAsync(leaveMst);
                                                await _dbContext.SaveChangesAsync();
                                                var sandwichleave = SandwichLeave(request);

                                                transactionScope.Complete();
                                                response.Status = true;
                                                response.Message = "Leave added successfully!";
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
                                else
                                {
                                    response.Message = "Your can applied leave after joining!";
                                }
                            }
                            #endregion
                        }
                        else // updated
                        {
                            #region Updated Region
                            noOfDay = GetNoOfDays(request.StartDate, request.EndDate, request.StartTime, request.EndTime);

                            bool IsUserManager = false, IsUserHR = false;

                            CommonResponse departmentResponse = await new CommonController(_dbRepo, _dbContext, _commonHelper).GetDepartmentByUserId(userId);

                            string departmentCode = string.Empty;
                            if (departmentResponse.Status)
                            {
                                departmentCode = departmentResponse.Data.DepartmentCode;
                                IsUserHR = departmentCode == CommonEnums.DepartmentMst.Human_Resource.ToString();
                            }

                            var leaveDetails = await _dbRepo.LeaveLists().FirstOrDefaultAsync(x => x.Id == request.Id);
                            var userDetailsMst = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == leaveDetails.AppliedByUserId);

                            if (userDetailsMst != null)
                            {
                                IsUserManager = userDetailsMst.ReportingManager == _commonHelper.GetLoggedInUserId();
                            }

                            if (userDetailsMst != null)
                            {
                                decimal isProbationPeriodDays = 0;
                                bool isProbationPeriod = false;

                                if (userDetailsMst.JoinDate < request.StartDate)
                                {
                                    isProbationPeriodDays = GetProbationPeriod(userDetailsMst.JoinDate, request.StartDate);

                                    if (isProbationPeriodDays > 0)
                                    {
                                        if (isProbationPeriodDays < Convert.ToDecimal(userDetailsMst.ProbationPeriod))
                                        {
                                            isProbationPeriod = false;
                                        }
                                        else if (isProbationPeriodDays >= Convert.ToDecimal(userDetailsMst.ProbationPeriod))
                                        {
                                            isProbationPeriod = true;
                                        }
                                    }
                                }
                                if (leaveDetails != null)
                                {
                                    #region Calculation of paid leave
                                    var BalanceList = await _dbRepo.LeaveBalanceLists().Where(x => x.UserId == leaveDetails.AppliedByUserId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                                    var LeaveType = await _dbRepo.LeaveTypeLists().Where(x => x.Id == request.LeaveTypeId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                                    if (BalanceList != null && LeaveType != null)
                                    {
                                        if (noOfDay >= 4)
                                        {
                                            if (LeaveType.LeaveTypeName.ToLower() == "sickleave")
                                            {
                                                if (BalanceList.SickLeaveBalance >= noOfDay)
                                                {
                                                    leaveDetails.PaidDays = noOfDay;
                                                    leaveDetails.UnPaidDays = decimal.Parse("0");
                                                }
                                                else
                                                {
                                                    leaveDetails.PaidDays = BalanceList.SickLeaveBalance;
                                                    leaveDetails.UnPaidDays = noOfDay - leaveDetails.PaidDays;
                                                }
                                            }
                                            else if (LeaveType.LeaveTypeName.ToLower() == "casualleave")
                                            {
                                                if (BalanceList.CasualLeaveBalance >= noOfDay)
                                                {
                                                    leaveDetails.PaidDays = noOfDay;
                                                    leaveDetails.UnPaidDays = decimal.Parse("0");
                                                }
                                                else
                                                {
                                                    leaveDetails.PaidDays = BalanceList.CasualLeaveBalance;
                                                    leaveDetails.UnPaidDays = noOfDay - leaveDetails.PaidDays;
                                                }
                                            }
                                            else if (LeaveType.LeaveTypeName.ToLower() == "earnedleave")
                                            {
                                                if (BalanceList.EarnedLeaveBalance >= noOfDay)
                                                {
                                                    leaveDetails.PaidDays = noOfDay;
                                                    leaveDetails.UnPaidDays = decimal.Parse("0");
                                                }
                                                else
                                                {
                                                    leaveDetails.PaidDays = BalanceList.EarnedLeaveBalance;
                                                    leaveDetails.UnPaidDays = noOfDay - leaveDetails.PaidDays;
                                                }
                                            }

                                        }
                                        else
                                        {
                                            if (LeaveType.LeaveTypeName.ToLower() == "sickleave")
                                            {
                                                if (BalanceList.SickLeaveBalance >= noOfDay)
                                                {
                                                    leaveDetails.PaidDays = noOfDay;
                                                    leaveDetails.UnPaidDays = decimal.Parse("0");
                                                }
                                                else
                                                {
                                                    leaveDetails.PaidDays = BalanceList.SickLeaveBalance;
                                                    leaveDetails.UnPaidDays = noOfDay - leaveDetails.PaidDays;
                                                }
                                            }
                                            else if (LeaveType.LeaveTypeName.ToLower() == "casualleave")
                                            {
                                                if (BalanceList.CasualLeaveBalance >= noOfDay)
                                                {
                                                    leaveDetails.PaidDays = noOfDay;
                                                    leaveDetails.UnPaidDays = decimal.Parse("0");
                                                }
                                                else
                                                {
                                                    leaveDetails.PaidDays = BalanceList.CasualLeaveBalance;
                                                    leaveDetails.UnPaidDays = noOfDay - leaveDetails.PaidDays;
                                                }
                                            }
                                            else if (LeaveType.LeaveTypeName.ToLower() == "earnedleave")
                                            {
                                                if (BalanceList.EarnedLeaveBalance >= noOfDay)
                                                {
                                                    leaveDetails.PaidDays = noOfDay;
                                                    leaveDetails.UnPaidDays = decimal.Parse("0");
                                                }
                                                else
                                                {
                                                    leaveDetails.PaidDays = BalanceList.EarnedLeaveBalance;
                                                    leaveDetails.UnPaidDays = noOfDay - leaveDetails.PaidDays;

                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        response.Message = "Data Not Found!";
                                    }
                                    if (isProbationPeriod == false)
                                    {
                                        leaveDetails.PaidDays = 0;
                                        leaveDetails.UnPaidDays = noOfDay;
                                    }
                                    #endregion
                                    #region edit LeaveMst 
                                    leaveDetails.LeaveTypeId = request.LeaveTypeId;
                                    leaveDetails.StartDate = request.StartDate;
                                    leaveDetails.EndDate = request.EndDate;
                                    leaveDetails.StartTime = Convert.ToDateTime(request.StartTime).TimeOfDay;
                                    leaveDetails.EndTime = Convert.ToDateTime(request.EndTime).TimeOfDay;
                                    leaveDetails.Reason = request.Reason;
                                    leaveDetails.NoOfDays = noOfDay;
                                    leaveDetails.LeaveStatusId = 3;
                                    leaveDetails.UpdatedBy = userId;
                                    leaveDetails.UpdatedDate = _commonHelper.GetCurrentDateTime();

                                    if (IsUserManager)
                                    {
                                        leaveDetails.ApprovedByReportingStatus = request.LeaveStatusId;
                                        leaveDetails.ApprovedByReportingUserId = userId;
                                        if (IsUserHR)
                                        {
                                            leaveDetails.Hrstatus = 1;
                                            leaveDetails.ApprovedByHruserId = userId;
                                        }

                                    }

                                    else if (IsUserHR)
                                    {
                                        var LeaveStatusApproved1 = await _dbRepo.LeaveStatusLists().FirstOrDefaultAsync(x => x.Id == leaveDetails.LeaveStatusId);
                                        var ProjectManagerStatusApproved1 = await _dbRepo.LeaveStatusLists().FirstOrDefaultAsync(x => x.Id == leaveDetails.ApprovedByReportingStatus);
                                        if (ProjectManagerStatusApproved1.LeaveStatus.ToLower() == "approve")
                                        {
                                            leaveDetails.Hrstatus = request.LeaveStatusId;
                                            leaveDetails.ApprovedByHruserId = userId;
                                        }
                                    }

                                    #endregion
                                    _dbContext.Entry(leaveDetails).State = EntityState.Modified;
                                    await _dbContext.SaveChangesAsync();

                                    #region Calculation of deduction leave as approve 
                                    var LeaveStatusApproved = await _dbRepo.LeaveStatusLists().FirstOrDefaultAsync(x => x.Id == leaveDetails.LeaveStatusId);
                                    var HrStatusStatusApproved = await _dbRepo.LeaveStatusLists().FirstOrDefaultAsync(x => x.Id == leaveDetails.Hrstatus);
                                    var ProjectManagerStatusApproved = await _dbRepo.LeaveStatusLists().FirstOrDefaultAsync(x => x.Id == leaveDetails.ApprovedByReportingStatus);

                                    if (LeaveStatusApproved != null && HrStatusStatusApproved != null && ProjectManagerStatusApproved != null)
                                    {
                                        if (HrStatusStatusApproved.LeaveStatus.ToLower() == "approve" && ProjectManagerStatusApproved.LeaveStatus.ToLower() == "approve")
                                        {
                                            var leaveBalanceLastList1 = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == leaveDetails.AppliedByUserId).OrderByDescending(x => x.Id).FirstOrDefault();

                                            LeaveBalanceMst tbl = new LeaveBalanceMst();
                                            tbl.UserId = leaveBalanceLastList1.UserId;
                                            tbl.LeaveTypeId = leaveBalanceLastList1.LeaveTypeId;
                                            tbl.BalanceMonth = leaveBalanceLastList1.BalanceMonth;
                                            tbl.BalanceYear = leaveBalanceLastList1.BalanceYear;
                                            tbl.NoOfDays = leaveBalanceLastList1.NoOfDays;
                                            tbl.OpeningLeaveBalance = leaveBalanceLastList1 == null ? 0 : leaveBalanceLastList1.OpeningLeaveBalance;
                                            tbl.SickLeaveBalance = leaveBalanceLastList1 == null ? 0 : leaveBalanceLastList1.SickLeaveBalance;
                                            tbl.CasualLeaveBalance = leaveBalanceLastList1 == null ? 0 : leaveBalanceLastList1.CasualLeaveBalance;
                                            tbl.EarnedLeaveBalance = leaveBalanceLastList1 == null ? 0 : leaveBalanceLastList1.EarnedLeaveBalance;
                                            tbl.SickLeaveTaken = leaveBalanceLastList1.SickLeaveTaken;
                                            tbl.CasualLeaveTaken = leaveBalanceLastList1.CasualLeaveTaken;
                                            tbl.EarnedLeaveTaken = leaveBalanceLastList1.EarnedLeaveTaken;
                                            tbl.ClosingLeaveBalance = leaveBalanceLastList1 == null ? 0 : leaveBalanceLastList1.ClosingLeaveBalance;
                                            tbl.LeaveTaken = requestmonth == leaveBalanceLastList1.BalanceMonth ? noOfDay : 0;
                                            tbl.BalanceDate = leaveBalanceLastList1.BalanceDate;
                                            tbl.Detail = leaveBalanceLastList1.Detail;
                                            tbl.IsActive = leaveBalanceLastList1.IsActive;
                                            tbl.IsDelete = leaveBalanceLastList1.IsDelete;
                                            tbl.CreatedBy = leaveBalanceLastList1.CreatedBy;
                                            tbl.UpdatedBy = leaveBalanceLastList1.UpdatedBy;
                                            tbl.CreatedDate = leaveBalanceLastList1.CreatedDate;
                                            tbl.UpdatedDate = leaveBalanceLastList1.UpdatedDate;

                                            await _dbContext.LeaveBalanceMsts.AddAsync(tbl);
                                            _dbContext.SaveChanges();

                                            var leaveBalanceList1 = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == leaveDetails.AppliedByUserId).OrderByDescending(x => x.Id).FirstOrDefault();
                                            if (leaveBalanceList1 != null)
                                            {
                                                if (isProbationPeriod)
                                                {
                                                    var leaveType1 = await _dbRepo.LeaveTypeLists().Where(x => x.Id == request.LeaveTypeId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                                                    leaveBalanceList1.LeaveTypeId = request.LeaveTypeId;
                                                    leaveBalanceList1.BalanceMonth = leaveBalanceList1.BalanceMonth;
                                                    leaveBalanceList1.BalanceYear = leaveBalanceList1.BalanceYear;
                                                    leaveBalanceList1.NoOfDays = noOfDay;
                                                    if (leaveType1.LeaveTypeName.ToLower() == "sickleave")
                                                    {
                                                        leaveBalanceList1.SickLeaveTaken = 0.00M;
                                                        leaveBalanceList1.SickLeaveBalance = leaveBalanceList1.SickLeaveBalance == 0 ? 0 : leaveBalanceList1.SickLeaveBalance - noOfDay;
                                                        if (leaveBalanceList1.SickLeaveBalance < 0)
                                                        {
                                                            leaveBalanceList1.SickLeaveBalance = 0.00M;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        leaveBalanceList1.SickLeaveBalance = leaveBalanceList1 == null ? 0 : leaveBalanceList1.SickLeaveBalance;
                                                        leaveBalanceList1.SickLeaveTaken = leaveBalanceList1.SickLeaveTaken;
                                                    }

                                                    if (leaveType1.LeaveTypeName.ToLower() == "casualleave")
                                                    {
                                                        leaveBalanceList1.CasualLeaveTaken = 0.00M;
                                                        leaveBalanceList1.CasualLeaveBalance = leaveBalanceList1.CasualLeaveBalance == 0 ? leaveBalanceList1.CasualLeaveBalance : leaveBalanceList1.CasualLeaveBalance - noOfDay;
                                                        if (leaveBalanceList1.CasualLeaveBalance < 0)
                                                        {
                                                            leaveBalanceList1.CasualLeaveBalance = 0.00M;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        leaveBalanceList1.CasualLeaveBalance = leaveBalanceList1 == null ? 0 : leaveBalanceList1.CasualLeaveBalance;
                                                        leaveBalanceList1.CasualLeaveTaken = leaveBalanceList1.CasualLeaveTaken;
                                                    }
                                                    if (leaveType1.LeaveTypeName.ToLower() == "earnedleave")
                                                    {
                                                        leaveBalanceList1.EarnedLeaveTaken = 0.00M;
                                                        leaveBalanceList1.EarnedLeaveBalance = leaveBalanceList1.EarnedLeaveBalance == 0 ? leaveBalanceList1.EarnedLeaveBalance : leaveBalanceList1.EarnedLeaveBalance - noOfDay;
                                                        if (leaveBalanceList1.EarnedLeaveBalance < 0)
                                                        {
                                                            leaveBalanceList1.EarnedLeaveBalance = 0.00M;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        leaveBalanceList1.EarnedLeaveBalance = leaveBalanceList1 == null ? 0 : leaveBalanceList1.EarnedLeaveBalance;
                                                        leaveBalanceList1.EarnedLeaveTaken = leaveBalanceList1.EarnedLeaveTaken;
                                                    }
                                                    leaveBalanceList1.ClosingLeaveBalance = leaveBalanceList1 == null ? 0 : leaveBalanceList1.EarnedLeaveBalance + leaveBalanceList1.SickLeaveBalance + leaveBalanceList1.CasualLeaveBalance;
                                                    leaveBalanceList1.OpeningLeaveBalance = leaveBalanceList1 == null ? 0 : leaveBalanceList1.OpeningLeaveBalance;
                                                    leaveBalanceList1.LeaveTaken = requestmonth == leaveBalanceList1.BalanceMonth ? noOfDay : 0;
                                                    leaveBalanceList1.BalanceDate = _commonHelper.GetCurrentDateTime();
                                                    leaveBalanceList1.Detail = "Monthly Leave Balance";
                                                    leaveBalanceList1.IsActive = true;
                                                    leaveBalanceList1.IsDelete = false;
                                                    leaveBalanceList1.CreatedBy = userId;
                                                    leaveBalanceList1.UpdatedBy = userId;
                                                    leaveBalanceList1.CreatedDate = _commonHelper.GetCurrentDateTime();
                                                    leaveBalanceList1.UpdatedDate = _commonHelper.GetCurrentDateTime();

                                                    if (IsUserHR)
                                                    {
                                                        leaveDetails.Hrstatus = request.LeaveStatusId;
                                                        leaveDetails.ApprovedByHruserId = userId;
                                                    }
                                                    else if (IsUserManager)
                                                    {
                                                        leaveDetails.ApprovedByReportingStatus = request.LeaveStatusId;
                                                        leaveDetails.ApprovedByReportingUserId = userId;
                                                    }
                                                    if (LeaveType.LeaveTypeName.ToLower() == "sickleave")
                                                    {
                                                        leaveBalanceList1.SickLeaveTaken = noOfDay;
                                                        leaveBalanceList1.CasualLeaveTaken = 0;
                                                        leaveBalanceList1.EarnedLeaveTaken = 0;
                                                    }
                                                    else if (LeaveType.LeaveTypeName.ToLower() == "casualleave")
                                                    {
                                                        leaveBalanceList1.SickLeaveTaken = 0;
                                                        leaveBalanceList1.CasualLeaveTaken = noOfDay;
                                                        leaveBalanceList1.EarnedLeaveTaken = 0;
                                                    }
                                                    else if (LeaveType.LeaveTypeName.ToLower() == "earnedleave")
                                                    {
                                                        leaveBalanceList1.SickLeaveTaken = 0;
                                                        leaveBalanceList1.CasualLeaveTaken = 0;
                                                        leaveBalanceList1.EarnedLeaveTaken = noOfDay;
                                                    }

                                                    _dbContext.Entry(leaveBalanceList1).State = EntityState.Modified;
                                                    await _dbContext.SaveChangesAsync();

                                                    leaveDetails.OpeningLeaveBalance = leaveBalanceList1.OpeningLeaveBalance;
                                                    leaveDetails.LeaveStatusId = 1;

                                                    _dbContext.Entry(leaveDetails).State = EntityState.Modified;
                                                    await _dbContext.SaveChangesAsync();
                                                }
                                            }
                                            else
                                            {
                                                response.Message = "Data not found!";

                                            }
                                        }
                                        else
                                        {
                                            response.Message = "Your leave is not approve";
                                        }
                                    }
                                    #endregion
                                }
                                response.Status = true;

                                response.Message = "Leave updated successfully!";
                            }
                            else
                            {
                                response.Message = "Leave not found!";
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        response.Message = "Past date is not allowed!";
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

        [HttpPost]
        public async Task<CommonResponse> UpdateCancelLeave([FromBody] UpdateCancleLeaveReqModel request)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var userId = _commonHelper.GetLoggedInUserId();
                #region calculation of cancel leave  in Leave-balance  
                var leaveStatus = await _dbRepo.LeaveStatusLists().ToListAsync();
                var leavebalance = await _dbRepo.LeaveBalanceLists().Where(x => x.UserId == userId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                if (leavebalance != null)
                {
                    var leavemst = await _dbRepo.LeaveLists().Where(x => x.Id == request.Id).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                    if (leavemst.AppliedByUserId == userId)
                    {
                        var leaveApprovedByReportingStatus = leaveStatus.Where(x => x.Id == leavemst.ApprovedByReportingStatus).OrderByDescending(x => x.Id).FirstOrDefault();

                        var leaveApprovedByhrStatus = leaveStatus.Where(x => x.Id == leavemst.Hrstatus).OrderByDescending(x => x.Id).FirstOrDefault();

                        if (leaveApprovedByhrStatus != null && leaveApprovedByReportingStatus != null && leaveApprovedByReportingStatus.LeaveStatus.ToLower() == "approve" && leaveApprovedByhrStatus.LeaveStatus.ToLower() == "approve")
                        {
                            leavebalance.IsActive = false;
                            leavebalance.IsDelete = true;

                            leavemst.Reason = request.Reason;
                            leavemst.LeaveStatusId = 1002;
                            leavemst.IsActive = false;
                            leavemst.IsDelete = true;

                            _dbContext.Entry(leavebalance).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();

                            _dbContext.Entry(leavemst).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            leavemst.LeaveStatusId = 1002;
                            leavemst.IsActive = false;
                            leavemst.IsDelete = true;
                            _dbContext.Entry(leavemst).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();

                        }
                    }
                }
                #endregion
                response.Status = true;
                response.Message = "Data updated successfully!";
            }
            catch (Exception e)
            {
                response.Message = e.Message;
            }
            return response;
        }

        [HttpGet]
        public async Task<IActionResult> ShowLeavesDetails()
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var userId = _commonHelper.GetLoggedInUserId();
                var leaveDetails = await (from leaveBalance in _dbRepo.LeaveBalanceLists()
                                          where leaveBalance.UserId == userId
                                          select new
                                          {
                                              leaveBalance.Id,
                                              leaveBalance.OpeningLeaveBalance,
                                              leaveBalance.ClosingLeaveBalance,
                                              leaveBalance.SickLeaveBalance,
                                              leaveBalance.SickLeaveTaken,
                                              leaveBalance.CasualLeaveBalance,
                                              leaveBalance.CasualLeaveTaken,
                                              leaveBalance.EarnedLeaveBalance,
                                              leaveBalance.EarnedLeaveTaken,
                                              leaveBalance.BalanceMonth,
                                              leaveBalance.BalanceYear
                                          }).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                if (leaveDetails != null)
                {
                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Data found successfully!";
                    response.Data = leaveDetails;
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

        #region Private Functions

        private decimal GetNoOfDays(DateTime StartDate, DateTime EndDate, string StartTime, string EndTime)
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

        private decimal GetProbationPeriod(DateTime StartDate, DateTime EndDate)
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

        private async Task<CommonResponse> GetPerMonthBalanceAsync()
        {
            CommonResponse response = new CommonResponse();
            CommonResponse commonResponse = new CommonResponse();
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var loginUserId = _commonHelper.GetLoggedInUserId();
                    var userJoiningDate = _dbRepo.UserDetailList().FirstOrDefault(x => x.UserId == loginUserId);
                    if (userJoiningDate != null)
                    {
                        int pastYear = _commonHelper.GetCurrentDateTime().AddYears(-1).Year;
                        DateTime dt2 = new DateTime(_commonHelper.GetCurrentDateTime().Year, 01, 01);
                        DateTime perviousdt = new DateTime(pastYear, 12, 31);
                        // int pastmonth = userJoiningDate.JoinDate.AddMonths(15).Month;
                        // DateTime nextMonthDate = new DateTime(_commonHelper.GetCurrentDateTime().Year, pastmonth, 1);

                        DateTime dtNow = _commonHelper.GetCurrentDateTime();
                        DateTime userJoiningDatemodify = new DateTime(userJoiningDate.JoinDate.Year, userJoiningDate.JoinDate.Month, dtNow.Day);
                        var LeaveBalanceList = _dbRepo.LeaveBalanceLists().ToList();

                        for (DateTime i = userJoiningDatemodify; i <= dtNow.Date; i = i.Date.AddMonths(1))
                        {
                            var BalanceMonth = i.Date.ToString("MMMM");
                            if (i.Date > perviousdt)
                            {
                                var LeaveBalanceDetail = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == loginUserId).OrderByDescending(x => x.Id).FirstOrDefault();
                                if (LeaveBalanceDetail != null)
                                {
                                    var LeaveBalance1 = await _dbRepo.LeaveBalanceLists().Where(x => x.UserId == loginUserId && x.BalanceMonth == BalanceMonth && x.BalanceYear == i.Date.Year).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                                    if (LeaveBalance1 == null && BalanceMonth.ToLower() == "january")
                                    {
                                        LeaveBalanceMst tbl = new LeaveBalanceMst();
                                        tbl.UserId = loginUserId;
                                        tbl.LeaveTypeId = 0;
                                        tbl.BalanceMonth = BalanceMonth;
                                        tbl.BalanceYear = i.Date.Year;
                                        tbl.OpeningLeaveBalance = 0;
                                        tbl.SickLeaveBalance = 0.5m;
                                        tbl.CasualLeaveBalance = 0.5m;
                                        tbl.EarnedLeaveBalance = 0.5m + LeaveBalanceDetail.EarnedLeaveBalance;
                                        tbl.SickLeaveTaken = 0;
                                        tbl.CasualLeaveTaken = 0;
                                        tbl.EarnedLeaveTaken = 0;
                                        tbl.ClosingLeaveBalance = tbl.EarnedLeaveBalance + tbl.SickLeaveBalance + tbl.CasualLeaveBalance;
                                        tbl.LeaveTaken = 0;
                                        tbl.BalanceDate = _commonHelper.GetCurrentDateTime();
                                        tbl.Detail = "Monthly Leave Balance";
                                        tbl.IsActive = true;
                                        tbl.IsDelete = false;
                                        tbl.CreatedBy = loginUserId;
                                        tbl.UpdatedBy = loginUserId;
                                        tbl.CreatedDate = dtNow;
                                        tbl.UpdatedDate = dtNow;

                                        await _dbContext.LeaveBalanceMsts.AddAsync(tbl);
                                        await _dbContext.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        var LeaveBalance4 = await _dbRepo.LeaveBalanceLists().Where(x => x.UserId == loginUserId && x.BalanceMonth == BalanceMonth && x.BalanceYear == i.Date.Year).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                                        if (LeaveBalance4 == null)
                                        {
                                            LeaveBalanceMst tbl = new LeaveBalanceMst();
                                            tbl.UserId = loginUserId;
                                            tbl.LeaveTypeId = 0;
                                            tbl.BalanceMonth = i.Date.ToString("MMMM");
                                            tbl.BalanceYear = i.Year;
                                            tbl.NoOfDays = 0;
                                            tbl.OpeningLeaveBalance = LeaveBalanceDetail == null ? 0 : LeaveBalanceDetail.ClosingLeaveBalance;
                                            tbl.SickLeaveBalance = LeaveBalanceDetail == null ? 0 : LeaveBalanceDetail.SickLeaveBalance + decimal.Parse("0.5");
                                            tbl.CasualLeaveBalance = LeaveBalanceDetail == null ? 0 : LeaveBalanceDetail.CasualLeaveBalance + decimal.Parse("0.5");
                                            tbl.EarnedLeaveBalance = LeaveBalanceDetail == null ? 0 : LeaveBalanceDetail.EarnedLeaveBalance + decimal.Parse("0.5");
                                            tbl.SickLeaveTaken = 0;
                                            tbl.CasualLeaveTaken = 0;
                                            tbl.EarnedLeaveTaken = 0;
                                            tbl.ClosingLeaveBalance = LeaveBalanceDetail == null ? 0 : tbl.SickLeaveBalance + tbl.CasualLeaveBalance + tbl.EarnedLeaveBalance;
                                            tbl.LeaveTaken = 0;
                                            tbl.BalanceDate = _commonHelper.GetCurrentDateTime();
                                            tbl.Detail = "Monthly Leave Balance";
                                            tbl.IsActive = true;
                                            tbl.IsDelete = false;
                                            tbl.CreatedBy = loginUserId;
                                            tbl.UpdatedBy = loginUserId;
                                            tbl.CreatedDate = _commonHelper.GetCurrentDateTime();
                                            tbl.UpdatedDate = _commonHelper.GetCurrentDateTime();

                                            await _dbContext.LeaveBalanceMsts.AddAsync(tbl);
                                            await _dbContext.SaveChangesAsync();
                                        }
                                    }
                                }

                                var LeaveBalance = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == loginUserId && x.BalanceMonth == BalanceMonth).OrderByDescending(x => x.Id).FirstOrDefault();

                                if (LeaveBalance == null)
                                {
                                    var LeaveBalancedetails = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == loginUserId).OrderByDescending(x => x.Id).FirstOrDefault();
                                    if (LeaveBalancedetails == null)
                                    {
                                        LeaveBalanceMst tbl = new LeaveBalanceMst();
                                        tbl.UserId = loginUserId;
                                        tbl.LeaveTypeId = 0;
                                        tbl.BalanceMonth = BalanceMonth;
                                        tbl.BalanceYear = i.Date.Year;
                                        tbl.OpeningLeaveBalance = 0;
                                        tbl.SickLeaveBalance = 0.5m;
                                        tbl.CasualLeaveBalance = 0.5m;
                                        tbl.EarnedLeaveBalance = LeaveBalancedetails == null ? 0.5m : 0.5m + LeaveBalancedetails.EarnedLeaveBalance;
                                        tbl.SickLeaveTaken = 0;
                                        tbl.CasualLeaveTaken = 0;
                                        tbl.EarnedLeaveTaken = 0;
                                        tbl.ClosingLeaveBalance = tbl.EarnedLeaveBalance + tbl.SickLeaveBalance + tbl.CasualLeaveBalance;
                                        tbl.LeaveTaken = 0;
                                        tbl.BalanceDate = _commonHelper.GetCurrentDateTime();
                                        tbl.Detail = "Monthly Leave Balance";
                                        tbl.IsActive = true;
                                        tbl.IsDelete = false;
                                        tbl.CreatedBy = loginUserId;
                                        tbl.UpdatedBy = loginUserId;
                                        tbl.CreatedDate = dtNow;
                                        tbl.UpdatedDate = dtNow;

                                        _dbContext.LeaveBalanceMsts.Add(tbl);
                                        _dbContext.SaveChanges();
                                    }

                                }
                                else
                                {
                                    var LeaveBalanceDetails = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == loginUserId).OrderByDescending(x => x.Id).FirstOrDefault();
                                    if (LeaveBalanceDetails != null)
                                    {
                                        var LeaveBalance1 = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == loginUserId && x.BalanceMonth == BalanceMonth).OrderByDescending(x => x.Id).FirstOrDefault();

                                        if (LeaveBalance1 == null)
                                        {
                                            LeaveBalanceMst tbl = new LeaveBalanceMst();
                                            tbl.UserId = loginUserId;
                                            tbl.LeaveTypeId = 0;
                                            tbl.BalanceMonth = i.Date.ToString("MMMM");
                                            tbl.BalanceYear = i.Year;
                                            tbl.NoOfDays = 0;
                                            tbl.OpeningLeaveBalance = LeaveBalanceDetails == null ? 0 : LeaveBalanceDetails.ClosingLeaveBalance;
                                            tbl.SickLeaveBalance = LeaveBalanceDetails == null ? 0 : LeaveBalanceDetails.SickLeaveBalance + decimal.Parse("0.5");
                                            tbl.CasualLeaveBalance = LeaveBalanceDetails == null ? 0 : LeaveBalanceDetails.CasualLeaveBalance + decimal.Parse("0.5");
                                            tbl.EarnedLeaveBalance = LeaveBalanceDetails == null ? 0 : LeaveBalanceDetails.EarnedLeaveBalance + decimal.Parse("0.5");
                                            tbl.SickLeaveTaken = 0;
                                            tbl.CasualLeaveTaken = 0;
                                            tbl.EarnedLeaveTaken = 0;
                                            tbl.ClosingLeaveBalance = LeaveBalanceDetails == null ? 0 : tbl.SickLeaveBalance + tbl.CasualLeaveBalance + tbl.EarnedLeaveBalance;
                                            tbl.LeaveTaken = 0;
                                            tbl.BalanceDate = _commonHelper.GetCurrentDateTime();
                                            tbl.Detail = "Monthly Leave Balance";
                                            tbl.IsActive = true;
                                            tbl.IsDelete = false;
                                            tbl.CreatedBy = loginUserId;
                                            tbl.UpdatedBy = loginUserId;
                                            tbl.CreatedDate = _commonHelper.GetCurrentDateTime();
                                            tbl.UpdatedDate = _commonHelper.GetCurrentDateTime();

                                            _dbContext.LeaveBalanceMsts.Add(tbl);
                                            _dbContext.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var LeaveBalanceList1 = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == loginUserId).OrderBy(x => x.Id).LastOrDefault();
                                if (LeaveBalanceList1 == null)
                                {
                                    LeaveBalanceMst tbl1 = new LeaveBalanceMst();
                                    tbl1.UserId = loginUserId;
                                    tbl1.LeaveTypeId = 0;
                                    tbl1.BalanceMonth = BalanceMonth;
                                    tbl1.BalanceYear = i.Date.Year;
                                    tbl1.OpeningLeaveBalance = 0;
                                    tbl1.SickLeaveBalance = 0.5m;
                                    tbl1.CasualLeaveBalance = 0.5m;
                                    tbl1.EarnedLeaveBalance = 0.5m;
                                    tbl1.SickLeaveTaken = 0;
                                    tbl1.CasualLeaveTaken = 0;
                                    tbl1.EarnedLeaveTaken = 0;
                                    tbl1.ClosingLeaveBalance = tbl1.EarnedLeaveBalance + tbl1.SickLeaveBalance + tbl1.CasualLeaveBalance;
                                    tbl1.LeaveTaken = 0;
                                    tbl1.BalanceDate = _commonHelper.GetCurrentDateTime();
                                    tbl1.Detail = "Monthly Leave Balance";
                                    tbl1.IsActive = true;
                                    tbl1.IsDelete = false;
                                    tbl1.CreatedBy = loginUserId;
                                    tbl1.UpdatedBy = loginUserId;
                                    tbl1.CreatedDate = dtNow;
                                    tbl1.UpdatedDate = dtNow;

                                    _dbContext.LeaveBalanceMsts.Add(tbl1);
                                    _dbContext.SaveChanges();


                                }
                                else
                                {
                                    var LeaveBalanceDetails = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == loginUserId).OrderByDescending(x => x.Id).FirstOrDefault();
                                    if (LeaveBalanceDetails != null)
                                    {
                                        var LeaveBalance = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == loginUserId && x.BalanceMonth == BalanceMonth).OrderByDescending(x => x.Id).FirstOrDefault();
                                        if (LeaveBalance == null)
                                        {
                                            LeaveBalanceMst tbl = new LeaveBalanceMst();
                                            tbl.UserId = loginUserId;
                                            tbl.LeaveTypeId = 0;
                                            tbl.BalanceMonth = i.Date.ToString("MMMM");
                                            tbl.BalanceYear = i.Year;
                                            tbl.NoOfDays = 0;
                                            tbl.OpeningLeaveBalance = LeaveBalanceDetails == null ? 0 : LeaveBalanceDetails.ClosingLeaveBalance;
                                            tbl.SickLeaveBalance = LeaveBalanceDetails == null ? 0 : LeaveBalanceDetails.SickLeaveBalance + decimal.Parse("0.5");
                                            tbl.CasualLeaveBalance = LeaveBalanceDetails == null ? 0 : LeaveBalanceDetails.CasualLeaveBalance + decimal.Parse("0.5");
                                            tbl.EarnedLeaveBalance = LeaveBalanceDetails == null ? 0 : LeaveBalanceDetails.EarnedLeaveBalance + decimal.Parse("0.5");
                                            tbl.SickLeaveTaken = 0;
                                            tbl.CasualLeaveTaken = 0;
                                            tbl.EarnedLeaveTaken = 0;
                                            tbl.ClosingLeaveBalance = LeaveBalanceDetails == null ? 0 : tbl.SickLeaveBalance + tbl.CasualLeaveBalance + tbl.EarnedLeaveBalance;
                                            tbl.LeaveTaken = 0;
                                            tbl.BalanceDate = _commonHelper.GetCurrentDateTime();
                                            tbl.Detail = "Monthly Leave Balance";
                                            tbl.IsActive = true;
                                            tbl.IsDelete = false;
                                            tbl.CreatedBy = loginUserId;
                                            tbl.UpdatedBy = loginUserId;
                                            tbl.CreatedDate = _commonHelper.GetCurrentDateTime();
                                            tbl.UpdatedDate = _commonHelper.GetCurrentDateTime();

                                            _dbContext.LeaveBalanceMsts.Add(tbl);
                                            _dbContext.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        transactionScope.Complete();
                    }
                    else
                    {

                    }
                }
                catch (Exception e)
                {
                    response.Message = e.Message;
                }
            }
            return response;
        }
        #endregion
    }
}
