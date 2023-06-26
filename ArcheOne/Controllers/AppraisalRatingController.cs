using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;

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
            AddEditAppraisalRatingResModel addEditAppraisalRatingResModel= new AddEditAppraisalRatingResModel();
            addEditAppraisalRatingResModel.reportingManagetDetail = new ManagetDetail();
            addEditAppraisalRatingResModel.reportingManagetDetail.EmployeeDetail = new EmployeesDetail();
            addEditAppraisalRatingResModel.appraisalRating = new AppraisalRating();

            var roleManagerList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("Manager") && !x.RoleCode.Contains("HR")).ToList();
            var roleManagerIdList = roleManagerList.Select(x => x.Id).ToList();
            var userList = _dbRepo.AllUserMstList().Where(x => x.RoleId != null && x.Id == _commonHelper.GetLoggedInUserId());

            var roleHRList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("HR")).ToList();
            var roleHRIdList = roleHRList.Select(x => x.Id).ToList();
     
           
            var reportingManagerList = userList.Where(x => roleManagerIdList.Contains(x.RoleId.Value)).ToList();

            var employeeList = userList.Where(x => !roleManagerIdList.Contains(x.RoleId.Value) && !roleHRIdList.Contains(x.RoleId.Value)).ToList();
            var HRuserList = userList.Where(x => roleHRIdList.Contains(x.RoleId.Value)).ToList();


            addEditAppraisalRatingResModel.EmployeeId = employeeList;
            addEditAppraisalRatingResModel.ReportingManagerId = reportingManagerList;


            addEditAppraisalRatingResModel.IsUserHR = true;
            addEditAppraisalRatingResModel.IsUserReportManager = false;
            addEditAppraisalRatingResModel.IsUserEmployee = false;
          

            try
            {
                
                if (Id > 0)
                {
                    var appraisal = _dbRepo.AppraisalList().FirstOrDefault(x => x.Id == Id);
                    var managerUserDetail = _dbRepo.AllUserMstList().FirstOrDefault(x => x.Id == appraisal.ReportingManagerId);
                    var employeeUserDetail = _dbRepo.AllUserMstList().FirstOrDefault(x => x.Id == appraisal.EmployeeId);
                    if (appraisal != null)
                    {
                        addEditAppraisalRatingResModel.reportingManagetDetail.ReportingManagerId = appraisal.ReportingManagerId;
                        addEditAppraisalRatingResModel.reportingManagetDetail.EmployeeDetail.EmployeeId = appraisal.EmployeeId;
                        addEditAppraisalRatingResModel.Id = appraisal.Id;
                        addEditAppraisalRatingResModel.Date = appraisal.CreatedDate.Date.ToString("dd-MM-yyyy");
                        addEditAppraisalRatingResModel.ReviewDate = appraisal.UpdatedDate.Date.ToString("dd-MM-yyyy");

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


    }
}
