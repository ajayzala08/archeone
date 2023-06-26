﻿using ArcheOne.Database.Entities;
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
        public async Task<IActionResult> LogIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn([FromBody] LoginReqModel loginModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                //loginModel.RememberMe = true;
                //Thread.Sleep(10000);
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
                            commonResponse.Message = "Login SuccessFully!";
                            commonResponse.Data = UserDetail;
                        }
                        else
                        {
                            commonResponse.Message = "Please Enter Valid Username & Password!";
                        }
                    }
                    else
                    {
                        commonResponse.Message = "Please Enter Valid Username & Password!";
                    }
                }
                else
                {
                    commonResponse.Message = "Please Enter Valid Username & Password!";
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Data = ex.StackTrace;
            }
            return Json(commonResponse);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("LogIn", "LogIn");
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordReqModel forgotPasswordreqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                if (!string.IsNullOrEmpty(forgotPasswordreqModel.Email))
                {
                    var baseURL = _configuration.GetSection("SiteEmailConfigure:BaseURL").Value;
                    var res = await this._dbRepo.UserMstList().FirstOrDefaultAsync(x => x.Email == forgotPasswordreqModel.Email);

                    if (res != null)
                    {

                        var datetimevalue = _commonHelper.GetCurrentDateTime().ToString("ddMMyyyyhhmmsstt");
                        baseURL += "?q=" + res.Id + "&d=" + datetimevalue;

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

                        var IsLinkSave = AddResetPasswordLink(res.Id, baseURL);

                        commonResponse.Status = true;
                        commonResponse.Message = "Password Reset Link Has Been Sent To Your Email!";
                        commonResponse.Data = res.Id;
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
        public async Task<IActionResult> ResetPassword(string q, string d)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var url = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
                CheckResetPasswordLinkReqModel model = new CheckResetPasswordLinkReqModel();
                model.Id = q;
                model.Link = url;
                model.SecurityCode = d;
                commonResponse = await CheckResetPasswordLink(model);
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
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordReqModel resetPasswordReqDTO)
        {
            CommonResponse commonResponse = new();
            try
            {
                if (!string.IsNullOrEmpty(resetPasswordReqDTO.UserId) && !string.IsNullOrEmpty(resetPasswordReqDTO.NewPassword))
                {
                    int userId = Convert.ToInt32(resetPasswordReqDTO.UserId);
                    var IsExistId = await _dbRepo.UserMstList().FirstOrDefaultAsync(x => x.Id == userId);
                    if (IsExistId != null)
                    {
                        var encryptedPassword = _commonHelper.EncryptString(resetPasswordReqDTO.NewPassword);
                        IsExistId.Password = encryptedPassword;
                        _dbContext.Entry(IsExistId).State = EntityState.Modified;
                        _dbContext.SaveChanges();

                        commonResponse.Status = true;
                        commonResponse.Message = "Reset Password Successfully!";
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
        public async Task<CommonResponse> CheckResetPasswordLink(CheckResetPasswordLinkReqModel checkResetPasswordLinkReqModel)
        {
            CommonResponse commonResponse = new();
            try
            {
                if (!string.IsNullOrEmpty(checkResetPasswordLinkReqModel.Id) && !string.IsNullOrEmpty(checkResetPasswordLinkReqModel.Link) && !string.IsNullOrEmpty(checkResetPasswordLinkReqModel.SecurityCode))
                {
                    var IsExistLink = _dbRepo.LinkMstList().FirstOrDefault(x => x.UserId == Convert.ToInt32(checkResetPasswordLinkReqModel.Id) && x.ResetPasswordLink == checkResetPasswordLinkReqModel.Link && x.IsClicked == false);
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
                    commonResponse.Message = "Link is expired";
                }
            }
            catch (Exception)
            {
                throw;
            }
            return commonResponse;
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            string userId = await Task.Run(() => _httpContextAccessor.HttpContext.Session.GetString("UserId"));
            ViewBag.data = userId;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordReqModel changePasswordReqModel)
        {
            CommonResponse commonResponse = new();
            try
            {
                if (!string.IsNullOrEmpty(changePasswordReqModel.UserId) && !string.IsNullOrEmpty(changePasswordReqModel.OldPassword) && !string.IsNullOrEmpty(changePasswordReqModel.NewPassword))
                {

                    int userId = Convert.ToInt32(changePasswordReqModel.UserId);
                    var IsExistId = _dbRepo.UserMstList().FirstOrDefault(x => x.Id == userId);
                    if (IsExistId != null)
                    {
                        var decryptedPassword = _commonHelper.DecryptString(changePasswordReqModel.OldPassword);
                        var isValidOldPassword = IsExistId.Password.Equals(decryptedPassword);
                        if (isValidOldPassword)
                        {
                            var encryptedPassword = _commonHelper.EncryptString(changePasswordReqModel.NewPassword);
                            IsExistId.Password = encryptedPassword;
                            _dbContext.Entry(IsExistId).State = EntityState.Modified;
                            _dbContext.SaveChanges();

                            commonResponse.Status = true;
                            commonResponse.Message = "Change Password Successfully!";
                        }
                        else
                        {
                            commonResponse.Status = false;
                            commonResponse.Message = "Can Not Match Your OldPassword!";
                        }
                    }
                    else
                    {
                        commonResponse.Status = false;
                        commonResponse.Message = "Can Not Change Your Password!";
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

