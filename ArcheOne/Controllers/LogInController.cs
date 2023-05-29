using ArcheOne.Helper;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace ArcheOne.Controllers
{
    public class LogInController : Controller
    {
        private readonly DbRepo _dbRepo;

        public LogInController(DbRepo dbRepo)
        {
            _dbRepo = dbRepo;
        }
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LogIn1([FromBody] LoginModel loginModel)
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
        public IActionResult Login([FromBody] LoginModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Perform your authentication logic here

                // If authentication is successful, create a ClaimsIdentity and sign in the user
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.UserName),
                // Add additional claims as needed
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Dashboard");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }
    }
}

