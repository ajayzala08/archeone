using ArcheOne.Helper;
<<<<<<< HEAD
=======
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
>>>>>>> cd527d961c63f0609fca23c3d960641591127856
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        public IActionResult LogIn([FromBody] LoginModel loginModel)
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
    }
}
