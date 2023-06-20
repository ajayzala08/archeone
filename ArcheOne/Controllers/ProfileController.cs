using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;

namespace ArcheOne.Controllers
{
    public class ProfileController : Controller
    {
        private readonly CommonHelper _commonHelper;
        private readonly DbRepo _dbRepo;
        private readonly ArcheOneDbContext _dbContext;

        public ProfileController(ArcheOneDbContext dbContext, CommonHelper commonHelper, DbRepo dbRepo)
        {
            _dbContext = dbContext;
            _commonHelper = commonHelper;
            _dbRepo = dbRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Viewprofile()
        {
            var userMst = _dbRepo.UserMstList().FirstOrDefault(x => x.Id == _commonHelper.GetLoggedInUserId());
            var userDetails = _dbRepo.UserDetailList().FirstOrDefault(x => x.UserId == _commonHelper.GetLoggedInUserId());
            ProfileResModel profileResModel = new ProfileResModel()
            {
                UserId = userMst != null ? userMst.Id : 0,
                FullName = userMst != null ? (userMst.FirstName.ToString() + " " + userMst.MiddleName.ToString() + " " + userMst.LastName.ToString()) : "",
                EmployeeCode = userDetails != null ? userDetails.EmployeeCode.ToString() : "",
                DOB = userDetails != null ? userDetails.Dob.ToString("dd MMMM yyyy") : "",
                DOJ = userDetails != null ? userDetails.JoinDate.ToString("dd MMMM yyyy") : "",
                ProfileImage = userMst != null ? (userMst.PhotoUrl.ToString() != "" ? userMst.PhotoUrl.ToString() : "Theme\\Logo\\default_user_profile.png") : "Theme\\Logo\\default_user_profile.png",
                Address = userMst != null ? ($"{userMst.Address.ToString()} {userMst.Pincode.ToString()}") : "",
                Designation = userDetails != null ? userDetails.Designation : "",
                Email = userMst != null ? userMst.Email : "",
                Mobile = userMst != null ? userMst.Mobile1 : "",
                BloodGroup = userDetails != null ? userDetails.BloodGroup : ""

            };



            /*var userProfileDetails = from userMst in _dbRepo.UserMstList()
                                     where userMst.Id == _commonHelper.GetLoggedInUserId()
                                     join userDetail in _dbRepo.UserDetailList() on userMst.Id equals userDetail.UserId into Details
                                     from tempUserDetails in Details.DefaultIfEmpty()
                                     select new
                                     {
                                         UserId = userMst.Id,
                                         FullName = userMst.FirstName.ToString() + " " + userMst.MiddleName.ToString() + " " + userMst.LastName.ToString(),
                                         EmployeeCode = tempUserDetails.EmployeeCode.ToString(),
                                         DOB = tempUserDetails.Dob.ToString("dd MMMM yyyy"),
                                         DOJ = tempUserDetails.JoinDate.ToString("dd MMMM yyyy"),
                                         ProfileImage = userMst.PhotoUrl.ToString() != "" ? userMst.PhotoUrl.ToString() : "Theme\\Logo\\default_user_profile.png",
                                         Address = $"{userMst.Address.ToString()} {userMst.Pincode.ToString()}",
                                         Designation = tempUserDetails.Designation,
                                         Email = userMst.Email,
                                         Mobile = userMst.Mobile1
                                     };*/


            /*(from userMst in _dbRepo.UserMstList()
                                           where userMst.Id == _commonHelper.GetLoggedInUserId()
                                           join userDetail in _dbRepo.UserDetailList()
                                           on userMst.Id equals userDetail.UserId
                                           select new { userMst, userDetail }
                                          ).Select(x => new ProfileResModel
                                          {
                                              UserId = (int)x.userMst.Id,
                                              FullName = x.userMst.FirstName.ToString() + " " + x.userMst.MiddleName.ToString() + " " + x.userMst.LastName.ToString(),
                                              EmployeeCode = x.userDetail.EmployeeCode.ToString(),
                                              Designation = x.userDetail.Designation.ToString(),
                                              Email = x.userMst.Email.ToString(),
                                              Mobile = x.userMst.Mobile1.ToString(),
                                              DOB = x.userDetail.Dob.ToString("dd MMMM yyyy"),
                                              DOJ = x.userDetail.JoinDate.ToString("dd MMMM yyyy"),
                                              ProfileImage = x.userMst.PhotoUrl.ToString() != "" ? x.userMst.PhotoUrl.ToString() : "Theme\\Logo\\default_user_profile.png",
                                              Address = $"{x.userMst.Address.ToString()} {x.userMst.Pincode.ToString()}",
                                          }).FirstOrDefault();*/


            //_dbRepo.UserMstList().FirstOrDefault(x => x.Id == _commonHelper.GetLoggedInUserId());
            return View(profileResModel);
        }
    }
}
