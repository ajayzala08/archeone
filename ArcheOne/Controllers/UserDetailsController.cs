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
	public class UserDetailsController : Controller
	{
		private readonly CommonHelper _commonHelper;
		private readonly DbRepo _dbRepo;
		private readonly ArcheOneDbContext _dbContext;
		public UserDetailsController(CommonHelper commonHelper, DbRepo dbRepo, ArcheOneDbContext dbContext)
		{
			_commonHelper = commonHelper;
			_dbRepo = dbRepo;
			_dbContext = dbContext;
		}
		public async Task<IActionResult> UserDetails()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> AddEditUserDetails(int userId)
		{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				UserDetailsAddEditResModel userDetailsAddEditResModel = new UserDetailsAddEditResModel();
				userDetailsAddEditResModel.UserDetail = new UserDetails();
				userDetailsAddEditResModel.EmploymentTypeList = _dbRepo.EmploymentTypeList().ToList();


				var a = await new RoleController(_dbRepo).GetRoleByUserId(userId);
				if (a != null && a.Status && a.Data != null && a.Data.RoleCode == CommonEnums.RoleMst.Admin.ToString())
				{
					userDetailsAddEditResModel.ReportingManagerList = new List<UserMst> { new UserMst { Id = 0, FirstName = "NA" } };
				}
				else
				{
					userDetailsAddEditResModel.ReportingManagerList = await _dbRepo.UserMstList().Where(x => x.Id != userId && x.RoleId == 3).Select(x => new UserMst() { Id = x.Id, FirstName = x.FirstName, MiddleName = x.MiddleName, LastName = x.LastName }).ToListAsync();
				}



				userDetailsAddEditResModel.UserDetail.UserId = userId;
				var isUserDetailsExist = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == userId);
				if (isUserDetailsExist != null)
				{
					userDetailsAddEditResModel.UserDetail.Id = isUserDetailsExist.Id;
					userDetailsAddEditResModel.UserDetail.UserId = isUserDetailsExist.UserId;
					userDetailsAddEditResModel.UserDetail.EmployeeCode = isUserDetailsExist.EmployeeCode;
					userDetailsAddEditResModel.UserDetail.Gender = isUserDetailsExist.Gender;
					userDetailsAddEditResModel.UserDetail.EmergencyContact = isUserDetailsExist.EmergencyContact;
					userDetailsAddEditResModel.UserDetail.Dob = isUserDetailsExist.Dob;
					userDetailsAddEditResModel.UserDetail.PostCode = isUserDetailsExist.PostCode;
					userDetailsAddEditResModel.UserDetail.EmploymentType = isUserDetailsExist.EmploymentType;
					userDetailsAddEditResModel.UserDetail.Location = isUserDetailsExist.Location;
					userDetailsAddEditResModel.UserDetail.BloodGroup = isUserDetailsExist.BloodGroup;
					userDetailsAddEditResModel.UserDetail.OfferDate = isUserDetailsExist.OfferDate;
					userDetailsAddEditResModel.UserDetail.JoinDate = isUserDetailsExist.JoinDate;
					userDetailsAddEditResModel.UserDetail.BankName = isUserDetailsExist.BankName;
					userDetailsAddEditResModel.UserDetail.AccountNumber = isUserDetailsExist.AccountNumber;
					userDetailsAddEditResModel.UserDetail.Branch = isUserDetailsExist.Branch;
					userDetailsAddEditResModel.UserDetail.IfscCode = isUserDetailsExist.IfscCode;
					userDetailsAddEditResModel.UserDetail.PfaccountNumber = isUserDetailsExist.PfaccountNumber;
					userDetailsAddEditResModel.UserDetail.PancardNumber = isUserDetailsExist.PanCardNumber;
					userDetailsAddEditResModel.UserDetail.AdharCardNumber = isUserDetailsExist.AadharCardNumber;
					userDetailsAddEditResModel.UserDetail.Salary = Convert.ToDecimal(isUserDetailsExist.Salary.ToString("#.##"));
					userDetailsAddEditResModel.UserDetail.ReportingManager = isUserDetailsExist.ReportingManager;
					userDetailsAddEditResModel.UserDetail.Reason = isUserDetailsExist.Reason;
					userDetailsAddEditResModel.UserDetail.EmployeePersonalEmailId = isUserDetailsExist.EmployeePersonalEmailId;
					userDetailsAddEditResModel.UserDetail.ProbationPeriod = isUserDetailsExist.ProbationPeriod;
					userDetailsAddEditResModel.UserDetail.IsActive = isUserDetailsExist.IsActive;
				}
				else
				{
					DateTime todayDate = _commonHelper.GetCurrentDateTime();
					DateTime eighteenYearsAgo = todayDate.AddYears(-18);
					userDetailsAddEditResModel.UserDetail.OfferDate = todayDate;
					userDetailsAddEditResModel.UserDetail.JoinDate = todayDate;
					userDetailsAddEditResModel.UserDetail.Dob = eighteenYearsAgo;
				}
				commonResponse.Status = true;
				commonResponse.StatusCode = HttpStatusCode.OK;
				commonResponse.Message = "Success!";
				commonResponse.Data = userDetailsAddEditResModel;
			}
			catch (Exception ex)
			{
				commonResponse.Message = ex.Message;
				commonResponse.Data = ex;
			}
			return View(commonResponse.Data);
		}

		[HttpPost]
		public async Task<IActionResult> SaveUpdateUserDetails(AddEditUserDetailsReqModel addEditUserDetailsReqModel)
		{
			CommonResponse commonResponse = new CommonResponse();
			UserDetailsMst userDetailsMst = new UserDetailsMst();
			try
			{
				if (addEditUserDetailsReqModel.JoinDate.Date >= addEditUserDetailsReqModel.OfferDate.Date)
				{
					var isUserExist = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == addEditUserDetailsReqModel.UserId);
					if (isUserExist != null)
					{
						// Edit Mode
						//var editUserDetails = _dbRepo.UserDetailList().FirstOrDefault(x => x.Id == addEditUserDetailsReqModel.Id && x.EmployeeCode != addEditUserDetailsReqModel.EmployeeCode && x.EmployeePersonalEmailId != addEditUserDetailsReqModel.EmployeePersonalEmailId);

						if (isUserExist.EmployeeCode == addEditUserDetailsReqModel.EmployeeCode)
						{
							isUserExist.EmployeeCode = addEditUserDetailsReqModel.EmployeeCode;
							isUserExist.Gender = addEditUserDetailsReqModel.Gender;
							isUserExist.EmergencyContact = addEditUserDetailsReqModel.EmergencyContact;
							isUserExist.Dob = addEditUserDetailsReqModel.Dob;
							isUserExist.PostCode = addEditUserDetailsReqModel.PostCode;
							isUserExist.EmploymentType = addEditUserDetailsReqModel.EmploymentType;
							isUserExist.Location = addEditUserDetailsReqModel.Location;
							isUserExist.BloodGroup = addEditUserDetailsReqModel.BloodGroup;
							isUserExist.OfferDate = addEditUserDetailsReqModel.OfferDate;
							isUserExist.JoinDate = addEditUserDetailsReqModel.JoinDate;
							isUserExist.BankName = addEditUserDetailsReqModel.BankName;
							isUserExist.AccountNumber = addEditUserDetailsReqModel.AccountNumber;
							isUserExist.Branch = addEditUserDetailsReqModel.Branch;
							isUserExist.IfscCode = addEditUserDetailsReqModel.IfscCode;
							isUserExist.PfaccountNumber = addEditUserDetailsReqModel.PfaccountNumber != null ? addEditUserDetailsReqModel.PfaccountNumber : "NA";
							isUserExist.PanCardNumber = addEditUserDetailsReqModel.PancardNumber;
							isUserExist.AadharCardNumber = addEditUserDetailsReqModel.AdharCardNumber;
							isUserExist.Salary = addEditUserDetailsReqModel.Salary;
							isUserExist.ReportingManager = addEditUserDetailsReqModel.ReportingManager;
							isUserExist.Reason = addEditUserDetailsReqModel.Reason != null ? addEditUserDetailsReqModel.Reason : "NA";
							isUserExist.EmployeePersonalEmailId = addEditUserDetailsReqModel.EmployeePersonalEmailId;
							isUserExist.ProbationPeriod = addEditUserDetailsReqModel.ProbationPeriod;
							isUserExist.UpdatedBy = _commonHelper.GetLoggedInUserId();
							isUserExist.UpdatedDate = _commonHelper.GetCurrentDateTime();

							_dbContext.Entry(isUserExist).State = EntityState.Modified;
							_dbContext.SaveChanges();

							commonResponse.Message = "UserDetails updated successfully!";
							commonResponse.Status = true;
							commonResponse.StatusCode = HttpStatusCode.OK;
						}
						else
						{
							commonResponse.Message = "EmployeeCode already exist!";
						}
					}
					else
					{
						//Add Mode
						var userDetails = await _dbRepo.UserDetailList().Where(x => x.EmployeeCode == addEditUserDetailsReqModel.EmployeeCode || x.EmployeePersonalEmailId.ToLower() == addEditUserDetailsReqModel.EmployeePersonalEmailId.ToLower()).ToListAsync();
						if (userDetails.Count == 0)
						{
							userDetailsMst.UserId = addEditUserDetailsReqModel.UserId;
							userDetailsMst.EmployeeCode = addEditUserDetailsReqModel.EmployeeCode;
							userDetailsMst.Gender = addEditUserDetailsReqModel.Gender;
							userDetailsMst.EmergencyContact = addEditUserDetailsReqModel.EmergencyContact;
							userDetailsMst.Dob = addEditUserDetailsReqModel.Dob;
							userDetailsMst.PostCode = addEditUserDetailsReqModel.PostCode;
							userDetailsMst.EmploymentType = addEditUserDetailsReqModel.EmploymentType;
							userDetailsMst.Location = addEditUserDetailsReqModel.Location;
							userDetailsMst.BloodGroup = addEditUserDetailsReqModel.BloodGroup;
							userDetailsMst.OfferDate = addEditUserDetailsReqModel.OfferDate;
							userDetailsMst.JoinDate = addEditUserDetailsReqModel.JoinDate;
							userDetailsMst.BankName = addEditUserDetailsReqModel.BankName;
							userDetailsMst.AccountNumber = addEditUserDetailsReqModel.AccountNumber;
							userDetailsMst.Branch = addEditUserDetailsReqModel.Branch;
							userDetailsMst.IfscCode = addEditUserDetailsReqModel.IfscCode;
							userDetailsMst.PfaccountNumber = addEditUserDetailsReqModel.PfaccountNumber != null ? addEditUserDetailsReqModel.PfaccountNumber : "NA";
							userDetailsMst.PanCardNumber = addEditUserDetailsReqModel.PancardNumber;
							userDetailsMst.AadharCardNumber = addEditUserDetailsReqModel.AdharCardNumber;
							userDetailsMst.Salary = addEditUserDetailsReqModel.Salary;
							userDetailsMst.ReportingManager = addEditUserDetailsReqModel.ReportingManager;
							userDetailsMst.Reason = addEditUserDetailsReqModel.Reason != null ? addEditUserDetailsReqModel.Reason : "NA";
							userDetailsMst.EmployeePersonalEmailId = addEditUserDetailsReqModel.EmployeePersonalEmailId;
							userDetailsMst.ProbationPeriod = addEditUserDetailsReqModel.ProbationPeriod;
							userDetailsMst.IsActive = true;
							userDetailsMst.IsDelete = false;
							userDetailsMst.CreatedBy = _commonHelper.GetLoggedInUserId();
							userDetailsMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
							userDetailsMst.CreatedDate = _commonHelper.GetCurrentDateTime();
							userDetailsMst.UpdatedDate = _commonHelper.GetCurrentDateTime();

							await _dbContext.AddAsync(userDetailsMst);
							await _dbContext.SaveChangesAsync();

							commonResponse.Message = "UserDetails added successfully!";
							commonResponse.Status = true;
							commonResponse.StatusCode = HttpStatusCode.OK;
						}
						else
						{
							commonResponse.Message = "UserDetails already exist!";
							commonResponse.StatusCode = HttpStatusCode.NotFound;
						}
					}
					commonResponse.Data = userDetailsMst;
				}
				else
				{
					commonResponse.Message = "Please enter the valid joindate!";
				}
			}
			catch (Exception ex)
			{
				commonResponse.Message = ex.Message;
				commonResponse.Data = ex.ToString();
			}
			return Json(commonResponse);
		}

		[HttpPost]
		public async Task<IActionResult> CheckEmployeeCode([FromBody] CheckEmployeeCodeReqModel checkEmployeeCodeReqModel)
		{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				var userDetails = await _dbRepo.UserDetailList().Where(x => x.EmployeeCode == checkEmployeeCodeReqModel.EmployeeCode.ToString() && x.UserId != checkEmployeeCodeReqModel.Id).ToListAsync();
				if (userDetails.Count == 0)
				{
					commonResponse.Status = true;
					commonResponse.StatusCode = HttpStatusCode.OK;
					commonResponse.Message = "Employee Code Valid";
				}
				else
				{
					commonResponse.Message = "Employee Code Already Exists";
				}
			}
			catch (Exception ex)
			{
				commonResponse.Message = ex.Message;
			}
			return Json(commonResponse);
		}
	}
}
