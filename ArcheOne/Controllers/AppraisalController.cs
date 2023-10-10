using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.RegularExpressions;

namespace ArcheOne.Controllers
{
    public class AppraisalController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly CommonHelper _commonHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ArcheOneDbContext _dbContext;
        public AppraisalController(DbRepo dbRepo, CommonHelper commonHelper, IWebHostEnvironment webHostEnvironment, ArcheOneDbContext dbContext)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Appraisal()
        {
            int userId = _commonHelper.GetLoggedInUserId();
            bool showAddAppraisalButton = false;

            CommonResponse departmentDetailsResponse = await new CommonController(_dbRepo, _dbContext, _commonHelper).GetDepartmentByUserId(userId);

            if (departmentDetailsResponse.Status)
            {
                showAddAppraisalButton = departmentDetailsResponse.Data.DepartmentCode == CommonEnums.DepartmentMst.Human_Resource.ToString();
            }
            showAddAppraisalButton = !showAddAppraisalButton ? _commonHelper.CheckHasPermission(CommonEnums.PermissionMst.Appraisal_Add_View) : showAddAppraisalButton;

            return View(showAddAppraisalButton);
            /*AppraisalResModel appraisalResModel = new AppraisalResModel();

            var managerRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("Manager")).ToList();
            var managerroleIdList = managerRoleList.Select(x => x.Id).ToList();

            var hrRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("HR")).ToList();
            var hrroleIdList = hrRoleList.Select(x => x.Id).ToList();

            var adminRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("Admin")).ToList();
            var adminroleIdList = adminRoleList.Select(x => x.Id).ToList();

            var employeeRoleList = _dbRepo.RoleMstList().Where(x => !x.RoleCode.Contains("HR") && !x.RoleCode.Contains("Manager") && !x.RoleCode.Contains("Admin")).ToList();
            var employeeroleIdList = employeeRoleList.Select(x => x.Id).ToList();

            var loginUserList = _dbRepo.AllUserMstList().Where(x => x.RoleId != null && x.Id == _commonHelper.GetLoggedInUserId());

            var IsUserManager = loginUserList.Where(x => managerroleIdList.Contains(x.RoleId)).ToList();
            var IsUserHR = loginUserList.Where(x => hrroleIdList.Contains(x.RoleId)).ToList();
            var IsUserEmployee = loginUserList.Where(x => employeeroleIdList.Contains(x.RoleId)).ToList();


            appraisalResModel.IsUserHR = IsUserHR.Count > 0 ? true : false;
            appraisalResModel.IsUserManager = IsUserManager.Count > 0 ? true : false;
            appraisalResModel.IsUserEmployee = IsUserEmployee.Count > 0 ? true : false;


            return View(appraisalResModel);*/
        }

