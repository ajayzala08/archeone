using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        public async Task<IActionResult> ViewProfile()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var userMst = _dbRepo.AllUserMstList().FirstOrDefault(x => x.Id == _commonHelper.GetLoggedInUserId());
                if (userMst != null)
                {
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
                        //Designation = userDetails != null ? userDetails.Designation : "",
                        Email = userMst != null ? userMst.Email : "",
                        Mobile = userMst != null ? userMst.Mobile1 : "",
                        BloodGroup = userDetails != null ? userDetails.BloodGroup : ""

                    };
                    commonResponse.Data = profileResModel;
                    commonResponse.StatusCode = HttpStatusCode.OK;
                    commonResponse.Status = true;

                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Data = ex;
            }
            return View(commonResponse.Data);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeProfileImage(ChangeProfileImageReqModel changeProfileImageReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var loggedInUserDetails = _dbRepo.AllUserMstList().FirstOrDefault(x => x.Id == _commonHelper.GetLoggedInUserId());
                if (loggedInUserDetails != null)
                {

                    IFormFile file;
                    string fileName = string.Empty;
                    bool validateFileExtension = false;
                    bool validateFileSize = false;
                    string filePath = string.Empty;
                    UserMst userMst = new UserMst();
                    if (changeProfileImageReqModel.UserImage != null)
                    {
                        file = changeProfileImageReqModel.UserImage;
                        fileName = file.FileName;
                        FileInfo fileInfo = new FileInfo(fileName);
                        string fileExtension = fileInfo.Extension;
                        long fileSize = file.Length;

                        string[] allowedFileExtensions = { CommonConstant.jpeg, CommonConstant.png, CommonConstant.jpg };
                        long allowedFileSize = 1 * 1024 * 1024 * 10; // 10MB
                        validateFileExtension = allowedFileExtensions.Contains(fileExtension) ? true : false;
                        validateFileSize = fileSize <= allowedFileSize ? true : false;
                        fileName = loggedInUserDetails.UserName + loggedInUserDetails.Id + fileExtension;
                        if (validateFileExtension && validateFileSize)
                        {
                            var imageFile = _commonHelper.UploadFile(changeProfileImageReqModel.UserImage, @"UserProfile", fileName, false, true, false);
                            filePath = imageFile.Data.RelativePath;
                            if (!string.IsNullOrEmpty(filePath))
                            {
                                loggedInUserDetails.PhotoUrl = filePath;
                                _dbContext.UserMsts.Update(loggedInUserDetails);
                                await _dbContext.SaveChangesAsync();
                                commonResponse.Status = true;
                                commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
                                commonResponse.Message = "Profile image changed successfully.";

                            }
                            else
                            {
                                commonResponse.Message = "Failed to change Profile Image !";
                            }
                        }
                        else
                        {
                            commonResponse.Message = "Only jpg and png files are Allowed !";
                        }
                    }
                }
                else
                {
                    commonResponse.Message = "User details not found !";
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
