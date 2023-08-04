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
    public class PolicyController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly CommonHelper _commonHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ArcheOneDbContext _dbContext;
        public PolicyController(DbRepo dbRepo, CommonHelper commonHelper, IWebHostEnvironment webHostEnvironment, ArcheOneDbContext dbContext)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
        }


        public async Task<IActionResult> Policy()
        {
            int userId = _commonHelper.GetLoggedInUserId();
            bool showAddSalaryButton = false;

            CommonResponse departmentDetailsResponse = await new CommonController(_dbRepo, _dbContext, _commonHelper).GetDepartmentByUserId(userId);

            if (departmentDetailsResponse.Status)
            {
                showAddSalaryButton = departmentDetailsResponse.Data.DepartmentCode == CommonEnums.DepartmentMst.Human_Resource.ToString();
            }
            showAddSalaryButton = !showAddSalaryButton ? _commonHelper.CheckHasPermission(CommonEnums.PermissionMst.Salary_Add_View) : showAddSalaryButton;

            return View(showAddSalaryButton);
        }

        public async Task<IActionResult> PolicyList()
        {
            CommonResponse response = new CommonResponse();

            GetPolicyListResModel getPolicyListResModel = new GetPolicyListResModel();

            int userId = _commonHelper.GetLoggedInUserId();
            bool isUserHR = false;

            CommonResponse departmentDetailsResponse = await new CommonController(_dbRepo, _dbContext, _commonHelper).GetDepartmentByUserId(userId);

            if (departmentDetailsResponse.Status)
            {
                isUserHR = departmentDetailsResponse.Data.DepartmentCode == CommonEnums.DepartmentMst.Human_Resource.ToString();
            }
            isUserHR = !isUserHR ? _commonHelper.CheckHasPermission(CommonEnums.PermissionMst.Policy_Delete_View) : isUserHR;

            try
            {
                getPolicyListResModel.IsDeletable = isUserHR;

                getPolicyListResModel.PolicyDetails = new List<GetPolicyListResModel.PolicyDetail>();
                getPolicyListResModel.PolicyDetails = await _dbRepo.PolicyList().Where(x => !isUserHR ? x.PolicyName == "HRPolicy" : true).Select(x => new GetPolicyListResModel.PolicyDetail
                {
                    Id = x.Id,
                    PolicyName = x.PolicyName,
                    PolicyDocument = x.PolicyDocumentName,
                }).ToListAsync();

                response.Data = getPolicyListResModel;
                if (getPolicyListResModel != null)
                {
                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Data found successfully!";
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
            return View(getPolicyListResModel);
        }

        public IActionResult AddEditPolicy(int Id)
        {
            CommonResponse response = new CommonResponse();
            AddEditPolicyReqModel addEditPolicyReqModel = new AddEditPolicyReqModel();
            try
            {
                if (Id > 0)
                {
                    var policyList = _dbRepo.PolicyList().FirstOrDefault(x => x.Id == Id);

                    addEditPolicyReqModel.Id = policyList.Id;
                    addEditPolicyReqModel.PolicyName = policyList.PolicyName;
                    addEditPolicyReqModel.PolicyDocumentName = policyList.PolicyDocumentName;

                    response.Status = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = "GetAll PolicyList Successfully";
                }
                else
                {
                    response.Message = "No Data Found";
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                }
                response.Data = addEditPolicyReqModel;

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return View(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveUpdatePolicy(PolicySaveUpdateReqModel policySaveUpdateReqModel)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                IFormFile file;
                string fileName = string.Empty;
                bool validateFileExtension = false;
                bool validateFileSize = false;
                string filePath = string.Empty;
                PolicyMst policyMst = new PolicyMst();
                if (policySaveUpdateReqModel.PolicyDocumentName != null)
                {
                    file = policySaveUpdateReqModel.PolicyDocumentName;
                    fileName = file.FileName;
                    FileInfo fileInfo = new FileInfo(fileName);
                    string fileExtension = fileInfo.Extension;
                    long fileSize = file.Length;

                    string[] allowedFileExtensions = { CommonConstant.pdf };
                    long allowedFileSize = 1 * 1024 * 1024 * 10; // 10MB
                    validateFileExtension = allowedFileExtensions.Contains(fileExtension) ? true : false;
                    validateFileSize = fileSize <= allowedFileSize ? true : false;
                    fileName = policySaveUpdateReqModel.PolicyName + policySaveUpdateReqModel.Id + fileExtension;
                    if (validateFileExtension && validateFileSize)
                    {
                        if (policySaveUpdateReqModel.PolicyName == "HRPolicy")
                        {
                            var policyFile = _commonHelper.UploadFile(policySaveUpdateReqModel.PolicyDocumentName, @"DefaultPolicyDocument", fileName, false, true, false);
                            filePath = policyFile.Data.RelativePath;
                        }
                        else
                        {
                            var policyFile = _commonHelper.UploadFile(policySaveUpdateReqModel.PolicyDocumentName, @"PolicyDocument", fileName, false, true, false);
                            filePath = policyFile.Data.RelativePath;
                        }
                        var policyDetail = await _dbRepo.PolicyList().FirstOrDefaultAsync(x => x.Id == policySaveUpdateReqModel.Id);
                        if (policyDetail != null)
                        {
                            //Edit Mode
                            policyDetail.PolicyName = policySaveUpdateReqModel.PolicyName;
                            if (!string.IsNullOrEmpty(filePath))
                            {
                                policyDetail.PolicyDocumentName = filePath;
                            }
                            policyDetail.UpdatedDate = _commonHelper.GetCurrentDateTime();
                            policyDetail.UpdatedBy = _commonHelper.GetLoggedInUserId();

                            _dbContext.Entry(policyDetail).State = EntityState.Modified;
                            _dbContext.SaveChanges();

                            response.Status = true;
                            response.StatusCode = HttpStatusCode.OK;
                            response.Message = "Policy Updated Successfully!";
                        }
                        else
                        {
                            //Add Mode
                            var duplicateCheck = await _dbRepo.PolicyList().Where(x => x.PolicyName == policySaveUpdateReqModel.PolicyName).ToListAsync();
                            if (duplicateCheck.Count == 0)
                            {
                                policyMst.PolicyName = policySaveUpdateReqModel.PolicyName;

                                policyMst.PolicyDocumentName = filePath;
                                policyMst.CreatedDate = _commonHelper.GetCurrentDateTime();
                                policyMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
                                policyMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                                policyMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                                policyMst.IsActive = true;
                                policyMst.IsDelete = false;

                                _dbContext.Add(policyMst);
                                _dbContext.SaveChanges();

                                response.Status = true;
                                response.StatusCode = HttpStatusCode.OK;
                                response.Message = "Policy Added Successfully!";
                            }
                            else
                            {
                                response.Message = "Policy Is Already Exist";
                            }
                        }

                    }
                    else
                    {

                        response.Message = "Only PDF files are Allowed !";
                    }
                }

                response.Data = policyMst;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return Json(response);
        }

        public async Task<IActionResult> DeletePolicy(int Id)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                if (Id > 0)
                {
                    var policyList = await _dbRepo.PolicyList().FirstOrDefaultAsync(x => x.Id == Id);
                    if (policyList != null)
                    {
                        _dbContext.Remove(policyList);
                        _dbContext.SaveChanges();

                        response.Status = true;
                        response.StatusCode = HttpStatusCode.OK;
                        response.Message = "Policy Deleted Successfully";
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

        public async Task<FileResult> GetPolicyReport(int? Id)
        {
            CommonResponse response = new CommonResponse();
            string DefaultPolicy = "Files\\DefaultPolicyDocument\\HRPolicy0.pdf";
            byte[] FileBytes = System.IO.File.ReadAllBytes(Path.Combine(_commonHelper.GetPhysicalRootPath(false), DefaultPolicy));
            try
            {
                if (Id > 0)
                {
                    var policyList = await _dbRepo.PolicyList().FirstOrDefaultAsync(x => x.Id == Id);

                    string ReportURL = policyList.PolicyDocumentName;

                    FileBytes = System.IO.File.ReadAllBytes(Path.Combine(_commonHelper.GetPhysicalRootPath(false), ReportURL));
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
            return File(FileBytes, "application/pdf");

        }

    }
}
