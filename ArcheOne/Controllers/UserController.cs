using System.Net;
using System.Security.AccessControl;
using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using Azure;
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
			List<SelectListItem> list = new List<SelectListItem>().ToList();
			List<CompanyReqModel> companyReqModel = new List<CompanyReqModel>();
			var companies = _dbRepo.CompanyMstList().Select(x => new SelectListItem { Text = x.CompanyName, Value = x.Id.ToString()}).ToList();
			ViewBag.Company = companies;

			List<RoleReqModel> roleReqModel = new List<RoleReqModel>();
			var roles = _dbRepo.RoleMstList().Select(x => new SelectListItem { Text = x.RoleName, Value = x.Id.ToString() }).ToList();
			 ViewBag.Role = roles;
			return View();
		}

		[HttpGet("User")]
		[Consumes("multipart/form-data")]
		public IActionResult User(UserModel userModel)
		{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				if (ModelState.IsValid)
				{
					UserMst userMst = new UserMst();
					var user = _dbRepo.UserMstList().FirstOrDefault(x => x.Email.ToLower() == userModel.Email.ToLower() && x.Mobile1 == userModel.Mobile1 && x.Mobile2 == userModel.Mobile2);
					if (user == null)
					{
						int LoggedInUserId = _commonHelper.GetLoggedInUserId();
						if (userModel.PhotoUrl.Length > 0)
						{
							var fileName = Path.GetFileName(userModel.PhotoUrl.FileName).ToLower();
							var fileExt = Path.GetExtension(userModel.PhotoUrl.FileName).ToLower();

							Guid guid = Guid.NewGuid();
							var file = guid + fileName; //Create a new Name for the file due to security reasons.

							string path = Path.Combine(this._webHostEnvironment.WebRootPath, "Images");
							if (!Directory.Exists(path))
							{
								Directory.CreateDirectory(path);
							}
							var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\", fileName);
							if (fileExt != ".jpg" && fileExt != ".png")
							{
								commonResponse.Status = false;
								commonResponse.Message = "Image does not support,Plz upload in jpg and png format..";
							}
							else
							{
								var pathBuilt1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\", fileName);

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
								userMst.PhotoUrl = fileName;
								userMst.IsActive = true;
								userMst.IsDelete = false;
								userMst.CreatedDate = _commonHelper.GetCurrentDateTime();
								userMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
								userMst.CreatedBy = LoggedInUserId;
								userMst.UpdatedBy = LoggedInUserId;

								var result = _dbContext.UserMsts.Add(userMst);
								_dbContext.SaveChanges();

								if (result != null)
								{
									commonResponse.Data = userMst;
									commonResponse.Message = "Data Uploaded Successfully";
									commonResponse.Status = true;
								}

								using (var fileSrteam = new FileStream(pathBuilt1, FileMode.Create))
								{
									userModel.PhotoUrl.CopyTo(fileSrteam);
								}
							}
						}
						else
						{
							commonResponse.Status = false;
							commonResponse.Message = "Photo does not exist !";
						}
					}
					else
					{
						commonResponse.Status = false;
						commonResponse.Message = "Email or MobileNumber already exists !";
					}
					ViewBag.Message = commonResponse.Message;
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
					user.IsDelete = true;
					user.UpdatedBy = _commonHelper.GetLoggedInUserId();
					user.CreatedDate = _commonHelper.GetCurrentDateTime();
					user.UpdatedDate = _commonHelper.GetCurrentDateTime();

					_dbContext.Entry(user).State = EntityState.Modified;
					_dbContext.SaveChanges();
				}
				else
				{
					commonResponse.Message = "Data not found !";
					commonResponse.StatusCode = HttpStatusCode.BadRequest;
				}
			}
			catch { throw; }
			return RedirectToAction("UserList");
		}

		//#region Company 
		//public IActionResult Company()
		//{
		//	CommonResponse commonResponse = new CommonResponse();
		//	var companies = _dbRepo.CompanyMstList().ToList();
		//	if (companies.Count > 0)
		//	{
		//		commonResponse.Data = companies;
		//		commonResponse.Status = true;
		//		commonResponse.StatusCode = HttpStatusCode.OK;
		//		commonResponse.Message = "Data found successfully !";
		//	}
		//	else
		//	{
		//		commonResponse.Message = "Data not found !";
		//		commonResponse.StatusCode = HttpStatusCode.NotFound;
		//	}
		//	ViewBag.Company = commonResponse.Data;
		//	return Json(companies);
		//}
		//#endregion
	}
}
