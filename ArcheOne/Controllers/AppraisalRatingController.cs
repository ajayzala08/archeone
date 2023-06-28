using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ArcheOne.Controllers
{
    public class AppraisalRatingController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly CommonHelper _commonHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ArcheOneDbContext _dbContext;
        public AppraisalRatingController(DbRepo dbRepo, CommonHelper commonHelper, IWebHostEnvironment webHostEnvironment, ArcheOneDbContext dbContext)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
        }


        public IActionResult AddEditAppraisalRating(int Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            AddEditAppraisalRatingResModel addEditAppraisalRatingResModel = new AddEditAppraisalRatingResModel();
            addEditAppraisalRatingResModel.reportingManagetDetail = new ManagetDetail();
            addEditAppraisalRatingResModel.reportingManagetDetail.EmployeeDetail = new EmployeesDetail();
            addEditAppraisalRatingResModel.appraisalRating = new AppraisalRating();
            addEditAppraisalRatingResModel.EmployeeRating = new EmployeeRating();

            var managerRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("Manager")).ToList();
            var managerroleIdList = managerRoleList.Select(x => x.Id).ToList();

            var hrRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("HR")).ToList();
            var hrroleIdList = hrRoleList.Select(x => x.Id).ToList();

            var employeeRoleList = _dbRepo.RoleMstList().Where(x => !x.RoleCode.Contains("HR") && !x.RoleCode.Contains("Manager")).ToList();
            var employeeroleIdList = employeeRoleList.Select(x => x.Id).ToList();

            var userList = _dbRepo.AllUserMstList().Where(x => x.RoleId != null);
            var loginUserList = _dbRepo.AllUserMstList().Where(x => x.RoleId != null && x.Id == _commonHelper.GetLoggedInUserId());

            var appraisalList = _dbRepo.AppraisalList().ToList();

            var reportingManagerList = userList.Where(x => managerroleIdList.Contains(x.RoleId.Value)).ToList();
            var employeeList = userList.Where(x => !managerroleIdList.Contains(x.RoleId.Value)).ToList();

            var IsUserManager = loginUserList.Where(x => managerroleIdList.Contains(x.RoleId.Value)).ToList();
            var IsUserHR = loginUserList.Where(x => hrroleIdList.Contains(x.RoleId.Value)).ToList();
            var IsUserEmployee = loginUserList.Where(x => employeeroleIdList.Contains(x.RoleId.Value)).ToList();


            addEditAppraisalRatingResModel.EmployeeId = employeeList;
            addEditAppraisalRatingResModel.ReportingManagerId = reportingManagerList;

            addEditAppraisalRatingResModel.IsUserHR = IsUserHR.Count > 0 ? true : false;
            addEditAppraisalRatingResModel.IsUserReportManager = IsUserManager.Count > 0 ? true : false;
            addEditAppraisalRatingResModel.IsUserEmployee = IsUserEmployee.Count > 0 ? true : false;

            try
            {

                if (Id > 0)
                {
                    var appraisal = _dbRepo.AppraisalList().FirstOrDefault(x => x.Id == Id);
                    var appraisalManagerRating = _dbRepo.AppraisalRatingList().FirstOrDefault(x => x.RatingFromUserId == _commonHelper.GetLoggedInUserId() && x.AppraisalId == Id);
                    if (appraisal != null)
                    {
                        addEditAppraisalRatingResModel.reportingManagetDetail.ReportingManagerId = appraisal.ReportingManagerId;
                        addEditAppraisalRatingResModel.reportingManagetDetail.EmployeeDetail.EmployeeId = appraisal.EmployeeId;
                        addEditAppraisalRatingResModel.Id = appraisal.Id;
                        addEditAppraisalRatingResModel.Date = appraisal.CreatedDate.Date.ToString("dd-MM-yyyy");
                        addEditAppraisalRatingResModel.ReviewDate = appraisal.UpdatedDate.Date.ToString("dd-MM-yyyy");
                        if (addEditAppraisalRatingResModel.IsUserHR == true || addEditAppraisalRatingResModel.IsUserReportManager == true)
                        {
                            if (addEditAppraisalRatingResModel.IsUserHR == true)
                            {
                                appraisalManagerRating = _dbRepo.AppraisalRatingList().FirstOrDefault(x => x.AppraisalId == Id && x.RatingFromUserId == appraisal.ReportingManagerId);
                            }
                            if (appraisalManagerRating != null)
                            {
                                addEditAppraisalRatingResModel.appraisalRating.QualityOfWork = appraisalManagerRating.QualityOfWork;
                                addEditAppraisalRatingResModel.appraisalRating.GoalNtarget = appraisalManagerRating.GoalNtarget;
                                addEditAppraisalRatingResModel.appraisalRating.WrittenVerbalSkill = appraisalManagerRating.WrittenVerbalSkill;
                                addEditAppraisalRatingResModel.appraisalRating.InitiativeMotivation = appraisalManagerRating.InitiativeMotivation;
                                addEditAppraisalRatingResModel.appraisalRating.TeamWork = appraisalManagerRating.TeamWork;
                                addEditAppraisalRatingResModel.appraisalRating.ProblemSolvingAbillity = appraisalManagerRating.ProblemSolvingAbillity;
                                addEditAppraisalRatingResModel.appraisalRating.Attendance = appraisalManagerRating.Attendance;
                                addEditAppraisalRatingResModel.appraisalRating.Total = appraisalManagerRating.Total;
                                addEditAppraisalRatingResModel.appraisalRating.Comment = appraisalManagerRating.Comment;

                            }
                        }
                        if (addEditAppraisalRatingResModel.IsUserHR == true || addEditAppraisalRatingResModel.IsUserEmployee == true)
                        {
                            var appraisalEmployeeRating = _dbRepo.AppraisalRatingList().FirstOrDefault(x => x.RatingToUserId == _commonHelper.GetLoggedInUserId() || x.RatingToUserId == appraisal.EmployeeId);
                            if (addEditAppraisalRatingResModel.IsUserHR == true)
                            {
                                appraisalEmployeeRating = _dbRepo.AppraisalRatingList().FirstOrDefault(x => x.AppraisalId == Id && x.RatingFromUserId == appraisal.EmployeeId);
                            }
                            if (appraisalEmployeeRating != null)
                            {
                                addEditAppraisalRatingResModel.EmployeeRating.QualityOfWork = appraisalEmployeeRating.QualityOfWork;
                                addEditAppraisalRatingResModel.EmployeeRating.GoalNtarget = appraisalEmployeeRating.GoalNtarget;
                                addEditAppraisalRatingResModel.EmployeeRating.WrittenVerbalSkill = appraisalEmployeeRating.WrittenVerbalSkill;
                                addEditAppraisalRatingResModel.EmployeeRating.InitiativeMotivation = appraisalEmployeeRating.InitiativeMotivation;
                                addEditAppraisalRatingResModel.EmployeeRating.TeamWork = appraisalEmployeeRating.TeamWork;
                                addEditAppraisalRatingResModel.EmployeeRating.ProblemSolvingAbillity = appraisalEmployeeRating.ProblemSolvingAbillity;
                                addEditAppraisalRatingResModel.EmployeeRating.Attendance = appraisalEmployeeRating.Attendance;
                                addEditAppraisalRatingResModel.EmployeeRating.Total = appraisalEmployeeRating.Total;
                                addEditAppraisalRatingResModel.EmployeeRating.Comment = appraisalEmployeeRating.Comment;

                            }
                        }

                        commonResponse.Status = true;
                        commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
                        commonResponse.Message = "Get Appraisal Rating Successfully";
                        commonResponse.Data = addEditAppraisalRatingResModel;

                    }
                    else
                    {
                        commonResponse.Message = "Data Not Found";
                        commonResponse.StatusCode = System.Net.HttpStatusCode.NotFound;

                    }
                }
                else
                {
                    commonResponse.Message = "Data Not Found";
                    commonResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                }
                commonResponse.Data = addEditAppraisalRatingResModel;

            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
            }
            return View(commonResponse.Data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveUpdateAppraisalRating([FromBody] AppraisalRatingSaveUpdateReqModel appraisalRatingSaveUpdateReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                AppraisalRatingMst appraisalRatingMst = new AppraisalRatingMst();
                var appraisalRatingDetail = await _dbRepo.AppraisalRatingList().FirstOrDefaultAsync(x => x.RatingFromUserId == _commonHelper.GetLoggedInUserId() && x.AppraisalId == appraisalRatingSaveUpdateReqModel.Id);

                if (appraisalRatingDetail != null)
                {
                    //Edit Mode
                    appraisalRatingDetail.QualityOfWork = appraisalRatingSaveUpdateReqModel.QualityOfWork;
                    appraisalRatingDetail.GoalNtarget = appraisalRatingSaveUpdateReqModel.GoalNtarget;
                    appraisalRatingDetail.WrittenVerbalSkill = appraisalRatingSaveUpdateReqModel.WrittenVerbalSkill;
                    appraisalRatingDetail.InitiativeMotivation = appraisalRatingSaveUpdateReqModel.InitiativeMotivation;
                    appraisalRatingDetail.TeamWork = appraisalRatingSaveUpdateReqModel.TeamWork;
                    appraisalRatingDetail.ProblemSolvingAbillity = appraisalRatingSaveUpdateReqModel.ProblemSolvingAbillity;
                    appraisalRatingDetail.Attendance = appraisalRatingSaveUpdateReqModel.Attendance;
                    appraisalRatingDetail.TeamWork = appraisalRatingSaveUpdateReqModel.TeamWork;
                    appraisalRatingDetail.Comment = appraisalRatingSaveUpdateReqModel.Comment;

                    appraisalRatingDetail.UpdatedDate = _commonHelper.GetCurrentDateTime();
                    appraisalRatingDetail.UpdatedBy = _commonHelper.GetLoggedInUserId();


                    _dbContext.Entry(appraisalRatingDetail).State = EntityState.Modified;
                    _dbContext.SaveChanges();

                    commonResponse.Status = true;
                    commonResponse.StatusCode = HttpStatusCode.OK;
                    commonResponse.Message = "Appraisal Rating Updated Successfully!";
                }
                else
                {
                    //Add Mode
                    appraisalRatingMst.AppraisalId = appraisalRatingSaveUpdateReqModel.Id;
                    appraisalRatingMst.RatingFromUserId = _commonHelper.GetLoggedInUserId();
                    appraisalRatingMst.RatingToUserId = appraisalRatingSaveUpdateReqModel.ReportingManagerId;
                    appraisalRatingMst.QualityOfWork = appraisalRatingSaveUpdateReqModel.QualityOfWork;
                    appraisalRatingMst.GoalNtarget = appraisalRatingSaveUpdateReqModel.GoalNtarget;
                    appraisalRatingMst.WrittenVerbalSkill = appraisalRatingSaveUpdateReqModel.WrittenVerbalSkill;
                    appraisalRatingMst.InitiativeMotivation = appraisalRatingSaveUpdateReqModel.InitiativeMotivation;
                    appraisalRatingMst.TeamWork = appraisalRatingSaveUpdateReqModel.TeamWork;
                    appraisalRatingMst.ProblemSolvingAbillity = appraisalRatingSaveUpdateReqModel.ProblemSolvingAbillity;
                    appraisalRatingMst.Attendance = appraisalRatingSaveUpdateReqModel.Attendance;
                    appraisalRatingMst.TeamWork = appraisalRatingSaveUpdateReqModel.TeamWork;
                    appraisalRatingMst.Total = appraisalRatingSaveUpdateReqModel.Total;
                    appraisalRatingMst.Comment = appraisalRatingSaveUpdateReqModel.Comment;
                    appraisalRatingMst.CreatedDate = _commonHelper.GetCurrentDateTime();
                    appraisalRatingMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
                    appraisalRatingMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                    appraisalRatingMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                    appraisalRatingMst.IsActive = true;
                    appraisalRatingMst.IsDelete = false;


                    _dbContext.Add(appraisalRatingMst);
                    _dbContext.SaveChanges();

                    commonResponse.Status = true;
                    commonResponse.StatusCode = HttpStatusCode.OK;
                    commonResponse.Message = "Appraisal Rating Added Successfully!";

                }
                commonResponse.Data = appraisalRatingMst;
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Data = ex;
            }

            return Json(commonResponse);
        }


    }
}
