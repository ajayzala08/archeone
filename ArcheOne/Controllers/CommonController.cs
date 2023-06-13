using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArcheOne.Controllers
{
    public class CommonController : Controller
    {
        private readonly DbRepo _dbRepo;
        public CommonController(DbRepo dbRepo)
        {
            _dbRepo = dbRepo;
        }

        public async Task<CommonResponse> GetInterviewRoundTypeList()
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var data = await _dbRepo.GetInterviewRoundTypeList().Select(x => new { x.Id, x.InterviewRoundTypeName, x.InterviewRoundTypeCode }).ToListAsync();
                if (data != null && data.Count > 0)
                {
                    response.Data = data;
                    response.Message = "Data found successfully!";
                    response.Status = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
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
