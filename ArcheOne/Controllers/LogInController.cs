using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        public async Task<IActionResult> LogIn([FromBody] LoginReqModel loginModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(loginModel.UserName) && !string.IsNullOrEmpty(loginModel.Password))
                    {
                        var encryptPassword = _commonHelper.EncryptString(loginModel.Password);
                        var UserDetail = await _dbRepo.AllUserMstList().FirstOrDefaultAsync(x => x.UserName.ToLower() == loginModel.UserName.ToLower() && x.Password.ToLower() == encryptPassword.ToLower());
                        if (UserDetail != null)
                        {
                            _httpContextAccessor.HttpContext.Session.SetString("User", UserDetail.UserName);
                            _httpContextAccessor.HttpContext.Session.SetString("UserId", UserDetail.Id.ToString());


                            #region remeberme 
                            if (loginModel.RememberMe)
                            {
                                var claims = new List<Claim>
                            {
                                new Claim("Username", loginModel.UserName),
                                new Claim("Password", loginModel.Password)
                            };

                                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                                var authProperties = new AuthenticationProperties();

                                if (loginModel.RememberMe)
                                {
                                    authProperties.IsPersistent = true;
                                    authProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7);
                                }

                                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                            }
                            else
                            {
                                var existingClaims = HttpContext.User.Claims.ToList();
                                existingClaims.RemoveAll(c => c.Type == "Username" || c.Type == "Password");

                                var claimsIdentity = new ClaimsIdentity(existingClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                            }
                            #endregion

                            commonResponse.Status = true;
                            commonResponse.Message = "Login successFully!";
                            commonResponse.Data = UserDetail;
                        }
                        else
                        {
                            commonResponse.Message = "Please enter valid username & password!";
                        }
                    }
                    else
                    {
                        commonResponse.Message = "Please enter valid username & password!";
                    }
                }
                else
                {
                    commonResponse.Message = "Please enter valid username & password!";
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return Json(commonResponse);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("LogIn", "LogIn");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordReqModel forgotPasswordReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                if (!string.IsNullOrEmpty(forgotPasswordReqModel.Email))
                {
                    var baseURL = _configuration.GetSection("SiteEmailConfigure:BaseURL").Value;
                    var res = await this._dbRepo.AllUserMstList().FirstOrDefaultAsync(x => x.Email == forgotPasswordreqModel.Email);

                    if (userList != null)
                    {

                        var dateTimeValue = _commonHelper.GetCurrentDateTime().ToString("ddMMyyyyhhmmsstt");
                        baseURL += "?q=" + userList.Id + "&d=" + dateTimeValue;
                        var emailTemplatePath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", "ArcheOne", "EmailTemplate", "EmailTemplate.html");
                        StreamReader str = new StreamReader(emailTemplatePath);
                        string MailText = str.ReadToEnd();
                        str.Close();

                        var htmlBody = MailText.Replace("[ResetLink]", "<a target='_blank' href='" + baseURL + "'>Reset Password</a>").Replace("[Username]", userList.FirstName + " " + userList.LastName);
                        SendEmailRequestModel sendEmailRequestModel = new SendEmailRequestModel();
                        sendEmailRequestModel.ToEmail = forgotPasswordReqModel.Email;
                        sendEmailRequestModel.Body = htmlBody;
                        sendEmailRequestModel.Subject = "Reset Password Link";

                        var EmailSend = _commonHelper.SendEmail(sendEmailRequestModel);
                        if (EmailSend.Status)
                        {

                            var IsLinkSave = AddResetPasswordLink(userList.Id, baseURL);

                        commonResponse.Status = true;
                            commonResponse.Message = "Password reset link has been sent to your email!";
                            commonResponse.Data = userList.Id;
                    }
                    else
                    {
                            commonResponse.Status = false;
                            commonResponse.Message = "Password reset link has been not sent to your email";
                    }
                }
                else
                {
                        commonResponse.Message = "Email not found!";
                }
            }
                else
                {
                    commonResponse.Message = "Please enter valid email";
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return Json(commonResponse);
        }

        private bool AddResetPasswordLink(int Id, string BaseUrl)
        {
            try
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
            catch (Exception e)
            {
                return false;
            }
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string Id, string SecurityCode)
        {
            try
            {
                CommonResponse commonResponse = new CommonResponse();
                var url = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
                CheckResetPasswordLinkReqModel model = new CheckResetPasswordLinkReqModel();
                model.Id = Id;
                model.Link = url;
                model.SecurityCode = SecurityCode;
                commonResponse = await CheckResetPasswordLink(model);
                if (commonResponse.Status)
                {
                    ViewBag.data = Id;
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
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordReqModel resetPasswordReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                if (!string.IsNullOrEmpty(resetPasswordReqModel.UserId) && !string.IsNullOrEmpty(resetPasswordReqModel.NewPassword))
                {
                    int userId = Convert.ToInt32(resetPasswordReqModel.UserId);
                    var IsExistId = await _dbRepo.UserMstList().FirstOrDefaultAsync(x => x.Id == userId);
                    if (IsExistId != null)
                    {
                        var encryptedPassword = _commonHelper.EncryptString(resetPasswordReqModel.NewPassword);
                        IsExistId.Password = encryptedPassword;
                        _dbContext.Entry(IsExistId).State = EntityState.Modified;
                        await _dbContext.SaveChangesAsync();

                        commonResponse.Status = true;
                        commonResponse.Message = "Reset password successfully!";
                    }
                    else
                    {
                        commonResponse.Message = "Can not reset your password!";
                    }
                }
                else
                {
                    commonResponse.Status = false;
                    commonResponse.Message = "Please enter valid password";
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return Json(commonResponse);
        }

        [HttpPost]
        public async Task<CommonResponse> CheckResetPasswordLink(CheckResetPasswordLinkReqModel checkResetPasswordLinkReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                if (!string.IsNullOrEmpty(checkResetPasswordLinkReqModel.Id) && !string.IsNullOrEmpty(checkResetPasswordLinkReqModel.Link) && !string.IsNullOrEmpty(checkResetPasswordLinkReqModel.SecurityCode))
                {
                    var IsExistLink = await _dbRepo.LinkMstList().FirstOrDefaultAsync(x => x.UserId == Convert.ToInt32(checkResetPasswordLinkReqModel.Id) && x.ResetPasswordLink == checkResetPasswordLinkReqModel.Link && x.IsClicked == false);
                    if (IsExistLink != null)
                    {
                        if (IsExistLink.ExpiredDate <= _commonHelper.GetCurrentDateTime())
                        {
                            commonResponse.Message = "Link is expired!";
                        }
                        else
                        {
                            DateTime date = DateTime.ParseExact(checkResetPasswordLinkReqModel.SecurityCode, "ddMMyyyyhhmmsstt", null);
                            date = date.AddDays(1);
                            if (_commonHelper.GetCurrentDateTime() <= date)
                            {
                                IsExistLink.IsClicked = true;
                                _dbContext.Entry(IsExistLink).State = EntityState.Modified;
                                await _dbContext.SaveChangesAsync();

                                commonResponse.Status = true;
                                commonResponse.Message = "Link is valid!";
                            }
                            else
                            {
                                commonResponse.Message = "Link is expired!";
                            }
                        }
                    }
                    else
                    {
                        commonResponse.Message = "Link is expired!";
                    }
                }
                else
                {
                    commonResponse.Message = "Link is expired!";
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return commonResponse;
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            ViewBag.data = _httpContextAccessor.HttpContext.Session.GetString("UserId"); ;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordReqModel changePasswordReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                if (!string.IsNullOrEmpty(changePasswordReqModel.UserId) && !string.IsNullOrEmpty(changePasswordReqModel.OldPassword) && !string.IsNullOrEmpty(changePasswordReqModel.NewPassword))
                {

                    int userId = Convert.ToInt32(changePasswordReqModel.UserId);
                    var IsExistId = await _dbRepo.UserMstList().FirstOrDefaultAsync(x => x.Id == userId);
                    if (IsExistId != null)
                    {
                        var decryptedPassword = _commonHelper.EncryptString(changePasswordReqModel.OldPassword);
                        var isValidOldPassword = IsExistId.Password.Equals(decryptedPassword);
                        if (isValidOldPassword)
                        {
                            var encryptedPassword = _commonHelper.EncryptString(changePasswordReqModel.NewPassword);
                            IsExistId.Password = encryptedPassword;
                            _dbContext.Entry(IsExistId).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();

                            commonResponse.Status = true;
                            commonResponse.Message = "Change password successfully!";
                        }
                        else
                        {
                            commonResponse.Status = false;
                            commonResponse.Message = "Can not match your oldPassword!";
                        }
                    }
                    else
                    {
                        commonResponse.Status = false;
                        commonResponse.Message = "Can not change your password!";
                    }
                }
                else
                {
                    commonResponse.Status = false;
                    commonResponse.Message = "Please enter valid password!";
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return Json(commonResponse);
        }

        [HttpPost]
        public string GetEncryption([FromBody] EncryptDecryptReqModel encryptDecryptReqModel)
        {
            return _commonHelper.EncryptString(encryptDecryptReqModel.Text);
        }

        [HttpPost]
        public string GetDecryption([FromBody] EncryptDecryptReqModel encryptDecryptReqModel)
        {
            return _commonHelper.DecryptString(encryptDecryptReqModel.Text);
        }

    }
}

