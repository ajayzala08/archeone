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
		public IActionResult UserDetails()
		{
			return View();
		}

		//[HttpPost]
		//public async Task<IActionResult> AddEditUserDetails(int userId)
		//{
		//	CommonResponse commonResponse = new CommonResponse();
		//	try
		//	{
		//	}
		//	catch (Exception ex)
		//	{
		//		commonResponse.Message = ex.Message;
		//		commonResponse.Data = ex;
		//	}
		//	return View(userId);
		//}

		[HttpPost]
		public async Task<IActionResult> AddEditUserDetails(int userId, int id)
		{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				AddEditUserDetailsReqModel addEditUserDetailsReqModel = new AddEditUserDetailsReqModel();
				if (userId > 0)
				{
					AddEditUserDetailsResModel addEditUserDetailsResModel = new AddEditUserDetailsResModel();
					var isUserExist = _dbRepo.UserDetailList().FirstOrDefault(x => x.UserId == userId);
					if (isUserExist != null)
					{
						// Edit Mode
						//var editUserDetails = _dbRepo.UserDetailList().FirstOrDefault(x => x.);
					}
					else
					{
						var userDetails = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.EmployeeCode == addEditUserDetailsReqModel.EmployeeCode && x.EmployeePersonalEmailId.ToLower() == addEditUserDetailsReqModel.EmployeePersonalEmailId.ToLower());
						if (userDetails != null)
						{
							addEditUserDetailsResModel.EmployeeCode = addEditUserDetailsReqModel.EmployeeCode;
							addEditUserDetailsResModel.Gender = addEditUserDetailsReqModel.Gender;
							addEditUserDetailsResModel.EmergencyContact = addEditUserDetailsReqModel.EmergencyContact;
							addEditUserDetailsResModel.Dob = addEditUserDetailsReqModel.Dob;
							addEditUserDetailsResModel.PostCode = addEditUserDetailsReqModel.PostCode;
							addEditUserDetailsResModel.EmploymentType = addEditUserDetailsReqModel.EmploymentType;
							addEditUserDetailsResModel.Department = addEditUserDetailsReqModel.Department;
							addEditUserDetailsResModel.Designation = addEditUserDetailsReqModel.Designation;
							addEditUserDetailsResModel.Location = addEditUserDetailsReqModel.Location;
							addEditUserDetailsResModel.BloodGroup = addEditUserDetailsReqModel.BloodGroup;
							addEditUserDetailsResModel.OfferDate = addEditUserDetailsReqModel.OfferDate;
							addEditUserDetailsResModel.JoinDate = addEditUserDetailsReqModel.JoinDate;
							addEditUserDetailsResModel.BankName = addEditUserDetailsReqModel.BankName;
							addEditUserDetailsResModel.AccountNumber = addEditUserDetailsReqModel.AccountNumber;
							addEditUserDetailsResModel.Branch = addEditUserDetailsReqModel.Branch;
							addEditUserDetailsResModel.IfscCode = addEditUserDetailsReqModel.IfscCode;
							addEditUserDetailsResModel.PfaccountNumber = addEditUserDetailsReqModel.PfaccountNumber;
							addEditUserDetailsResModel.PancardNumber = addEditUserDetailsReqModel.PancardNumber;
							addEditUserDetailsResModel.AdharCardNumber = addEditUserDetailsReqModel.AdharCardNumber;
							addEditUserDetailsResModel.Salary = addEditUserDetailsReqModel.Salary;
							addEditUserDetailsResModel.ReportingManager = addEditUserDetailsReqModel.ReportingManager;
							addEditUserDetailsResModel.Reason = addEditUserDetailsReqModel.Reason;
							addEditUserDetailsResModel.EmployeePersonalEmailId = addEditUserDetailsReqModel.EmployeePersonalEmailId;
							addEditUserDetailsResModel.ProbationPeriod = addEditUserDetailsReqModel.ProbationPeriod;

							_dbContext.Add(addEditUserDetailsResModel);
							_dbContext.SaveChanges();

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
					commonResponse.Data = addEditUserDetailsResModel;
				}
				else
				{
					commonResponse.Message = "Invalid User!";
					commonResponse.StatusCode = HttpStatusCode.NotFound;
				}
			}
			catch (Exception ex)
			{
				commonResponse.Message = ex.Message;
				commonResponse.Data = ex;
			}
			return View();
		}
	}
}
