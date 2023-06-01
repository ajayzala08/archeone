using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArcheOne.Controllers
{
    public class LogInController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly ArcheOneDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly CommonHelper _commonHelper;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment { get; }

        public LogInController(DbRepo dbRepo, ArcheOneDbContext dbContext, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, CommonHelper commonHelper, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _dbRepo = dbRepo;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _commonHelper = commonHelper;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogIn([FromBody] LoginReqModel loginModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(loginModel.UserName) && string.IsNullOrEmpty(loginModel.Password))
                    {

                        var UserDetail = _dbRepo.AllUserMstList().Where(x => x.UserName.ToLower() == loginModel.UserName.ToLower() && x.Password.ToLower() == loginModel.Password.ToLower()).FirstOrDefault();
                        if (UserDetail != null)
                        {
                            _httpContextAccessor.HttpContext.Session.SetString("User", UserDetail.UserName);
                            _httpContextAccessor.HttpContext.Session.SetString("UserId", UserDetail.Id.ToString());
                            commonResponse.Status = true;
                            commonResponse.Message = "Login SuccessFully!";
                            commonResponse.Data = UserDetail;
                        }
                        else
                        {
                            commonResponse.Message = "Login Fail";
                        }
                    }
                    else
                    {
                        commonResponse.Message = "Please Enter Valid Username & Password";
                    }
                }
                else
                {
                    commonResponse.Message = "Please Enter Valid Username & Password";
                }
            }
            catch { throw; }

            return Json(commonResponse);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("LogIn", "LogIn");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordReqModel forgotPasswordreqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                if (string.IsNullOrEmpty(forgotPasswordreqModel.Email))
                {
                    var baseURL = _configuration.GetSection("SiteEmailConfigration:BaseURL").Value;
                    var res = this._dbRepo.UserMstList().Where(x => x.Email == forgotPasswordreqModel.Email).FirstOrDefault();
                    var userid = 0;
                    if (res != null)
                    {
                        userid = res.Id;
                        var datetimevalue = _commonHelper.GetCurrentDateTime().ToString("ddMMyyyyhhmmsstt");
                        baseURL += "?q=" + userid + "&d=" + datetimevalue;

                        // var ImagePath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", "EmailTemplate", "logo.png");
                        var emailTemplatePath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", "ArcheOne", "EmailTemplate", "EmailTemplate.html");
                        StreamReader str = new StreamReader(emailTemplatePath);
                        string MailText = str.ReadToEnd();
                        str.Close();

                        var htmlBody = MailText.Replace("[Resetlink]", "<a target='_blank' href='" + baseURL + "'>Reset Password</a>").Replace("[Username]", res.FirstName + " " + res.LastName);
                        //htmlBody = htmlBody.Replace("logo.png", ImagePath);
                        SendEmailRequestModel sendEmailRequestModel = new SendEmailRequestModel();
                        sendEmailRequestModel.ToEmail = forgotPasswordreqModel.Email;
                        sendEmailRequestModel.Body = htmlBody;
                        sendEmailRequestModel.Subject = "Reset Password Link";

                        var EmailSend = _commonHelper.SendEmail(sendEmailRequestModel);

                        var IsLinkSave = AddResetPasswordLink(userid, baseURL);

                        commonResponse.Status = true;
                        commonResponse.Message = "Password Reset Link Has Been Sent To Your Email!";
                        commonResponse.Data = userid;
                    }
                    else
                    {
                        commonResponse.Message = "Email Not Found!";
                    }
                }
                else
                {
                    commonResponse.Message = "Please Enter Valid Email";
                }
            }
            catch { throw; }

            return Json(commonResponse);
        }

        private bool AddResetPasswordLink(int Id, string BaseUrl)
        {
            LinkMst linkMst = new LinkMst();
            linkMst.UserId = Id;
            linkMst.IsClicked = false;
            linkMst.ResetPasswordLink = BaseUrl;
            linkMst.CreatedDate = _commonHelper.GetCurrentDateTime();
            linkMst.ExpiredDate = _commonHelper.GetCurrentDateTime().AddDays(1);
            _dbContext.LinkMsts.Add(linkMst);
            _dbContext.SaveChanges();

            return true;
        }

        [HttpGet]
        public IActionResult ResetPassword(string q, string d)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var url = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
                CheckResetPasswordLinkReqModel model = new CheckResetPasswordLinkReqModel();
                model.Id = q;
                model.Link = url;
                model.SecurityCode = d;
                commonResponse = CheckResetPasswordLink(model);
                if (commonResponse.Status)
                {
                    ViewBag.data = q;
                    ViewBag.Status = commonResponse.Status;
                    return View();
                }
                else
                {
                    ViewBag.Status = commonResponse.Status;
                    ViewBag.msg = commonResponse.Message;
                    return View();
                }
            }
            catch { throw; }
        }

        [HttpPost]
        public IActionResult ResetPassword([FromBody] ResetPasswordReqModel resetPasswordReqDTO)
        {
            CommonResponse commonResponse = new();
            try
            {
                if (string.IsNullOrEmpty(resetPasswordReqDTO.UserId) && string.IsNullOrEmpty(resetPasswordReqDTO.NewPassword))
                {
                    int userId = Convert.ToInt32(resetPasswordReqDTO.UserId);
                    var IsExistId = _dbRepo.UserMstList().Where(x => x.Id == userId).FirstOrDefault();
                    if (IsExistId != null)
                    {
                        IsExistId.Password = resetPasswordReqDTO.NewPassword;
                        _dbContext.Entry(IsExistId).State = EntityState.Modified;
                        _dbContext.SaveChanges();

                        commonResponse.Status = true;
                        commonResponse.Message = "Reset Password Sucessfully!";
                    }
                    else
                    {
                        commonResponse.Status = false;
                        commonResponse.Message = "Can Not Reset Your Password!";
                    }
                }
                else
                {
                    commonResponse.Status = false;
                    commonResponse.Message = "Please Enter Valid Password";
                }
            }
            catch (Exception)
            {
                throw;

            }
            return Json(commonResponse);
        }

        [HttpPost]
        public CommonResponse CheckResetPasswordLink(CheckResetPasswordLinkReqModel checkResetPasswordLinkReqModel)
        {
            CommonResponse commonResponse = new();
            try
            {
                if (string.IsNullOrEmpty(checkResetPasswordLinkReqModel.Id) && string.IsNullOrEmpty(checkResetPasswordLinkReqModel.Link) && string.IsNullOrEmpty(checkResetPasswordLinkReqModel.SecurityCode))
                {
                    var IsExistLink = _dbRepo.LinkMstList().Where(x => x.UserId == Convert.ToInt32(checkResetPasswordLinkReqModel.Id) && x.ResetPasswordLink == checkResetPasswordLinkReqModel.Link && x.IsClicked == false).FirstOrDefault();

                    if (IsExistLink != null)
                    {
                        if (IsExistLink.ExpiredDate <= _commonHelper.GetCurrentDateTime())
                        {
                            commonResponse.Message = "Link is expired";
                        }
                        else
                        {
                            DateTime date = DateTime.ParseExact(checkResetPasswordLinkReqModel.SecurityCode, "ddMMyyyyhhmmsstt", null);
                            date = date.AddDays(1);
                            if (_commonHelper.GetCurrentDateTime() <= date)
                            {
                                IsExistLink.IsClicked = true;
                                _dbContext.Entry(IsExistLink).State = EntityState.Modified;
                                _dbContext.SaveChanges();
                                commonResponse.Status = true;
                                commonResponse.Message = "Link is valid.";
                            }
                            else
                            {
                                commonResponse.Message = "Link is expired";
                            }
                        }
                    }
                    else
                    {
                        commonResponse.Message = "Link is expired";
                    }
                }
                else
                {
                    commonResponse.Message = "Link expired";
                }
            }
            catch (Exception)
            {
                throw;
            }
            return commonResponse;
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {

            var id = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            ViewBag.data = id;
            return View();
        }

    }
}

