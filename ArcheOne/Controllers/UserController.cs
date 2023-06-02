using System.Net;
using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ArcheOne.Controllers
{
	public class UserController : Controller
	{
		private readonly DbRepo _dbRepo;
		private readonly CommonHelper _commonHelper;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly ArcheOneDbContext _dbContext;
		public UserController(DbRepo dbRepo, CommonHelper commonHelper, IWebHostEnvironment webHostEnvironment, ArcheOneDbContext dbContext)
		{
			_dbRepo = dbRepo;
			_commonHelper = commonHelper;
			_webHostEnvironment = webHostEnvironment;
			_dbContext = dbContext;
		}
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult User()
		{
			List<CompanyMst> companies = new List<CompanyMst>();
			companies.Add(new CompanyMst { Id = 0, CompanyName = "---Select---" });
			var companyList = _dbRepo.CompanyMstList().Select(x => new CompanyMst { Id = x.Id, CompanyName = x.CompanyName }).ToList();
			companies.AddRange(companyList);
			ViewBag.Company = companies;

			List<RoleMst> roles = new List<RoleMst>();
			roles.Add(new RoleMst { Id = 0, RoleName = "---Select---" });
			var roleList = _dbRepo.RoleMstList().Select(x => new RoleMst { Id = x.Id, RoleName = x.RoleName }).ToList();
			roles.AddRange(roleList);
			ViewBag.Role = roles;

			return View();
		}

		[HttpPost]
		public IActionResult AddUser([FromForm]UserModel userModel)
		{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				IFormFile file = userModel.PhotoUrl;

				//string FileName = file.FileName;
				//FileInfo fileInfo = new FileInfo(FileName);
				//string FileExtension = fileInfo.Extension;
				string FileName = userModel.PhotoUrl.ToString();
				string FileExtension = userModel.PhotoUrl.ToString();
				long fileSize = file.Length;
				bool validateFileExtension = false;
				bool validateFileSize = false;
				string[] allowedFileExtensions = { (CommonConstant.Jpeg), (CommonConstant.png) };
				long allowedFileSize = 1 * 1024 * 1024 * 10; // 10MB
				validateFileExtension = allowedFileExtensions.Contains(FileExtension) ? true : false;
				validateFileSize = fileSize <= allowedFileSize ? true : false;
				int LoggedInUserId = _commonHelper.GetLoggedInUserId();
				if (validateFileExtension && validateFileSize)
				{
					bool IsFileCategoryDuplicate = _dbRepo.UserMstList().FirstOrDefault(x => x.Email.ToLower() == userModel.Email.ToLower() && x.Mobile1 == userModel.Mobile1 && x.Mobile2 == userModel.Mobile2) != null ? true : false;
					if (!IsFileCategoryDuplicate)
					{
						var FileCategory = _commonHelper.UploadFile(userModel.PhotoUrl, @"Files\ProfilePhoto", FileName);
						UserMst userMst = new UserMst();
						userMst.PhotoUrl = FileCategory.Data;
						userMst.CompanyId = userModel.CompanyId;
						userMst.FirstName = userModel.FirstName;
						userMst.MiddleName = userModel.MiddleName;
						userMst.LastName = userModel.LastName;
						userMst.UserName = userModel.UserName;
						userMst.Password = userModel.Password;
						userMst.Address = userModel.Address;
						userMst.Pincode = userModel.Pincode;
						userMst.Mobile1 = userModel.Mobile1;
						userMst.Mobile2 = userModel.Mobile2;
						userMst.Email = userModel.Email;
						userMst.IsActive = true;
						userMst.IsDelete = false;
						userMst.CreatedDate = _commonHelper.GetCurrentDateTime();
						userMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
						userMst.CreatedBy = LoggedInUserId;
						userMst.UpdatedBy = LoggedInUserId;

						_dbContext.UserMsts.Add(userMst);
						_dbContext.SaveChanges();

						commonResponse.Data = userMst;
						commonResponse.Message = "Data Uploaded Successfully";
						commonResponse.Status = true;
					}
					else
					{
						commonResponse.Message = "File Category Name Already Exists for Selected Organization, Company and Department !";
					}

				}
				else
				{
					commonResponse.Message = "Only png and jpeg file With Max Size : 10 MB is Allowed !";
				}

			}
			catch (Exception ex)
			{
				commonResponse.Message = ex.Message;
			}
			return View();
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
				var res = _dbRepo.UserMstList().FirstOrDefault(x => x.Id == id);
				if (res != null)
				{
					UserModel user = new UserModel();
					//user.IsDelete = true;
					//user.UpdatedBy = _commonHelper.GetLoggedInUserId();
					//user.CreatedDate = _commonHelper.GetCurrentDateTime();
					//user.UpdatedDate = _commonHelper.GetCurrentDateTime();

					_dbContext.Entry(user).State = EntityState.Modified;
					_dbContext.SaveChanges();
				}
				else
				{
					commonResponse.Message = "Data not found!";
					commonResponse.StatusCode = HttpStatusCode.NotFound;
				}
			}
			catch { throw; }
			return RedirectToAction("UserList");
		}
	}
}
