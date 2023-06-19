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
            return View();
        }

        public IActionResult PolicyList()
        {
            CommonResponse commonResponse = new CommonResponse();
            PolicyMst policyMst = new PolicyMst();
            var policyList = _dbRepo.PolicyList().ToList();
            try
            {
                if (policyList.Count > 0)
                {
                    List<GetPolicyListResModel> getPolicyListResModel = new List<GetPolicyListResModel>();
                    getPolicyListResModel = _dbRepo.PolicyList().Where(x => x.IsActive == true && x.IsDelete == false).Select(x => new GetPolicyListResModel
                    {
                        Id = x.Id,
                        PolicyName = x.PolicyName,
                        PolicyDocument = x.PolicyDocumentName

                    }).ToList();
                    commonResponse.Data = getPolicyListResModel;


                    commonResponse.Status = true;
                    commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
                    commonResponse.Message = "GetAll HolidayList Successfully";
                }
                else
                {
                    commonResponse.Message = "No Data Found";
                    commonResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                commonResponse.Data = ex.Message;
                commonResponse.Status = false;
            }
            return View(commonResponse.Data);
        }

        public IActionResult AddEditPolicy(int Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            AddEditPolicyReqModel addEditPolicyReqModel = new AddEditPolicyReqModel();
            PolicyMst policyMst = new PolicyMst();

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
                    commonResponse.Message = "GetAll HolidayList Successfully";
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
                        var policyFile = _commonHelper.UploadFile(policySaveUpdateReqModel.PolicyDocumentName, @"PolicyDocument", fileName, false, true, false);
                        filePath = policyFile.Data;

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
                    policyDetail.UpdatedBy = _commonHelper.GetLoggedInUserId(); ;

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
                        policyMst.CreatedBy = 1;
                        policyMst.UpdatedBy = 1;
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
                        commonResponse.Message = "Holiday Deleted Successfully";
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
