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

        public IActionResult Appraisal()
        {
            AppraisalResModel appraisalResModel = new AppraisalResModel();

            var managerRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("Manager")).ToList();
            var managerroleIdList = managerRoleList.Select(x => x.Id).ToList();

            var hrRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("HR")).ToList();
            var hrroleIdList = hrRoleList.Select(x => x.Id).ToList();

            var adminRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("Admin")).ToList();
            var adminroleIdList = adminRoleList.Select(x => x.Id).ToList();

            var employeeRoleList = _dbRepo.RoleMstList().Where(x => !x.RoleCode.Contains("HR") && !x.RoleCode.Contains("Manager") && !x.RoleCode.Contains("Admin")).ToList();
            var employeeroleIdList = employeeRoleList.Select(x => x.Id).ToList();

            var loginUserList = _dbRepo.AllUserMstList().Where(x => x.RoleId != null && x.Id == _commonHelper.GetLoggedInUserId());

            var IsUserManager = loginUserList.Where(x => managerroleIdList.Contains(x.RoleId.Value)).ToList();
            var IsUserHR = loginUserList.Where(x => hrroleIdList.Contains(x.RoleId.Value)).ToList();
            var IsUserEmployee = loginUserList.Where(x => employeeroleIdList.Contains(x.RoleId.Value)).ToList();


            appraisalResModel.IsUserHR = IsUserHR.Count > 0 ? true : false;
            appraisalResModel.IsUserManager = IsUserManager.Count > 0 ? true : false;
            appraisalResModel.IsUserEmployee = IsUserEmployee.Count > 0 ? true : false;


            return View(appraisalResModel);
        }

        public IActionResult AppraisalList()
        {
            CommonResponse commonResponse = new CommonResponse();
            List<GetAppraisalListResModel> getAppraisalListResModel = new List<GetAppraisalListResModel>();
            try
            {
                var appraisalList = _dbRepo.AppraisalList().ToList(); 

                var hrRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("HR")).ToList();
                var hrroleIdList = hrRoleList.Select(x => x.Id).ToList();

                var managerRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("Manager")).ToList();
                var managerroleIdList = managerRoleList.Select(x => x.Id).ToList();

                var employeeRoleList = _dbRepo.RoleMstList().Where(x => !x.RoleCode.Contains("Manager") &&!x.RoleCode.Contains("HR")).ToList();
                var employeeroleIdList = employeeRoleList.Select(x => x.Id).ToList();

                var loginUserList = _dbRepo.AllUserMstList().Where(x => x.RoleId != null && x.Id == _commonHelper.GetLoggedInUserId());

                var IsUserHR = loginUserList.Where(x => hrroleIdList.Contains(x.RoleId.Value)).ToList();
                var IsUserManager = loginUserList.Where(x => managerroleIdList.Contains(x.RoleId.Value)).ToList();
                var IsUserEmployee = loginUserList.Where(x => employeeroleIdList.Contains(x.RoleId.Value)).ToList();



                if (IsUserManager.Count > 0)
                {
                    appraisalList = _dbRepo.AppraisalList().Where(x => x.ReportingManagerId == _commonHelper.GetLoggedInUserId()).ToList();
                }
              

                if (IsUserEmployee.Count > 0)
                {
                    appraisalList = _dbRepo.AppraisalList().Where(x => x.EmployeeId == _commonHelper.GetLoggedInUserId()).ToList();
                }
                if (IsUserHR.Count > 0)
                {
                    appraisalList = _dbRepo.AppraisalList().ToList();
                }
              

                if (appraisalList.Count > 0)
                {

                    getAppraisalListResModel = (from u in appraisalList
                                                join r in _dbRepo.AllUserMstList()
                                                on u.EmployeeId equals r.Id
                                                join i in _dbRepo.AllUserMstList()
                                                on u.ReportingManagerId equals i.Id
                                                select new { u, r, i }).Select(x => new GetAppraisalListResModel
                                                {
                                                    Id = x.u.Id,            
                                                    EmployeeName = x.r.FirstName + " " + x.r.LastName,
                                                    ReportingManagerName = x.i.FirstName + " " + x.i.LastName,
                                                    Year = x.u.Year,
                                                    IsUserHR = IsUserHR.Count > 0 ? true : false
                                                }).ToList();


                    commonResponse.Data = getAppraisalListResModel;


                    commonResponse.Status = true;
                    commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
                    commonResponse.Message = "GetAll Appraisal Successfully";
                }
                else
                {
                    commonResponse.Message = "No Data Found";
                    commonResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                commonResponse.Data = ex;
                commonResponse.Message = ex.Message;
            }
            return View(getAppraisalListResModel);
        }

        public IActionResult AddEditAppraisal(int Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            AddEditAppraisalResModel addEditAppraisalResModel = new AddEditAppraisalResModel();
            addEditAppraisalResModel.reportingManagetDetail = new ReportingManagetDetail();
            addEditAppraisalResModel.reportingManagetDetail.EmployeeDetail = new EmployeeDetail();

            var roleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("Manager")).ToList();
            var roleIdList = roleList.Select(x => x.Id).ToList();

            var hrroleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("HR") || x.RoleCode.Contains("Manager")).ToList();
            var hrroleIdList = hrroleList.Select(x => x.Id).ToList();

            var adminroleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("Admin")).ToList();
            var adminroleIdList = adminroleList.Select(x => x.Id).ToList();

            var userList = _dbRepo.AllUserMstList().Where(x => x.RoleId != null);

            var reportingManagerList = userList.Where(x => roleIdList.Contains(x.RoleId.Value)).ToList();
            var employeeList = userList.Where(x =>  !hrroleIdList.Contains(x.RoleId.Value) && !adminroleIdList.Contains(x.RoleId.Value)).ToList();


            addEditAppraisalResModel.EmployeeId = employeeList;
            addEditAppraisalResModel.ReportingManagerId = reportingManagerList;
        
            try
            {
                if (Id > 0)
                {
                    var appraisal = _dbRepo.AppraisalList().FirstOrDefault(x => x.Id == Id);
                 
                    if (appraisal != null)
                    {
                        addEditAppraisalResModel.reportingManagetDetail.ReportingManagerId = appraisal.ReportingManagerId;
                        addEditAppraisalResModel.reportingManagetDetail.EmployeeDetail.EmployeeId = appraisal.EmployeeId;
                        addEditAppraisalResModel.Id = appraisal.Id;
                        addEditAppraisalResModel.Year = appraisal.Year;

                        commonResponse.Status = true;
                        commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
                        commonResponse.Message = "Get Appraisal Successfully";
                        commonResponse.Data = addEditAppraisalResModel;
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
                commonResponse.Data = addEditAppraisalResModel;

            }
            catch (Exception ex)
            {
                commonResponse.Message= ex.Message;
                commonResponse.Data = ex;
            }
            return View(commonResponse.Data);
        }
        [HttpPost]
        public async Task<IActionResult> SaveUpdateAppraisal([FromBody] AppraisalSaveUpdateReqModel appraisalSaveUpdateReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
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

                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Message = "Appraisal Updated Successfully!";
                    }
                    else
                    {
                        commonResponse.Message = "Appraisal Is Already Exist";
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
                        appraisalMst.CreatedDate = _commonHelper.GetCurrentDateTime();
                        appraisalMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
                        appraisalMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                        appraisalMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                        appraisalMst.IsActive = true;
                        appraisalMst.IsDelete = false;


                        _dbContext.Add(appraisalMst);
                        _dbContext.SaveChanges();

                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Message = "Appraisal Added Successfully!";
                    }
                    else
                    {
                        commonResponse.Message = "Appraisal Is Already Exist";
                    }
                }
                commonResponse.Data = appraisalMst;
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Data = ex;
            }

            return Json(commonResponse);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAppraisal(int Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                if (Id > 0)
                {
                    var appraisalDetail = await _dbRepo.AppraisalList().FirstOrDefaultAsync(x => x.Id == Id);
                    if (appraisalDetail != null)
                    {
                        _dbContext.Remove(appraisalDetail);
                        _dbContext.SaveChanges();

                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Message = "Appraisal Deleted Successfully";
                    }
                    else
                    {
                        commonResponse.Message = "Data not found!";
                        commonResponse.StatusCode = HttpStatusCode.NotFound;
                    }
                }
                else
                {
                    commonResponse.Message = "Data not found!";
                    commonResponse.StatusCode = HttpStatusCode.NotFound;
                }

            }
            catch (Exception ex)
            {
                commonResponse.Data = ex;
                commonResponse.Message = ex.Message;
            }

            return Json(commonResponse);

        }

    }
}
