using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace ArcheOne.Controllers
{
    public class UploadedResumeController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly ArcheOneDbContext _dbContext;
        private readonly CommonHelper _commonHelper;
        public UploadedResumeController(DbRepo dbRepo, CommonHelper commonHelper, ArcheOneDbContext dbContext)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
            _dbContext = dbContext;
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

        public async Task<IActionResult> ScheduleInterview([FromBody] ScheduleInterviewReqModel request)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    InterviewMst interviewMst = new InterviewMst()
                    {
                        ResumeFileUploadId = request.ResumeFileUploadId,
                        ResumeFileUploadDetailId = request.ResumeFileUploadDetailId,
                        HireStatusId = 0,
                        OfferStatusId = 0,
                        IsActive = true,
                        IsDelete = false,
                        CreatedBy = request.CreatedBy,
                        UpdatedBy = request.CreatedBy,
                        CreatedDate = _commonHelper.GetCurrentDateTime(),
                        UpdatedDate = _commonHelper.GetCurrentDateTime(),
                    };

                    await _dbContext.InterviewMsts.AddRangeAsync(interviewMst);
                    await _dbContext.SaveChangesAsync();

                    InterviewRoundMst interviewRoundMst = new InterviewRoundMst()
                    {
                        InterviewId = interviewMst.Id,
                        InterviewRoundStatusId = Convert.ToInt32(CommonEnums.InterviewRoundStatusMst.Scheduled),
                        InterviewRoundTypeId = request.InterviewRoundTypeId,
                        InterviewStartDateTime = request.InterviewStartDateTime,
                        InterviewEndDateTime = request.InterviewStartDateTime.AddMinutes(30),
                        InterviewBy = request.InterviewBy,
                        InterviewLocation = request.InterviewLocation ?? string.Empty,
                        Notes = request.Note,
                        IsActive = true,
                        IsDelete = false,
                        CreatedBy = request.CreatedBy,
                        UpdatedBy = request.CreatedBy,
                        CreatedDate = _commonHelper.GetCurrentDateTime(),
                        UpdatedDate = _commonHelper.GetCurrentDateTime(),
                    };

                    await _dbContext.InterviewRoundMsts.AddRangeAsync(interviewRoundMst);
                    await _dbContext.SaveChangesAsync();

                    scope.Complete();

                    response.Status = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = "Interview scheduled successfully!";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Json(response);
        }
    }
}
