using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArcheOne.Controllers
{
    public class UploadedResumeController : Controller
    {
        private readonly DbRepo _dbRepo;
        public UploadedResumeController(DbRepo dbRepo)
        {
            _dbRepo = dbRepo;
        }

        public IActionResult UploadedResume()
        {
            return View();
        }

        public async Task<CommonResponse> GetUploadedResumeList()
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var data = await _dbRepo.ResumeFileUploadDetailList().ToListAsync();
                if (data != null && data.Count > 0)
                {
                    response.Message = "Data found successfully!";
                    response.Status = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Data = data;
                }
                else
                {
                    response.Message = "Data not found!";
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                }

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
