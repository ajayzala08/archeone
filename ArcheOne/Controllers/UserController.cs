using System.Net;
using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace ArcheOne.Controllers
{
	public class UserController : Controller
	{
		private readonly DbRepo _dbRepo;
		private readonly CommonHelper _commonHelper;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly ArcheOneDbContext _dbContext;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostEnvironment;
        public UserController(DbRepo dbRepo, CommonHelper commonHelper, IWebHostEnvironment webHostEnvironment, ArcheOneDbContext dbContext, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
		{
			_dbRepo = dbRepo;
			_commonHelper = commonHelper;
			_webHostEnvironment = webHostEnvironment;
			_dbContext = dbContext;
			_hostEnvironment = hostEnvironment;
		}
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult User()
		{
			//List<CompanyMst> companies = new List<CompanyMst>();
			//companies.Add(new CompanyMst { Id = 0, CompanyName = "---Select---" });
			//var companyList = _dbRepo.CompanyMstList().Select(x => new CompanyMst { Id = x.Id, CompanyName = x.CompanyName }).ToList();
			//companies.AddRange(companyList);
			//ViewBag.Company = companies;

			//List<RoleMst> roles = new List<RoleMst>();
			//roles.Add(new RoleMst { Id = 0, RoleName = "---Select---" });
			//var roleList = _dbRepo.RoleMstList().Select(x => new RoleMst { Id = x.Id, RoleName = x.RoleName }).ToList();
			//roles.AddRange(roleList);
			//ViewBag.Role = roles;

			return View();
		}

		//[HttpPost]
		//public JsonResult AddUser([FromBody]AddUserModel addUserModel)
		//{
		//	CommonResponse commonResponse = new CommonResponse();
		//	try
		//	{
		//		IFormFile file = addUserModel.PhotoUrl;

		//		//string FileName = file.FileName;
		//		//FileInfo fileInfo = new FileInfo(FileName);
		//		//string FileExtension = fileInfo.Extension;
		//		string FileName = addUserModel.PhotoUrl.ToString();
		//		string FileExtension = addUserModel.PhotoUrl.ToString();
		//		long fileSize = file.Length;
		//		bool validateFileExtension = false;
		//		bool validateFileSize = false;
		//		string[] allowedFileExtensions = { (CommonConstant.Jpeg), (CommonConstant.png) };
		//		long allowedFileSize = 1 * 1024 * 1024 * 10; // 10MB
		//		validateFileExtension = allowedFileExtensions.Contains(FileExtension) ? true : false;
		//		validateFileSize = fileSize <= allowedFileSize ? true : false;
		//		int LoggedInUserId = _commonHelper.GetLoggedInUserId();
		//		if (validateFileExtension && validateFileSize)
		//		{
		//			bool IsFileCategoryDuplicate = _dbRepo.UserMstList().FirstOrDefault(x => x.Email.ToLower() == addUserModel.Email.ToLower() && x.Mobile1 == addUserModel.Mobile1 && x.Mobile2 == addUserModel.Mobile2) != null ? true : false;
		//			if (!IsFileCategoryDuplicate)
		//			{
		//				var FileCategory = _commonHelper.UploadFile(addUserModel.PhotoUrl, @"Files\ProfilePhoto", FileName);
		//				UserMst userMst = new UserMst();
		//				userMst.PhotoUrl = FileCategory.Data;
		//				userMst.CompanyId = addUserModel.CompanyId;
		//				userMst.FirstName = addUserModel.FirstName;
		//				userMst.MiddleName = addUserModel.MiddleName;
		//				userMst.LastName = addUserModel.LastName;
		//				userMst.UserName = addUserModel.UserName;
		//				userMst.Password = addUserModel.Password;
		//				userMst.Address = addUserModel.Address;
		//				userMst.Pincode = addUserModel.Pincode;
		//				userMst.Mobile1 = addUserModel.Mobile1;
		//				userMst.Mobile2 = addUserModel.Mobile2;
		//				userMst.Email = addUserModel.Email;
		//				userMst.IsActive = true;
		//				userMst.IsDelete = false;
		//				userMst.CreatedDate = _commonHelper.GetCurrentDateTime();
		//				userMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
		//				userMst.CreatedBy = LoggedInUserId;
		//				userMst.UpdatedBy = LoggedInUserId;

		//				_dbContext.UserMsts.Add(userMst);
		//				_dbContext.SaveChanges();

		//				commonResponse.Data = userMst;
		//				commonResponse.Message = "Data Uploaded Successfully";
		//				commonResponse.Status = true;
		//			}
		//			else
		//			{
		//				commonResponse.Message = "File Category Name Already Exists for Selected Organization, Company and Department !";
		//			}

		//		}
		//		else
		//		{
		//			commonResponse.Message = "Only png and jpeg file With Max Size : 10 MB is Allowed !";
		//		}

		//	}
		//	catch (Exception ex)
		//	{
		//		commonResponse.Message = ex.Message;
		//	}
		//	return Json(true);
		//}

		public IActionResult AddEditUser(int Id)
		{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				UserAddEditReqViewModel userAddEditReqViewModel = new UserAddEditReqViewModel();
				userAddEditReqViewModel.UserDetails = new UserDetail();
				userAddEditReqViewModel.CompanyList = _dbRepo.CompanyMstList().ToList();
				userAddEditReqViewModel.RoleList = _dbRepo.RoleMstList().ToList();
				if (Id > 0)
				{
					var UserList = _dbRepo.AllUserMstList();
					var UserDetails = _dbRepo.AllUserMstList().FirstOrDefault(x => x.Id == Id);
					if (UserDetails != null)
					{
						string relativePath = _commonHelper.GetRelativeRootPath();
						userAddEditReqViewModel.UserDetails.Id = UserDetails.Id;
						userAddEditReqViewModel.UserDetails.CompanyId = UserDetails.CompanyId;
						userAddEditReqViewModel.UserDetails.FirstName = UserDetails.FirstName;
						userAddEditReqViewModel.UserDetails.MiddleName = UserDetails.MiddleName;
						userAddEditReqViewModel.UserDetails.LastName = UserDetails.LastName;
						userAddEditReqViewModel.UserDetails.UserName = UserDetails.UserName;
						userAddEditReqViewModel.UserDetails.Password  = UserDetails.Password;
						userAddEditReqViewModel.UserDetails.Address = UserDetails.Address;
						userAddEditReqViewModel.UserDetails.Pincode = UserDetails.Pincode;
						userAddEditReqViewModel.UserDetails.Mobile1 = UserDetails.Mobile1;
						userAddEditReqViewModel.UserDetails.Mobile2 = UserDetails.Mobile2;
						userAddEditReqViewModel.UserDetails.Email = UserDetails.Email;
						userAddEditReqViewModel.UserDetails.PhotoUrl = relativePath + UserDetails.PhotoUrl;
						userAddEditReqViewModel.UserDetails.RoleId = UserDetails.RoleId;
					}
				}
				commonResponse.Status = true;
				commonResponse.StatusCode = HttpStatusCode.OK;
				commonResponse.Message = "Success!";
				commonResponse.Data = userAddEditReqViewModel;

			}
			catch { throw; }
			return View(commonResponse.Data);
		}

        public CommonResponse SaveUpdateUser(UserMst user)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                UserMst userMst = new UserMst();
                bool IsDuplicate = false;
                var duplicateCheck = _dbRepo.AllUserMstList().FirstOrDefault(x => x.Id != user.Id && x.UserName != user.UserName);
                IsDuplicate = duplicateCheck != null;
                if (!IsDuplicate)
                {
                    var UserDetail = _dbRepo.AllUserMstList().FirstOrDefault(x => x.Id == user.Id);
                    if (UserDetail != null && UserDetail.Id > 0)
                    {
                        //Edit Mode
                        userMst = user;
                        userMst.CreatedDate = UserDetail.CreatedDate;
                        userMst.CreatedBy = UserDetail.CreatedBy;
                        userMst.IsActive = UserDetail.IsActive;
                        userMst.IsDelete = UserDetail.IsDelete;

                        userMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
                        userMst.UpdatedBy = 1;

                        _dbContext.Entry(userMst).State = EntityState.Modified;
                        _dbContext.SaveChanges();

                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            var ImagePath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "Files","UserImages", "logo.png");
                            var emailTemplatePath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "Files", "Files.html");
                            StreamReader str = new StreamReader(emailTemplatePath);
                            string MailText = str.ReadToEnd();
                            str.Close();
                            var htmlBody = MailText;
                            htmlBody = htmlBody.Replace("logo.png", ImagePath);
                            SendEmailRequestModel sendEmailRequestModel = new SendEmailRequestModel();
                            sendEmailRequestModel.ToEmail = user.Email;
                            sendEmailRequestModel.Subject = "Welcome To Reyna";
                            sendEmailRequestModel.Body = htmlBody;
                            var sentmail = _commonHelper.SendEmail(sendEmailRequestModel);

                            if (sentmail.Status)
                            {
                                userMst = user;
                                userMst.CreatedDate = UserDetail.CreatedDate;
                                userMst.CreatedBy = UserDetail.CreatedBy;
                                userMst.IsActive = UserDetail.IsActive;
                                userMst.IsDelete = UserDetail.IsDelete;
                                //userMst.WelcomeEmailSend = true;
                                userMst.IsActive = true;

								_dbContext.Add(userMst);
								_dbContext.SaveChanges();
                            }
                        }

                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Message = "User Updated Successfully!";
                    }
                    else
                    {
                        //Add Mode
                        userMst = user;
                        userMst.CreatedDate = _commonHelper.GetCurrentDateTime();
                        userMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
                        userMst.CreatedBy = 1;
                        userMst.UpdatedBy = 1;
                        userMst.IsActive = true;
                        userMst.IsDelete = false;
                        _dbContext.Add(userMst);
                        _dbContext.SaveChanges();

                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Message = "User Added Successfully!";
                    }
                }
                else
                {
                    commonResponse.Message = "User Name Already Exist";
                }
                commonResponse.Data = userMst;
            }
            catch { throw; }
            return commonResponse;
        }

        public IActionResult UserList()
		{
			return View(_dbRepo.UserMstList().ToList());
		}

		public IActionResult DeleteUser(int id)
		{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				//var res = _dbRepo.UserMstList().FirstOrDefault(x => x.Id == id);
				//if (res != null)
				//{
				//	UserModel user = new UserModel();
				//	//user.IsDelete = true;
				//	//user.UpdatedBy = _commonHelper.GetLoggedInUserId();
				//	//user.CreatedDate = _commonHelper.GetCurrentDateTime();
				//	//user.UpdatedDate = _commonHelper.GetCurrentDateTime();

				//	_dbContext.Entry(user).State = EntityState.Modified;
				//	_dbContext.SaveChanges();
				//}
				//else
				//{
				//	commonResponse.Message = "Data not found!";
				//	commonResponse.StatusCode = HttpStatusCode.NotFound;
				//}
			}
			catch { throw; }
			return RedirectToAction("UserList");
		}

		[HttpPost]
		public async Task<ActionResult> UploadFiles(IList<IFormFile> files)
		{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				string fileName = null;
				string filepath12 = null;
				foreach (IFormFile source in files)
				{
					var Extensionfile = Path.GetExtension(source.FileName).ToLower();
					if (Extensionfile == ".png" || Extensionfile == ".jpeg" || Extensionfile == ".jpg")
					{
						fileName = DateTime.Now.ToFileTime() + Path.GetExtension(source.FileName);
						commonResponse.Data = _commonHelper.UploadFile(source, "OrganizationLogo", fileName);

						commonResponse.Status = true;
						commonResponse.StatusCode = HttpStatusCode.OK;
						commonResponse.Message = "File Upload Successfully!";
					}
					else
					{
						commonResponse.Message = "Please Select Only png,jpeg,jpg files!";
					}
				}
			}
			catch { throw; }
			return Json(commonResponse);
		}
	}
}
