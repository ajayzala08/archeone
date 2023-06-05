using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ArcheOne.Controllers
{
    public class ChatController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly CommonHelper _commonHelper;
        public ChatController(DbRepo dbRepo, CommonHelper commonHelper, IWebHostEnvironment webHostEnvironment, ArcheOneDbContext dbContext)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
        }
        public IActionResult ChatApp()
        {
            return View();
        }

        public IActionResult GetUserList()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                List<GetUserListResModel> getUserListResModels = new List<GetUserListResModel>();
                getUserListResModels = _dbRepo.AllUserMstList().Select(x => new GetUserListResModel { Id = x.Id, FullName = x.FirstName + " " + x.LastName + "( " + x.UserName + " )" }).ToList();
                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "Success!";
                commonResponse.Data = getUserListResModels;
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Data = ex.StackTrace;
            }
            return Json(commonResponse);
        }
    }
}
