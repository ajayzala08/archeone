using System.Net;
using System.Security.Cryptography;
using System.Transactions;
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

		public async Task<IActionResult> AddEditUser(int Id)
		{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				UserAddEditReqViewModel userAddEditReqViewModel = new UserAddEditReqViewModel();
				userAddEditReqViewModel.UserDetails = new UserDetail();
				userAddEditReqViewModel.RoleList = _dbRepo.RoleMstList().ToList();
				if (Id > 0)
				{
					var userDetails = await _dbRepo.AllUserMstList().FirstOrDefaultAsync(x => x.Id == Id);
					if (userDetails != null)
					{
						userAddEditReqViewModel.UserDetails.Id = userDetails.Id;
						userAddEditReqViewModel.UserDetails.CompanyId = userDetails.CompanyId;
						userAddEditReqViewModel.UserDetails.FirstName = userDetails.FirstName;
						userAddEditReqViewModel.UserDetails.MiddleName = userDetails.MiddleName;
						userAddEditReqViewModel.UserDetails.LastName = userDetails.LastName;
						userAddEditReqViewModel.UserDetails.UserName = userDetails.UserName;
						userAddEditReqViewModel.UserDetails.Address = userDetails.Address;
						userAddEditReqViewModel.UserDetails.Pincode = userDetails.Pincode;
						userAddEditReqViewModel.UserDetails.Mobile1 = userDetails.Mobile1;
						userAddEditReqViewModel.UserDetails.Mobile2 = userDetails.Mobile2;
						userAddEditReqViewModel.UserDetails.Email = userDetails.Email;
						userAddEditReqViewModel.UserDetails.PhotoUrl = userDetails.PhotoUrl;
						userAddEditReqViewModel.UserDetails.RoleId = userDetails.RoleId.Value;
						userAddEditReqViewModel.UserDetails.IsActive = userDetails.IsActive.Value;
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
		public async Task<IActionResult> SaveUpdateUser(UserSaveUpdateReqModel userSaveUpdateReq)
		{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				if (ModelState.IsValid)
				{
					string file1 = string.Empty;
					var file1Slice = file1.Split("\\");
					UserMst userMst = new UserMst();
					IFormFile file = userSaveUpdateReq.PhotoUrl;
					string fileName = file.FileName;
					FileInfo fileInfo = new FileInfo(fileName);
					string fileExtension = fileInfo.Extension;
					long fileSize = file.Length;
					bool validateFileExtension = false;
					bool validateFileSize = false;
					string[] allowedFileExtensions = { CommonConstant.jpeg, CommonConstant.png, CommonConstant.jpg };
					long allowedFileSize = 1 * 1024 * 1024 * 10; // 10MB
					validateFileExtension = allowedFileExtensions.Contains(fileExtension) ? true : false;
					validateFileSize = fileSize <= allowedFileSize ? true : false;
					if (validateFileExtension && validateFileSize)
					{
						var duplicateCheck = await _dbRepo.AllUserMstList().Where(x => x.UserName == userSaveUpdateReq.UserName && x.Email == userSaveUpdateReq.Email && x.Mobile1 == userSaveUpdateReq.Mobile1).ToListAsync();
						if (duplicateCheck.Count == 0)
						{
							var imageFile = _commonHelper.UploadFile(userSaveUpdateReq.PhotoUrl, @"UserProfile", fileName, false, true, true);
							//string a = imageFile.Split();
							//string filePath = Path.Combine(_commonHelper.GetPhysicalRootPath(false), imageFile.Data);
							var userDetail = await _dbRepo.AllUserMstList().FirstOrDefaultAsync(x => x.Id == userSaveUpdateReq.Id);
							if (userDetail != null && userDetail.Id > 0)
							{
								//Edit Mode
								userDetail.RoleId = userSaveUpdateReq.RoleId;
								userDetail.CompanyId = 1;
								userDetail.FirstName = userSaveUpdateReq.FirstName;
								userDetail.MiddleName = userSaveUpdateReq.MiddleName;
								userDetail.LastName = userSaveUpdateReq.LastName;
								userDetail.UserName = userSaveUpdateReq.UserName;
								userDetail.Password = userSaveUpdateReq.Password;
								userDetail.Address = userSaveUpdateReq.Address;
								userDetail.Pincode = userSaveUpdateReq.Pincode;
								userDetail.Mobile1 = userSaveUpdateReq.Mobile1;
								userDetail.Mobile2 = userSaveUpdateReq.Mobile2;
								userDetail.Email = userSaveUpdateReq.Email;
								userDetail.PhotoUrl = imageFile.Data;
								userDetail.UpdatedDate = _commonHelper.GetCurrentDateTime();
								userDetail.UpdatedBy = 1;

								_dbContext.Entry(userDetail).State = EntityState.Modified;
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
				commonResponse.Data = ex;
			}
			return Json(commonResponse);
		}

		public async Task<IActionResult> UserList()
		{
			CommonResponse commonResponse = new CommonResponse();
			var userList = (from U in await _dbRepo.AllUserMstList().ToListAsync()
							join C in  _dbRepo.CompanyMstList()
											   on U.CompanyId equals C.Id
							join R in  _dbRepo.RoleMstList()
							on U.RoleId equals R.Id
							select new { U, C, R })
							   .Select(x => new UserListModel
							   {
								   Id = x.U.Id,
								   CompanyId = x.C.CompanyName,
								   RoleId = x.R.RoleName,
								   FullName = x.U.FirstName + ' ' + x.U.MiddleName + ' ' + x.U.LastName,
								   //FirstName = x.U.FirstName,
								   //MiddleName = x.U.MiddleName,
								   //LastName = x.U.LastName,
								   UserName = x.U.UserName,
								   Password = x.U.Password,
								   Address = x.U.Address,
								   Pincode = x.U.Pincode,
								   Mobile1 = x.U.Mobile1,
								   Mobile2 = x.U.Mobile2,
								   Email = x.U.Email,
								   PhotoUrl = x.U.PhotoUrl
							   }).ToList();
			if (userList.Count > 0)
			{
				commonResponse.Status = true;
				commonResponse.StatusCode = HttpStatusCode.OK;
				commonResponse.Message = "Users found successfully!";
				commonResponse.Data = userList;
			}
			else
			{
				commonResponse.StatusCode = HttpStatusCode.NotFound;
				commonResponse.Message = "User not found!";
			}
			return View(commonResponse);
		}

		public async Task<IActionResult> DeleteUser(int id)
		{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				var isUserExist = await _dbRepo.AllUserMstList().FirstOrDefaultAsync(x => x.Id == id);
				if (isUserExist != null)
				{
					isUserExist.IsDelete = true;
					isUserExist.UpdatedBy = _commonHelper.GetLoggedInUserId();
					isUserExist.UpdatedDate = _commonHelper.GetCurrentDateTime();

					_dbContext.Entry(isUserExist).State = EntityState.Modified;
					_dbContext.SaveChanges();

					commonResponse.Status = true;
					commonResponse.Message = "User Deleted Successfully!";
					commonResponse.StatusCode = HttpStatusCode.OK;
				}
				else
				{
					commonResponse.Message = "User not found!";
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

        public async Task<IActionResult> UserListByRoleId(int RoleId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var data = await _dbRepo.UserMstList().Where(x => x.RoleId == RoleId).ToListAsync();
                if (data != null && data.Count > 0)
                {
                    response.Data = data;
                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Data found successfully!";
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
    }
}
