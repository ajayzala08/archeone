using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        public LogInController(DbRepo dbRepo, ArcheOneDbContext dbContext, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, CommonHelper commonHelper)
        {
            _dbRepo = dbRepo;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _commonHelper = commonHelper;
        }
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LogIn([FromBody] LoginReqModel loginModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            var UserDetail = _dbRepo.UserMstList().Where(x => x.UserName == loginModel.UserName && x.Password == loginModel.Password).FirstOrDefault();
            if (UserDetail != null && UserDetail.Id != 0)
            {
                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "Login SuccessFully!";
                commonResponse.Data = UserDetail;
            }
            else
            {
                commonResponse.Message = "Login Fail";
            }
            return Json(commonResponse);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordReqModel forgotPasswordreqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            var baseURL = _configuration.GetSection("SiteEmailConfigration:BaseURL").Value;
            var res = this._dbRepo.UserMstList().Where(x => x.Email == forgotPasswordreqModel.Email).FirstOrDefault();
            if (res != null)
            {
                var userid = res.Id.ToString();
                var datetimevalue = DateTime.Now.ToString("ddmmyyyyhhmmsstt");
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
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "Password Reset Link Has Been Sent To Your Email!";
            }
            else
            {
                commonResponse.Status = false;
                commonResponse.StatusCode = HttpStatusCode.BadRequest;
                commonResponse.Message = "Email Not Found!";
            }



            return Json(commonResponse);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("LogIn", "LogIn");
        }
        private bool AddResetPasswordLink(string Id, string BaseUrl)
        {
            int id = Convert.ToInt32(Id);
            LinkMst linkMst = new LinkMst();
            linkMst.UserId = id;
            linkMst.IsClicked = false;
            linkMst.ResetPasswordLink = BaseUrl;
            linkMst.CreatedDate = _commonHelper.GetCurrentDateTime();
            linkMst.ExpiredDate = _commonHelper.GetCurrentDateTime().AddDays(1);
            _dbContext.LinkMsts.Add(linkMst);
            _dbContext.SaveChanges();

            return true;
        }
    }
}