        public async Task<IActionResult> AppraisalList(int? AppraisalStatusId)
        {
            CommonResponse response = new CommonResponse();
            List<GetAppraisalListResModel> getAppraisalListResModel = new List<GetAppraisalListResModel>();

            try
            {
                int userId = _commonHelper.GetLoggedInUserId();
                bool isUserHR = false;

                CommonResponse departmentDetailsResponse = await new CommonController(_dbRepo, _dbContext, _commonHelper).GetDepartmentByUserId(userId);

                if (departmentDetailsResponse.Status)
                {
                    isUserHR = departmentDetailsResponse.Data.DepartmentCode == CommonEnums.DepartmentMst.Human_Resource.ToString();
                };

                if (isUserHR)
                {
                    getAppraisalListResModel = await (from a in _dbRepo.AppraisalList().Where(x => AppraisalStatusId != null ? AppraisalStatusId == 1 ? x.IsApprove == false : x.IsApprove == true : true)
                                                      join b in _dbRepo.UserMstList() on a.EmployeeId equals b.Id
                                                      join c in _dbRepo.UserMstList() on a.ReportingManagerId equals c.Id into cGroup
                                                      from cItem in cGroup.DefaultIfEmpty()
                                                      join d in _dbRepo.AppraisalRatingList() on a.Id equals d.AppraisalId into dGroup
                                                      select new GetAppraisalListResModel
                                                      {
                                                          Id = a.Id,
                                                          Year = a.Year,
                                                          AppraisalStaus = a.IsApprove ?? false ? "Completed" : "InProgress",
                                                          EmployeeName = $"{b.FirstName} {b.LastName}",
                                                          ReportingManagerName = $"{cItem.FirstName} {cItem.LastName}",
                                                          IsUserHR = isUserHR,
                                                          IsEditable = !a.IsApprove ?? false,
                                                          IsManagerEditable = false,
                                                          IsHRManagerEditable = isUserHR && dGroup.Count() <= 0 ? false : true
                                                      }).ToListAsync();
                }
                else
                {
                    getAppraisalListResModel = await (from a in _dbRepo.AppraisalList().Where(x => (x.EmployeeId == userId || x.ReportingManagerId == userId) && (AppraisalStatusId != null ? AppraisalStatusId == 1 ? x.IsApprove == false : x.IsApprove == true : true))
                                                      join b in _dbRepo.UserMstList() on a.EmployeeId equals b.Id
                                                      join c in _dbRepo.UserMstList() on a.ReportingManagerId equals c.Id into cGroup
                                                      from cItem in cGroup.DefaultIfEmpty()
                                                      join d in _dbRepo.AppraisalRatingList() on a.EmployeeId equals d.RatingFromUserId into dGroup
                                                      select new GetAppraisalListResModel
                                                      {
                                                          Id = a.Id,
                                                          Year = a.Year,
                                                          AppraisalStaus = a.IsApprove ?? false ? "Completed" : "InProgress",
                                                          EmployeeName = $"{b.FirstName} {b.LastName}",
                                                          ReportingManagerName = $"{cItem.FirstName} {cItem.LastName}",
                                                          IsUserHR = isUserHR,
                                                          IsEditable = !a.IsApprove ?? false,
                                                          IsManagerEditable = a.EmployeeId == userId ? true : a.ReportingManagerId == userId ? dGroup.Count() <= 0 ? false : true : false,
                                                          IsHRManagerEditable = false
                                                      }).ToListAsync();
                }




                /*var appraisalList = _dbRepo.AppraisalList().ToList();
                var appraisalRatingList1 = _dbRepo.AppraisalRatingList().ToList();
                var AllUserList = _dbRepo.AllUserMstList().ToList();

                var hrRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("HR")).ToList();
                var hrroleIdList = hrRoleList.Select(x => x.Id).ToList();

                var managerRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("Manager")).ToList();
                var managerroleIdList = managerRoleList.Select(x => x.Id).ToList();

                var employeeRoleList = _dbRepo.RoleMstList().Where(x => !x.RoleCode.Contains("Manager") && !x.RoleCode.Contains("HR")).ToList();
                var employeeroleIdList = employeeRoleList.Select(x => x.Id).ToList();

                var loginUserList = _dbRepo.AllUserMstList().Where(x => x.RoleId != null && x.Id == _commonHelper.GetLoggedInUserId());

                var IsUserHR = loginUserList.Where(x => hrroleIdList.Contains(x.RoleId)).ToList();
                var IsUserManager = loginUserList.Where(x => managerroleIdList.Contains(x.RoleId)).ToList();
                var IsUserEmployee = loginUserList.Where(x => employeeroleIdList.Contains(x.RoleId)).ToList();



                if (IsUserManager.Count > 0)
                {
                    appraisalList = _dbRepo.AppraisalList().Where(x => x.ReportingManagerId == _commonHelper.GetLoggedInUserId() && x.IsApprove != true).ToList();
                }


                if (IsUserEmployee.Count > 0)
                {
                    appraisalList = _dbRepo.AppraisalList().Where(x => x.EmployeeId == _commonHelper.GetLoggedInUserId() && x.IsApprove != true).ToList();
                }
                if (IsUserHR.Count > 0)
                {
                    if (AppraisalStatusId == 1 || AppraisalStatusId == null)
                    {
                        appraisalList = _dbRepo.AppraisalList().Where(x => x.IsApprove == false).ToList();
                    }
                    else
                    {
                        appraisalList = _dbRepo.AppraisalList().Where(x => x.IsApprove == true).ToList();
                    }
                }*/


                if (getAppraisalListResModel.Count > 0)
                {
                    /*
                                        foreach (var item in appraisalList)
                                        {
                                            var EmployeeNamedetails = AllUserList.FirstOrDefault(x => x.Id == item.EmployeeId);
                                            var ReportingManagerDetails = AllUserList.FirstOrDefault(x => x.Id == item.ReportingManagerId);
                                            var IsManagerEditable = appraisalRatingList.FirstOrDefault(x => x.RatingFromUserId == item.EmployeeId);

                                            GetAppraisalListResModel getAppraisalListResModel1 = new GetAppraisalListResModel();
                                            getAppraisalListResModel1.Id = item.Id;
                                            getAppraisalListResModel1.EmployeeName = EmployeeNamedetails.FirstName + " " + EmployeeNamedetails.LastName;
                                            getAppraisalListResModel1.ReportingManagerName = ReportingManagerDetails.FirstName + " " + ReportingManagerDetails.LastName;
                                            getAppraisalListResModel1.Year = item.Year;
                                            getAppraisalListResModel1.AppraisalStaus = item.IsApprove == true ? "Completed" : "InProgress";
                                            getAppraisalListResModel1.IsUserHR = IsUserHR.Count > 0 ? true : false;
                                            getAppraisalListResModel1.IsEditable = item.IsApprove == true ? false : true;
                                            if (IsUserManager.Count > 0)
                                            {
                                                getAppraisalListResModel1.IsManagerEditable = IsManagerEditable == null ? false : true;
                                            }
                                            if (IsUserEmployee.Count > 0)
                                            {
                                                getAppraisalListResModel1.IsManagerEditable = true;
                                            }
                                            if (IsUserHR.Count > 0)
                                            {
                                                getAppraisalListResModel1.IsHRManagerEditable = IsManagerEditable == null ? false : true;
                                            }
                                            getAppraisalListResModel.Add(getAppraisalListResModel1);
                                        }*/



                    response.Data = getAppraisalListResModel;
                    response.Status = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = "GetAll Appraisal Successfully";
                }
                else
                {
                    response.Message = "No Data Found";
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return View(getAppraisalListResModel);
        }

        public IActionResult AddEditAppraisal(int Id)
        {
            CommonResponse response = new CommonResponse();
            AddEditAppraisalResModel addEditAppraisalResModel = new AddEditAppraisalResModel();
            addEditAppraisalResModel.reportingManagetDetail = new ReportingManagetDetail();
            addEditAppraisalResModel.reportingManagetDetail.EmployeeDetail = new EmployeeDetail();

            var userList = _dbRepo.AllUserMstList();

            var employeeList = userList.Where(x => x.RoleId > 2 && x.Id != _commonHelper.GetLoggedInUserId()).ToList(); //Only Manager,Team Lead and Professional

            addEditAppraisalResModel.EmployeeId = employeeList;

            try
            {
                if (Id > 0)
                {
                    var appraisal = _dbRepo.AppraisalList().FirstOrDefault(x => x.Id == Id);

                    if (appraisal != null)
                    {
                        addEditAppraisalResModel.reportingManagetDetail.ReportingManagerId = appraisal.ReportingManagerId;
                        addEditAppraisalResModel.reportingManagetDetail.ReportingManagerName = userList.Where(x => x.Id == appraisal.ReportingManagerId).Select(x => $"{x.FirstName} {x.LastName}").FirstOrDefault() ?? "N/A";
                        addEditAppraisalResModel.reportingManagetDetail.EmployeeDetail.EmployeeId = appraisal.EmployeeId;
                        addEditAppraisalResModel.Id = appraisal.Id;
                        addEditAppraisalResModel.Year = appraisal.Year;

                        response.Status = true;
                        response.StatusCode = System.Net.HttpStatusCode.OK;
                        response.Message = "Get Appraisal Successfully";
                        response.Data = addEditAppraisalResModel;
                    }
                    else
                    {
                        response.Message = "Data Not Found";
                        response.StatusCode = System.Net.HttpStatusCode.NotFound;

                    }
                }
                response.Data = addEditAppraisalResModel;

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return View(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveUpdateAppraisal([FromBody] AppraisalSaveUpdateReqModel appraisalSaveUpdateReqModel)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                //int currentYear = DateTime.Now.Year;
                //int perviousYear = DateTime.Now.AddYears(1).Year;
                Regex validateDateRegex1 = new Regex("^[2]{1}[0]{1}[0-8]{1}[0-9]{1}-[2]{1}[0]{1}[0-8]{1}[0-9]{1}");
                if (validateDateRegex1.IsMatch(appraisalSaveUpdateReqModel.Year))
                {
                    string firstYear = appraisalSaveUpdateReqModel.Year.Substring(0, appraisalSaveUpdateReqModel.Year.IndexOf("-")).Trim();
                    int nextyear = int.Parse(firstYear) + 1;
                    string finalYear = firstYear + "-" + nextyear;
                    /*if (firstYear == Convert.ToString(currentYear))
                    {*/
                    /*if (appraisalSaveUpdateReqModel.Year == finalYear)
                    {*/
                    AppraisalMst appraisalMst = new AppraisalMst();
                    var appraisalDetail = await _dbRepo.AppraisalList().FirstOrDefaultAsync(x => x.Id == appraisalSaveUpdateReqModel.Id);
                    var duplicateCheck = await _dbRepo.AppraisalList().Where(x => x.EmployeeId == appraisalSaveUpdateReqModel.EmployeeId && x.Year == appraisalSaveUpdateReqModel.Year).ToListAsync();
                    if (appraisalDetail != null)
                    {
                        if (duplicateCheck.Count == 0)
                        {

                            //Edit Mode
                            appraisalDetail.EmployeeId = appraisalSaveUpdateReqModel.EmployeeId;
                            appraisalDetail.ReportingManagerId = appraisalSaveUpdateReqModel.ReportingManagerId;
                            appraisalDetail.Year = appraisalSaveUpdateReqModel.Year;
                            appraisalDetail.UpdatedDate = _commonHelper.GetCurrentDateTime();
                            appraisalDetail.UpdatedBy = _commonHelper.GetLoggedInUserId();


                            _dbContext.Entry(appraisalDetail).State = EntityState.Modified;
                            _dbContext.SaveChanges();

                            response.Status = true;
                            response.StatusCode = HttpStatusCode.OK;
                            response.Message = "Appraisal Updated Successfully!";
                        }
                        else
                        {
                            response.Message = "Appraisal Is Already Exist";
                        }
                    }
                    else
                    {
                        //Add Mode
                        duplicateCheck = await _dbRepo.AppraisalList().Where(x => x.EmployeeId == appraisalSaveUpdateReqModel.EmployeeId && x.Year == appraisalSaveUpdateReqModel.Year).ToListAsync();
                        if (duplicateCheck.Count == 0)
                        {
                            appraisalMst.EmployeeId = appraisalSaveUpdateReqModel.EmployeeId;
                            appraisalMst.ReportingManagerId = appraisalSaveUpdateReqModel.ReportingManagerId;
                            appraisalMst.Year = appraisalSaveUpdateReqModel.Year;
                            appraisalMst.IsApprove = false;
                            appraisalMst.CreatedDate = _commonHelper.GetCurrentDateTime();
                            appraisalMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
                            appraisalMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                            appraisalMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                            appraisalMst.IsActive = true;
                            appraisalMst.IsDelete = false;


                            _dbContext.Add(appraisalMst);
                            _dbContext.SaveChanges();

                            response.Status = true;
                            response.StatusCode = HttpStatusCode.OK;
                            response.Message = "Appraisal Added Successfully!";
                        }
                        else
                        {
                            response.Message = "Appraisal Is Already Exist";
                        }
                    }
                    response.Data = appraisalMst;
                    /* }
                     else
                     {
                         response.Message = "Please enter valid year " + finalYear!;

                     }
                 }
                 else
                 {
                     response.Message = "Please enter current year " + currentYear!;
                 }*/
                }
                else
                {
                    response.Message = "Please enter valid year (yyyy-yyyy)!";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAppraisal(int Id)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                if (Id > 0)
                {
                    var appraisalDetail = await _dbRepo.AppraisalList().FirstOrDefaultAsync(x => x.Id == Id);
                    var appraisalratingdetails = await _dbRepo.AppraisalRatingList().FirstOrDefaultAsync(x => x.AppraisalId == Id);
                    if (appraisalDetail != null)
                    {
                        _dbContext.Remove(appraisalDetail);
                        _dbContext.SaveChanges();
                        if (appraisalratingdetails != null)
                        {
                            _dbContext.Remove(appraisalratingdetails);
                            _dbContext.SaveChanges();
                        }
                        response.Status = true;
                        response.StatusCode = HttpStatusCode.OK;
                        response.Message = "Appraisal Deleted Successfully";
                    }
                    else
                    {
                        response.Message = "Data not found!";
                        response.StatusCode = HttpStatusCode.NotFound;
                    }
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

        [HttpGet]
        public async Task<IActionResult> GetReportingManagerByUserId(int UserId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var reportingManager = await (from userDetail in _dbRepo.UserDetailList().Where(x => x.UserId == UserId)
                                              join userMst in _dbRepo.UserMstList() on userDetail.ReportingManager equals userMst.Id into userMstGroup
                                              from userMstItem in userMstGroup.DefaultIfEmpty()
                                              select new
                                              {
                                                  userMstItem.Id,
                                                  FullName = $"{userMstItem.FirstName} {userMstItem.LastName}"
                                              }).FirstOrDefaultAsync();

                if (reportingManager != null)
                {
                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Data found successfully!";
                    response.Data = reportingManager;
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
