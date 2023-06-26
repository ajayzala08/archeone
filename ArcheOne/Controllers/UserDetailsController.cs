using System.Net;
using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
				userDetailsAddEditResModel.DepartmentList = _dbRepo.DepartmentList().ToList();
				userDetailsAddEditResModel.DesignationList = _dbRepo.DesignationList().ToList();
				userDetailsAddEditResModel.EmploymentTypeList = _dbRepo.EmploymentTypeList().ToList();
				userDetailsAddEditResModel.ReportingManagerList = _dbRepo.ReportingManagerList().ToList();
				userDetailsAddEditResModel.UserDetail.UserId = userId;
				var isUserDetailsExist = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == userId);
				if (isUserDetailsExist != null)
				{
					userDetailsAddEditResModel.UserDetail.Id = isUserDetailsExist.Id;
					userDetailsAddEditResModel.UserDetail.UserId = isUserDetailsExist.UserId;
					userDetailsAddEditResModel.UserDetail.EmployeeCode = isUserDetailsExist.EmployeeCode;
					userDetailsAddEditResModel.UserDetail.Gender = isUserDetailsExist.Gender;
					userDetailsAddEditResModel.UserDetail.EmergencyContact = isUserDetailsExist.EmergencyContact;
					userDetailsAddEditResModel.UserDetail.Dob = isUserDetailsExist.Dob.ToString("dd-MM-yyyy");
					userDetailsAddEditResModel.UserDetail.PostCode = isUserDetailsExist.PostCode;
					userDetailsAddEditResModel.UserDetail.EmploymentType = isUserDetailsExist.EmploymentType;
					userDetailsAddEditResModel.UserDetail.Department = isUserDetailsExist.Department;
					userDetailsAddEditResModel.UserDetail.Designation = isUserDetailsExist.Designation;
					userDetailsAddEditResModel.UserDetail.Location = isUserDetailsExist.Location;
					userDetailsAddEditResModel.UserDetail.BloodGroup = isUserDetailsExist.BloodGroup;
					userDetailsAddEditResModel.UserDetail.OfferDate = isUserDetailsExist.OfferDate.ToString("dd-MM-yyyy");
					userDetailsAddEditResModel.UserDetail.JoinDate = isUserDetailsExist.JoinDate.ToString("dd-MM-yyyy");
					userDetailsAddEditResModel.UserDetail.BankName = isUserDetailsExist.BankName;
					userDetailsAddEditResModel.UserDetail.AccountNumber = isUserDetailsExist.AccountNumber;
					userDetailsAddEditResModel.UserDetail.Branch = isUserDetailsExist.Branch;
					userDetailsAddEditResModel.UserDetail.IfscCode = isUserDetailsExist.IfscCode;
					userDetailsAddEditResModel.UserDetail.PfaccountNumber = isUserDetailsExist.PfaccountNumber;
					userDetailsAddEditResModel.UserDetail.PancardNumber = isUserDetailsExist.PancardNumber;
					userDetailsAddEditResModel.UserDetail.AdharCardNumber = isUserDetailsExist.AdharCardNumber;
					userDetailsAddEditResModel.UserDetail.Salary = isUserDetailsExist.Salary;
					userDetailsAddEditResModel.UserDetail.ReportingManager = isUserDetailsExist.ReportingManager;
					userDetailsAddEditResModel.UserDetail.Reason = isUserDetailsExist.Reason;
					userDetailsAddEditResModel.UserDetail.EmployeePersonalEmailId = isUserDetailsExist.EmployeePersonalEmailId;
					userDetailsAddEditResModel.UserDetail.ProbationPeriod = isUserDetailsExist.ProbationPeriod;
					userDetailsAddEditResModel.UserDetail.IsActive = isUserDetailsExist.IsActive;
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
				var isUserExist = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == addEditUserDetailsReqModel.UserId);
				if (isUserExist != null)
				{
					// Edit Mode
					//var editUserDetails = _dbRepo.UserDetailList().FirstOrDefault(x => x.Id == addEditUserDetailsReqModel.Id && x.EmployeeCode != addEditUserDetailsReqModel.EmployeeCode && x.EmployeePersonalEmailId != addEditUserDetailsReqModel.EmployeePersonalEmailId);

					var editUserDetails = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.EmployeeCode == addEditUserDetailsReqModel.EmployeeCode);
					if (editUserDetails != null)
					{
						editUserDetails.EmployeeCode = addEditUserDetailsReqModel.EmployeeCode;
						editUserDetails.Gender = addEditUserDetailsReqModel.Gender;
						editUserDetails.EmergencyContact = addEditUserDetailsReqModel.EmergencyContact;
						editUserDetails.Dob = addEditUserDetailsReqModel.Dob;
						editUserDetails.PostCode = addEditUserDetailsReqModel.PostCode;
						editUserDetails.EmploymentType = addEditUserDetailsReqModel.EmploymentType;
						editUserDetails.Department = addEditUserDetailsReqModel.Department;
						editUserDetails.Designation = addEditUserDetailsReqModel.Designation;
						editUserDetails.Location = addEditUserDetailsReqModel.Location;
						editUserDetails.BloodGroup = addEditUserDetailsReqModel.BloodGroup;
						editUserDetails.OfferDate = addEditUserDetailsReqModel.OfferDate;
						editUserDetails.JoinDate = addEditUserDetailsReqModel.JoinDate;
						editUserDetails.BankName = addEditUserDetailsReqModel.BankName;
						editUserDetails.AccountNumber = addEditUserDetailsReqModel.AccountNumber;
						editUserDetails.Branch = addEditUserDetailsReqModel.Branch;
						editUserDetails.IfscCode = addEditUserDetailsReqModel.IfscCode;
						editUserDetails.PfaccountNumber = addEditUserDetailsReqModel.PfaccountNumber;
						editUserDetails.PancardNumber = addEditUserDetailsReqModel.PancardNumber;
						editUserDetails.AdharCardNumber = addEditUserDetailsReqModel.AdharCardNumber;
						editUserDetails.Salary = addEditUserDetailsReqModel.Salary;
						editUserDetails.ReportingManager = addEditUserDetailsReqModel.ReportingManager;
						editUserDetails.Reason = addEditUserDetailsReqModel.Reason;
						editUserDetails.EmployeePersonalEmailId = addEditUserDetailsReqModel.EmployeePersonalEmailId;
						editUserDetails.ProbationPeriod = addEditUserDetailsReqModel.ProbationPeriod;
						editUserDetails.UpdatedBy = _commonHelper.GetLoggedInUserId();
						editUserDetails.UpdatedDate = _commonHelper.GetCurrentDateTime();

						_dbContext.Entry(editUserDetails).State = EntityState.Modified;
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
						userDetailsMst.Department = addEditUserDetailsReqModel.Department;
						userDetailsMst.Designation = addEditUserDetailsReqModel.Designation;
						userDetailsMst.Location = addEditUserDetailsReqModel.Location;
						userDetailsMst.BloodGroup = addEditUserDetailsReqModel.BloodGroup;
						userDetailsMst.OfferDate = addEditUserDetailsReqModel.OfferDate;
						userDetailsMst.JoinDate = addEditUserDetailsReqModel.JoinDate;
						userDetailsMst.BankName = addEditUserDetailsReqModel.BankName;
						userDetailsMst.AccountNumber = addEditUserDetailsReqModel.AccountNumber;
						userDetailsMst.Branch = addEditUserDetailsReqModel.Branch;
						userDetailsMst.IfscCode = addEditUserDetailsReqModel.IfscCode;
						userDetailsMst.PfaccountNumber = addEditUserDetailsReqModel.PfaccountNumber;
						userDetailsMst.PancardNumber = addEditUserDetailsReqModel.PancardNumber;
						userDetailsMst.AdharCardNumber = addEditUserDetailsReqModel.AdharCardNumber;
						userDetailsMst.Salary = addEditUserDetailsReqModel.Salary;
						userDetailsMst.ReportingManager = addEditUserDetailsReqModel.ReportingManager;
						userDetailsMst.Reason = addEditUserDetailsReqModel.Reason;
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
			catch (Exception ex)
			{
				commonResponse.Message = ex.Message;
				commonResponse.Data = ex;
			}
			return Json(commonResponse);
		}
	}
}
