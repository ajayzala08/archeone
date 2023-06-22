using System.Net;
using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArcheOne.Controllers
{
	public class UserDocumentController : Controller
	{
        private readonly DbRepo _dbRepo;
        private readonly CommonHelper _commonHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ArcheOneDbContext _dbContext;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostEnvironment;
        public UserDocumentController(DbRepo dbRepo, CommonHelper commonHelper, IWebHostEnvironment webHostEnvironment, ArcheOneDbContext dbContext, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> UserDocument(int Id)
		{
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                AddEditUserDocumentReqModel addEditUserDocumentReqModel = new AddEditUserDocumentReqModel();
                addEditUserDocumentReqModel.AddEditUserDocuments = new AddEditUserDocument();
                addEditUserDocumentReqModel.UserList = _dbRepo.AllUserMstList().ToList();
                addEditUserDocumentReqModel.DocumentTypeList = _dbRepo.DocumentTypeList().ToList();
                if (Id > 0)
                {
                    var userDocuments = await _dbRepo.UserDocumentList().FirstOrDefaultAsync(x => x.Id == Id);
                    if (userDocuments != null)
                    {
                        addEditUserDocumentReqModel.AddEditUserDocuments.Id = userDocuments.Id;
                        addEditUserDocumentReqModel.AddEditUserDocuments.UserId = userDocuments.UserId;
                        addEditUserDocumentReqModel.AddEditUserDocuments.DocumentTypeId = userDocuments.DocumentTypeId;
                        addEditUserDocumentReqModel.AddEditUserDocuments.Document = userDocuments.Document;
                        addEditUserDocumentReqModel.AddEditUserDocuments.IsActive = userDocuments.IsActive;
                    }
                }
                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "Success!";
                commonResponse.Data = addEditUserDocumentReqModel;
            }
            catch (Exception ex)
            {
                commonResponse.Data = ex.Message;
                commonResponse.Status = false;
            }
            return View();
		}

        public async Task<IActionResult> SaveUpdateUserDocument(SaveUpdateUserDocumentsReqModel saveUpdateUserDocumentsReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                IFormFile file;
                string fileName = string.Empty;
                bool validateFileExtension = false;
                bool validateFileSize = false;
                string filePath = string.Empty;
                UserDocumentMst userDocumentMst = new UserDocumentMst();
                if (saveUpdateUserDocumentsReqModel.Document != null)
                {
                    file = saveUpdateUserDocumentsReqModel.Document;
                    fileName = file.FileName;
                    FileInfo fileInfo = new FileInfo(fileName);
                    string fileExtension = fileInfo.Extension;
                    long fileSize = file.Length;

                    string[] allowedFileExtensions = { CommonConstant.pdf };
                    long allowedFileSize = 1 * 1024 * 1024 * 10; // 10MB
                    validateFileExtension = allowedFileExtensions.Contains(fileExtension) ? true : false;
                    validateFileSize = fileSize <= allowedFileSize ? true : false;
                    fileName = saveUpdateUserDocumentsReqModel.UserId + saveUpdateUserDocumentsReqModel.Id + fileExtension;
                    if (validateFileExtension && validateFileSize)
                    {
                        var imageFile = _commonHelper.UploadFile(saveUpdateUserDocumentsReqModel.Document, @"UserDocument", fileName, false, true, false);
                        filePath = imageFile.Data.RelativePath;
                    }
                    else
                    {
                        commonResponse.Message = "Only pdf files are Allowed !";
                    }
                }

                var userDocs = await _dbRepo.UserDocumentList().FirstOrDefaultAsync(x => x.Id == saveUpdateUserDocumentsReqModel.Id && x.UserId != saveUpdateUserDocumentsReqModel.UserId);
                if (userDocs != null && userDocs.Id > 0)
                {
                    //Edit Mode
                    userDocs.UserId = saveUpdateUserDocumentsReqModel.UserId;
                    userDocs.DocumentTypeId = saveUpdateUserDocumentsReqModel.DocumentTypeId;
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        userDocs.Document = filePath;
                    }
                    userDocs.UpdatedDate = _commonHelper.GetCurrentDateTime();
                    userDocs.UpdatedBy = _commonHelper.GetLoggedInUserId(); ;

                    _dbContext.Entry(userDocs).State = EntityState.Modified;
                    _dbContext.SaveChanges();

                    commonResponse.Status = true;
                    commonResponse.StatusCode = HttpStatusCode.OK;
                    commonResponse.Message = "UserDocument Updated Successfully!";
                }
                else
                {
                    //Add Mode
                    var duplicateCheck = await _dbRepo.UserDocumentList().Where(x => x.UserId == saveUpdateUserDocumentsReqModel.UserId).ToListAsync();
                    if (duplicateCheck.Count == 0)
                    {
                        userDocumentMst.UserId = saveUpdateUserDocumentsReqModel.UserId;
                        userDocumentMst.DocumentTypeId = saveUpdateUserDocumentsReqModel.DocumentTypeId;
                        userDocumentMst.Document = filePath;
                        userDocumentMst.CreatedDate = _commonHelper.GetCurrentDateTime();
                        userDocumentMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
                        userDocumentMst.CreatedBy = 1;
                        userDocumentMst.UpdatedBy = 1;
                        userDocumentMst.IsActive = true;
                        userDocumentMst.IsDelete = false;

                        _dbContext.Add(userDocumentMst);
                        _dbContext.SaveChanges();

                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Message = "UserDocument Added Successfully!";
                    }
                    else
                    {
                        commonResponse.Message = "User Already Exist";
                    }
                }
                commonResponse.Data = userDocumentMst;
            }
            catch(Exception ex)
            {
                commonResponse.Data = ex.Message;
                commonResponse.Status = false;
            }
            return View();
        }

        public async Task<IActionResult> UserDocumentList()
        {
            return View();
        }

    }
}
