using System.Net;
using System.Transactions;
using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
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
				userAddEditReqViewModel.RoleList = _dbRepo.RoleMstList().ToList();
				if (Id > 0)
				{
					var UserDetails = _dbRepo.AllUserMstList().FirstOrDefault(x => x.Id == Id);
					if (UserDetails != null)
					{
						userAddEditReqViewModel.UserDetails.Id = UserDetails.Id;
						userAddEditReqViewModel.UserDetails.CompanyId = UserDetails.CompanyId;
						userAddEditReqViewModel.UserDetails.FirstName = UserDetails.FirstName;
						userAddEditReqViewModel.UserDetails.MiddleName = UserDetails.MiddleName;
						userAddEditReqViewModel.UserDetails.LastName = UserDetails.LastName;
						userAddEditReqViewModel.UserDetails.Address = UserDetails.Address;
						userAddEditReqViewModel.UserDetails.Pincode = UserDetails.Pincode;
						userAddEditReqViewModel.UserDetails.Mobile1 = UserDetails.Mobile1;
						userAddEditReqViewModel.UserDetails.Mobile2 = UserDetails.Mobile2;
						userAddEditReqViewModel.UserDetails.Email = UserDetails.Email;
						userAddEditReqViewModel.UserDetails.PhotoUrl = UserDetails.PhotoUrl;
						userAddEditReqViewModel.UserDetails.RoleId = UserDetails.RoleId.Value;
						userAddEditReqViewModel.UserDetails.IsActive = UserDetails.IsActive.Value;
					}
				}
				commonResponse.Status = true;
				commonResponse.StatusCode = HttpStatusCode.OK;
				commonResponse.Message = "Success!";
				commonResponse.Data = userAddEditReqViewModel;
			}
			catch (Exception ex)
			{
				commonResponse.Message = ex.Message;
				commonResponse.Data = ex;
			}
			return View(commonResponse.Data);
		}

		[HttpPost]
		public IActionResult SaveUpdateUser(UserSaveUpdateReqModel userSaveUpdateReq)
		{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				if (ModelState.IsValid)
				{
					UserMst userMst = new UserMst();
					IFormFile file = userSaveUpdateReq.PhotoUrl;
					string FileName = file.FileName;
					FileInfo fileInfo = new FileInfo(FileName);
					string FileExtension = fileInfo.Extension;
					long fileSize = file.Length;
					bool validateFileExtension = false;
					bool validateFileSize = false;
					string[] allowedFileExtensions = { (CommonConstant.jpg), (CommonConstant.png), (CommonConstant.jpeg) };
					long allowedFileSize = 1 * 1024 * 1024 * 10; // 10MB
					validateFileExtension = allowedFileExtensions.Contains(FileExtension) ? true : false;
					validateFileSize = fileSize <= allowedFileSize ? true : false;
					if (validateFileExtension && validateFileSize)
					{
						var duplicateCheck = _dbRepo.AllUserMstList().Where(x => x.UserName == userSaveUpdateReq.UserName && x.Email == userSaveUpdateReq.Email && x.Mobile1 == userSaveUpdateReq.Mobile1).ToList();
						if (duplicateCheck.Count == 0)
						{
							var imageFile = _commonHelper.UploadFile(userSaveUpdateReq.PhotoUrl, @"UserProfile", FileName, false, true, true);
							//string filePath = Path.Combine(_commonHelper.GetPhysicalRootPath(false), imageFile.Data);
							var UserDetail = _dbRepo.AllUserMstList().FirstOrDefault(x => x.Id == userSaveUpdateReq.Id);
							if (UserDetail != null && UserDetail.Id > 0)
							{
								//Edit Mode
								UserDetail.RoleId = userSaveUpdateReq.RoleId;
								UserDetail.CompanyId = 1;
								UserDetail.FirstName = userSaveUpdateReq.FirstName;
								UserDetail.MiddleName = userSaveUpdateReq.MiddleName;
								UserDetail.LastName = userSaveUpdateReq.LastName;
								UserDetail.UserName = userSaveUpdateReq.UserName;
								UserDetail.Password = userSaveUpdateReq.Password;
								UserDetail.Address = userSaveUpdateReq.Address;
								UserDetail.Pincode = userSaveUpdateReq.Pincode;
								UserDetail.Mobile1 = userSaveUpdateReq.Mobile1;
								UserDetail.Mobile2 = userSaveUpdateReq.Mobile2;
								UserDetail.Email = userSaveUpdateReq.Email;
								UserDetail.PhotoUrl = imageFile.Data;
								UserDetail.UpdatedDate = _commonHelper.GetCurrentDateTime();
								UserDetail.UpdatedBy = 1;

								_dbContext.Entry(UserDetail).State = EntityState.Modified;
								_dbContext.SaveChanges();

								commonResponse.Status = true;
								commonResponse.StatusCode = HttpStatusCode.OK;
								commonResponse.Message = "User Updated Successfully!";
							}
							else
							{
								//Add Mode
								userMst.RoleId = userSaveUpdateReq.RoleId;
								userMst.CompanyId = 1;
								userMst.FirstName = userSaveUpdateReq.FirstName;
								userMst.MiddleName = userSaveUpdateReq.MiddleName;
								userMst.LastName = userSaveUpdateReq.LastName;
								userMst.UserName = userSaveUpdateReq.UserName;
								userMst.Password = userSaveUpdateReq.Password;
								userMst.Address = userSaveUpdateReq.Address;
								userMst.Pincode = userSaveUpdateReq.Pincode;
								userMst.Mobile1 = userSaveUpdateReq.Mobile1;
								userMst.Mobile2 = userSaveUpdateReq.Mobile2;
								userMst.Email = userSaveUpdateReq.Email;
								userMst.PhotoUrl = imageFile.Data;
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
							commonResponse.Message = "UserName, Email and Contact Already Exist";
						}
						commonResponse.Data = userMst;
					}
					else
					{
						commonResponse.Message = "Only jpg and png files are Allowed !";
					}

				}

			}
			catch (Exception ex)
			{
				commonResponse.Message = ex.Message;
				commonResponse.Data = ex.StackTrace;
			}
			return Json(commonResponse);
		}

		public IActionResult UserList()
		{
			CommonResponse commonResponse = new CommonResponse();
			var userMst = (from U in _dbRepo.AllUserMstList().ToList()
						   join C in _dbRepo.CompanyMstList()
											  on U.CompanyId equals C.Id
						   join R in _dbRepo.RoleMstList()
						   on U.RoleId equals R.Id
						   select new { U, C, R })
							   .Select(x => new UserListModel
							   {
								   Id = x.U.Id,
								   CompanyId = x.C.CompanyName,
								   RoleId = x.R.RoleName,
								   FirstName = x.U.FirstName,
								   MiddleName = x.U.MiddleName,
								   LastName = x.U.LastName,
								   UserName = x.U.UserName,
								   Password = x.U.Password,
								   Address = x.U.Address,
								   Pincode = x.U.Pincode,
								   Mobile1 = x.U.Mobile1,
								   Mobile2 = x.U.Mobile2,
								   Email = x.U.Email,
								   PhotoUrl = x.U.PhotoUrl,
								   CreatedDate = x.C.UpdatedDate
							   }).OrderByDescending(x => x.CreatedDate).ToList();
			if (userMst.Count > 0)
			{
				commonResponse.Status = true;
				commonResponse.StatusCode = HttpStatusCode.OK;
				commonResponse.Message = "Data found successfully!";
				commonResponse.Data = userMst;
			}
			else
			{
				commonResponse.StatusCode = HttpStatusCode.NotFound;
				commonResponse.Message = "Data not found!";
			}
			return View(commonResponse);
		}

		public IActionResult DeleteUser(int id)
		{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				var res = _dbRepo.AllUserMstList().FirstOrDefault(x => x.Id == id);
				if (res != null)
				{
					res.IsDelete = true;
					res.UpdatedBy = _commonHelper.GetLoggedInUserId();
					res.CreatedDate = _commonHelper.GetCurrentDateTime();
					res.UpdatedDate = _commonHelper.GetCurrentDateTime();

					_dbContext.Entry(res).State = EntityState.Modified;
					_dbContext.SaveChanges();

					commonResponse.Status = true;
					commonResponse.Message = "Data Deleted Successfully!";
					commonResponse.StatusCode = HttpStatusCode.NotFound;
				}
				else
				{
					commonResponse.Message = "Data not found!";
					commonResponse.StatusCode = HttpStatusCode.NotFound;
				}
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
