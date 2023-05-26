using System.Net;
using System.Security.AccessControl;
using ArcheOne.Database.Entities;
using ArcheOne.Helper;
using ArcheOne.Models.Req;
using Azure;
using Microsoft.AspNetCore.Mvc;

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
			return View();
		}

		[HttpPost("User")]
		public IActionResult User(UserModel userModel)
		{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				UserMst userMst = new UserMst();
				var user = _dbRepo.UserMstList().Where(x => (x.FirstName != userModel.FirstName || x.MiddleName != userModel.MiddleName) && x.UserName != userModel.UserName).FirstOrDefault();
				if (user != null)
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
							userMst.CreatedDate = DateTime.Now;
							userMst.UpdatedDate = DateTime.Now;
							userMst.CreatedBy = LoggedInUserId;
							userMst.UpdatedBy = LoggedInUserId;

							var result = _dbContext.UserMsts.Add(userMst);
							_dbContext.SaveChanges();

							if (result != null)
							{
								commonResponse.Data = userMst;
								commonResponse.Message = "Photo uploaded Succesfully";
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
					commonResponse.Message = "Book Already Exists...!!!";
				}
			}
			catch (Exception ex)
			{
				commonResponse.Message = ex.Message;
			}
			return View();
		}

	}
}
