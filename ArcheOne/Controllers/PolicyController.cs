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


        public IActionResult Policy()
        {
            PolicyResModel policyResModel = new PolicyResModel();


            var hrRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("HR")).ToList();
            var hrroleIdList = hrRoleList.Select(x => x.Id).ToList();


            var loginUserList = _dbRepo.AllUserMstList().Where(x => x.RoleId != null && x.Id == _commonHelper.GetLoggedInUserId());
            var IsUserHr = loginUserList.Where(x => hrroleIdList.Contains(x.RoleId)).ToList();

            policyResModel.IsUserHR = IsUserHr.Count > 0 ? true : false;

            return View(policyResModel);
        }

        public IActionResult PolicyList()
        {
            CommonResponse commonResponse = new CommonResponse();

            List<GetPolicyListResModel> getPolicyListResModel = new List<GetPolicyListResModel>();

            var hrRoleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("HR")).ToList();
            var hrroleIdList = hrRoleList.Select(x => x.Id).ToList();


            var loginUserList = _dbRepo.AllUserMstList().Where(x => x.RoleId != null && x.Id == _commonHelper.GetLoggedInUserId());

            var IsUserHR = loginUserList.Where(x => hrroleIdList.Contains(x.RoleId)).ToList();

            try
            {
                var policyList = _dbRepo.PolicyList().ToList();
                if (policyList.Count > 0)
                {
                    if (IsUserHR.Count > 0)
                    {
                        getPolicyListResModel = _dbRepo.PolicyList().Where(x => x.IsActive == true && x.IsDelete == false).Select(x => new GetPolicyListResModel
                        {
                            Id = x.Id,
                            PolicyName = x.PolicyName,
                            PolicyDocument = x.PolicyDocumentName,
                            IsUserHR = IsUserHR.Count > 0 ? true : false,

                        }).ToList();
                        commonResponse.Data = getPolicyListResModel;
                    }
                    else
                    {
                        getPolicyListResModel = _dbRepo.PolicyList().Where(x => x.PolicyName == "HRPolicy").Select(x => new GetPolicyListResModel
                        {
                            Id = x.Id,
                            PolicyName = x.PolicyName,
                            PolicyDocument = x.PolicyDocumentName,
                            IsUserHR = IsUserHR.Count > 0 ? true : false,

                        }).ToList();
                        commonResponse.Data = getPolicyListResModel;
                    }


                    commonResponse.Status = true;
                    commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
                    commonResponse.Message = "GetAll PolicyList Successfully";
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
            return View(getPolicyListResModel);
        }

        public IActionResult AddEditPolicy(int Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            AddEditPolicyReqModel addEditPolicyReqModel = new AddEditPolicyReqModel();
            try
            {
                if (Id > 0)
                {
                    var policyList = _dbRepo.PolicyList().FirstOrDefault(x => x.Id == Id);

                    addEditPolicyReqModel.Id = policyList.Id;
                    addEditPolicyReqModel.PolicyName = policyList.PolicyName;
                    addEditPolicyReqModel.PolicyDocumentName = policyList.PolicyDocumentName;

                    commonResponse.Status = true;
                    commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
                    commonResponse.Message = "GetAll PolicyList Successfully";
                }
                else
                {
                    commonResponse.Message = "No Data Found";
                    commonResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                }
                commonResponse.Data = addEditPolicyReqModel;

            }
            catch (Exception ex)
            {
                commonResponse.Data = ex;
                commonResponse.Message = ex.Message;
            }

            return View(commonResponse.Data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveUpdatePolicy(PolicySaveUpdateReqModel policySaveUpdateReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
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

                    }
                    else
                    {

                        commonResponse.Message = "Only PDF files are Allowed !";
                    }
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

                    commonResponse.Status = true;
                    commonResponse.StatusCode = HttpStatusCode.OK;
                    commonResponse.Message = "Policy Updated Successfully!";
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

                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Message = "Policy Added Successfully!";
                    }
                    else
                    {
                        commonResponse.Message = "Policy Is Already Exist";
                    }
                }
                commonResponse.Data = policyMst;
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Data = ex;
            }

            return Json(commonResponse);
        }

        public async Task<IActionResult> DeletePolicy(int Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                if (Id > 0)
                {
                    var policyList = await _dbRepo.PolicyList().FirstOrDefaultAsync(x => x.Id == Id);
                    if (policyList != null)
                    {
                        _dbContext.Remove(policyList);
                        _dbContext.SaveChanges();

                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Message = "Policy Deleted Successfully";
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

        public FileResult GetPolicyReport(int? Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            string DefaultPolicy = "Files\\DefaultPolicyDocument\\HRPolicy0.pdf";
            byte[] FileBytes = System.IO.File.ReadAllBytes(Path.Combine(_commonHelper.GetPhysicalRootPath(false), DefaultPolicy));
            try
            {
                if (Id > 0)
                {
                    var policyList = _dbRepo.PolicyList().FirstOrDefault(x => x.Id == Id);

                    string ReportURL = policyList.PolicyDocumentName;

                    FileBytes = System.IO.File.ReadAllBytes(Path.Combine(_commonHelper.GetPhysicalRootPath(false), ReportURL));
                }
                else
                {
                    commonResponse.StatusCode = HttpStatusCode.NotFound;
                    commonResponse.Message = "Data Not Found";

                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Data = ex;
            }
            return File(FileBytes, "application/pdf");

        }

    }
}
