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

        public async Task<CommonResponse> GetUploadedResumeList(int ResumeFileUploadId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var data = await (from resumeFileUploadDetail in _dbRepo.ResumeFileUploadDetailList()
                                  where resumeFileUploadDetail.ResumeFileUploadId == ResumeFileUploadId
                                  join interview in _dbRepo.GetInterviewList() on resumeFileUploadDetail.Id equals interview.ResumeFileUploadDetailId into interviewGroup
                                  from interviewItem in interviewGroup.DefaultIfEmpty()
                                  join hireStatus in _dbRepo.GetHireStatusList() on interviewItem.HireStatusId equals hireStatus.Id into hireStatusGroup
                                  from hireStatusItem in hireStatusGroup.DefaultIfEmpty()
                                  select new
                                  {
                                      resumeFileUploadDetail.Id,
                                      resumeFileUploadDetail.FullName,
                                      resumeFileUploadDetail.Mobile1,
                                      resumeFileUploadDetail.Mobile2,
                                      resumeFileUploadDetail.Mobile3,
                                      resumeFileUploadDetail.Email1,
                                      resumeFileUploadDetail.Email2,
                                      resumeFileUploadDetail.TotalExperienceAnnual,
                                      resumeFileUploadDetail.RelevantExperienceYear,
                                      resumeFileUploadDetail.CurrentDesignation,
                                      resumeFileUploadDetail.Skills,
                                      FlowStatus = interviewItem.OfferStatusId == 0 ? CommonEnums.UploadedResumeTableFlowStatus.Interview_Info.ToString() : interviewItem.OfferStatusId == 1 ? CommonEnums.UploadedResumeTableFlowStatus.Offer.ToString() : hireStatusItem.HireStatusCode
                                  }).ToListAsync();
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

        public async Task<CommonResponse> ScheduleInterview([FromBody] ScheduleInterviewReqModel request)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    int userId = _commonHelper.GetLoggedInUserId();
                    // Create new round of interview
                    if (request.InterviewRoundId == 0)
                    {
                        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            bool existInterviewRound = true;

                            var existInterviewId = await _dbRepo.GetInterviewList().Where(x => x.ResumeFileUploadDetailId == request.ResumeFileUploadDetailId).Select(x => x.Id).FirstOrDefaultAsync();

                            if (existInterviewId == 0)
                            {
                                InterviewMst interviewMst = new InterviewMst()
                                {
                                    ResumeFileUploadId = request.ResumeFileUploadId,
                                    ResumeFileUploadDetailId = request.ResumeFileUploadDetailId,
                                    HireStatusId = 0,
                                    OfferStatusId = 0,
                                    IsActive = true,
                                    IsDelete = false,
                                    CreatedBy = userId,
                                    UpdatedBy = userId,
                                    CreatedDate = _commonHelper.GetCurrentDateTime(),
                                    UpdatedDate = _commonHelper.GetCurrentDateTime(),
                                };

                                await _dbContext.InterviewMsts.AddRangeAsync(interviewMst);
                                await _dbContext.SaveChangesAsync();

                                existInterviewId = interviewMst.Id;

                                existInterviewRound = false;
                            }
                            else
                            {
                                existInterviewRound = await _dbRepo.GetInterviewRoundList().AnyAsync(x => x.InterviewId == existInterviewId && (x.InterviewStartDateTime <= request.InterviewStartDateTime && x.InterviewEndDateTime >= request.InterviewStartDateTime));
                            }

                            if (!existInterviewRound)
                            {
                                InterviewRoundMst interviewRoundMst = new InterviewRoundMst()
                                {
                                    InterviewId = existInterviewId,
                                    InterviewRoundStatusId = Convert.ToInt32(CommonEnums.InterviewRoundStatusMst.Scheduled),
                                    InterviewStartDateTime = request.InterviewStartDateTime,
                                    InterviewEndDateTime = request.InterviewStartDateTime.AddMinutes(30),
                                    InterviewBy = request.InterviewBy,
                                    InterviewRoundTypeId = request.InterviewRoundTypeId,
                                    InterviewLocation = request.InterviewLocation ?? string.Empty,
                                    Notes = request.Note,
                                    IsActive = true,
                                    IsDelete = false,
                                    CreatedBy = userId,
                                    UpdatedBy = userId,
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
                            else
                            {
                                response.Message = "There is another interview scheduled in this time period with same candidate!";
                            }
                        }
                    }
                    else // Update old interview
                    {
                        var existInterviewId = await _dbRepo.GetInterviewList().Where(x => x.ResumeFileUploadDetailId == request.ResumeFileUploadDetailId).Select(x => x.Id).FirstOrDefaultAsync();
                        var existInterviewRound = await _dbRepo.GetInterviewRoundList().AnyAsync(x => x.InterviewId == existInterviewId && x.Id != request.InterviewRoundId && (x.InterviewStartDateTime <= request.InterviewStartDateTime && x.InterviewEndDateTime >= request.InterviewStartDateTime));

                        if (!existInterviewRound)
                        {
                            var interviewRound = await _dbRepo.GetInterviewRoundList().FirstOrDefaultAsync(x => x.Id == request.InterviewRoundId);
                            if (interviewRound != null)
                            {
                                interviewRound.InterviewRoundTypeId = request.InterviewRoundTypeId;
                                interviewRound.InterviewStartDateTime = request.InterviewStartDateTime;
                                interviewRound.InterviewEndDateTime = request.InterviewStartDateTime.AddMinutes(30);
                                interviewRound.InterviewBy = request.InterviewBy;
                                interviewRound.InterviewLocation = request.InterviewLocation ?? string.Empty;
                                interviewRound.Notes = request.Note;
                                interviewRound.UpdatedDate = _commonHelper.GetCurrentDateTime();
                                interviewRound.UpdatedBy = userId;

                                _dbContext.Entry(interviewRound).State = EntityState.Modified;
                                await _dbContext.SaveChangesAsync();

                                response.Status = true;
                                response.StatusCode = System.Net.HttpStatusCode.OK;
                                response.Message = "Interview Re-scheduled successfully!";
                            }
                            else
                            {
                                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                                response.Message = "Interview details not found! Please try refreshing the page!";
                            }
                        }
                        else
                        {
                            response.Message = "There is another interview scheduled in this time period with same candidate!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<CommonResponse> GetScheduledInterviewListByResumeId(int ResumeId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var data = (from interview in await _dbRepo.GetInterviewList().ToListAsync()
                            where interview.ResumeFileUploadDetailId == ResumeId
                            join interviewRound in _dbRepo.GetInterviewRoundList() on interview.Id equals interviewRound.InterviewId
                            join interviewRoundType in _dbRepo.GetInterviewRoundTypeList() on interviewRound.InterviewRoundTypeId equals interviewRoundType.Id into interviewRoundTypeGroup
                            join interviewRoundStatus in _dbRepo.GetInterviewRoundStatusList() on interviewRound.InterviewRoundStatusId equals interviewRoundStatus.Id into interviewRoundStatusGroup
                            from interviewRoundTypeItem in interviewRoundTypeGroup.DefaultIfEmpty()
                            from interviewRoundStatusItem in interviewRoundStatusGroup.DefaultIfEmpty()
                            select new
                            {
                                interviewRound.Id,
                                interviewRound.InterviewBy,
                                interviewRound.InterviewStartDateTime,
                                interviewRound.InterviewEndDateTime,
                                interviewRound.Notes,
                                interviewRoundStatusItem.InterviewRoundStatusName,
                                InterviewOn = interviewRound.InterviewRoundTypeId != 0 ? interviewRoundTypeItem.InterviewRoundTypeName : interviewRound.InterviewLocation,
                                interviewRound.InterviewRoundTypeId,
                                IsEditable = interviewRound.InterviewStartDateTime > _commonHelper.GetCurrentDateTime()
                            }).ToList();

                if (data != null && data.Count > 0)
                {
                    response.Data = data;
                    response.Status = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = "Data found successfully!";
                }
                else
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = "Data not found!";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<CommonResponse> UpdateInterviewStatus([FromBody] UpdateInterviewStatusReqModel request)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    int userId = _commonHelper.GetLoggedInUserId();

                    var interviewRoundMst = await _dbRepo.GetInterviewRoundList().FirstOrDefaultAsync(x => x.Id == request.InterviewRoundId);
                    if (interviewRoundMst != null)
                    {
                        string responseMessage = "Interview status changed successfully!";
                        var interviewRoundMstList = await _dbRepo.GetInterviewRoundList().Where(x => x.InterviewId == interviewRoundMst.InterviewId).ToListAsync();

                        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            interviewRoundMst.InterviewRoundStatusId = request.StatusId;
                            interviewRoundMst.UpdatedBy = userId;
                            interviewRoundMst.UpdatedDate = _commonHelper.GetCurrentDateTime();

                            _dbContext.Entry(interviewRoundMst).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();

                            if (request.StatusId == Convert.ToInt32(CommonEnums.InterviewRoundStatusMst.Cleared))
                            {
                                int clearedInterviewCount = interviewRoundMstList.Where(x => x.InterviewRoundStatusId == request.StatusId).Count();

                                var lastInterviewRoundDetail = interviewRoundMstList.OrderByDescending(x => x.InterviewEndDateTime).FirstOrDefault();

                                if (clearedInterviewCount == interviewRoundMstList.Count || (lastInterviewRoundDetail != null && lastInterviewRoundDetail.Id == request.InterviewRoundId))
                                {
                                    var interviewMst = await _dbRepo.GetInterviewList().FirstOrDefaultAsync(x => x.Id == interviewRoundMst.InterviewId);
                                    if (interviewMst != null)
                                    {
                                        interviewMst.OfferStatusId = Convert.ToInt32(CommonEnums.OfferStatusMst.Offer);
                                        interviewMst.UpdatedBy = userId;
                                        interviewMst.UpdatedDate = _commonHelper.GetCurrentDateTime();

                                        _dbContext.Entry(interviewMst).State = EntityState.Modified;
                                        await _dbContext.SaveChangesAsync();

                                        responseMessage = "Interview all/final rounds has been cleared successfully!";
                                    }
                                }
                            }
                            else
                            {
                                var interviewMst = await _dbRepo.GetInterviewList().FirstOrDefaultAsync(x => x.Id == interviewRoundMst.InterviewId);
                                if (interviewMst != null)
                                {
                                    interviewMst.OfferStatusId = 0;
                                    interviewMst.UpdatedBy = userId;
                                    interviewMst.UpdatedDate = _commonHelper.GetCurrentDateTime();

                                    _dbContext.Entry(interviewMst).State = EntityState.Modified;
                                    await _dbContext.SaveChangesAsync();
                                }
                            }

                            scope.Complete();

                            response.Status = true;
                            response.StatusCode = System.Net.HttpStatusCode.OK;
                            response.Message = responseMessage;
                        }
                    }
                    else
                    {
                        response.StatusCode = System.Net.HttpStatusCode.NotFound;
                        response.Message = "Interview details not found! Please try refreshing the page!";
                    }
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
