using ArcheOne.Helper.CommonModels;
using Microsoft.AspNetCore.Mvc;

namespace ArcheOne.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult PageNotFound()
        {
            return View();
        }
        public IActionResult Forbidden(bool isAjax)
        {
            CommonResponse response = new CommonResponse();
            if (isAjax)
            {
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.Message = "You're not authorized to perform this action!";
                return Json(response);
            }

            return View(response);
        }
    }
}
