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
            AddEditAppraisalResModel addEditAppraisalResModel = new AddEditAppraisalResModel();
            addEditAppraisalResModel.reportingManagetDetail = new ReportingManagetDetail();
            addEditAppraisalResModel.reportingManagetDetail.EmployeeDetail = new EmployeeDetail();

            var roleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("Manager")).ToList();
            var roleIdList = roleList.Select(x => x.Id).ToList();
            var userList = _dbRepo.AllUserMstList().Where(x => x.RoleId != null);

            var appraisalList = _dbRepo.AppraisalList().ToList();
            var reportingManagerList = userList.Where(x => roleIdList.Contains(x.RoleId.Value)).ToList();
            var employeeList = userList.Where(x => !roleIdList.Contains(x.RoleId.Value)).ToList();


            addEditAppraisalResModel.EmployeeId = employeeList;
            addEditAppraisalResModel.ReportingManagerId = reportingManagerList;
          

            try
            {
                AppraisalMst appraisalMst = new AppraisalMst();
                if (Id > 0)
                {
                    var appraisal = _dbRepo.AppraisalList().FirstOrDefault(x => x.Id == Id);
                    var managerUserDetail = _dbRepo.AllUserMstList().FirstOrDefault(x => x.Id == appraisal.ReportingManagerId);
                    var employeeUserDetail = _dbRepo.AllUserMstList().FirstOrDefault(x => x.Id == appraisal.EmployeeId);
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
                commonResponse.Message = ex.Message;
                commonResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
            }
            return View(commonResponse.Data);
        }


    }
}
