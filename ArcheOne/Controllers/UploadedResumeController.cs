using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Transactions;
using Xceed.Words.NET;

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
                                  join interview in _dbRepo.InterviewList() on resumeFileUploadDetail.Id equals interview.ResumeFileUploadDetailId into interviewGroup
                                  from interviewItem in interviewGroup.DefaultIfEmpty()
                                  join hireStatus in _dbRepo.HireStatusList() on interviewItem.HireStatusId equals hireStatus.Id into hireStatusGroup
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
                                      FlowStatus = interviewItem == null || interviewItem.OfferStatusId == 0 ? CommonEnums.UploadedResumeTableFlowStatus.Interview_Info.ToString() : interviewItem.OfferStatusId == 1 ? CommonEnums.UploadedResumeTableFlowStatus.Cleared.ToString() : interviewItem.OfferStatusId == 2 ? CommonEnums.UploadedResumeTableFlowStatus.Offer.ToString() : hireStatusItem.HireStatusCode
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

                            var existInterviewId = await _dbRepo.InterviewList().Where(x => x.ResumeFileUploadDetailId == request.ResumeFileUploadDetailId).Select(x => x.Id).FirstOrDefaultAsync();

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
                                existInterviewRound = await _dbRepo.InterviewRoundList().AnyAsync(x => x.InterviewId == existInterviewId && (x.InterviewStartDateTime <= request.InterviewStartDateTime && x.InterviewEndDateTime >= request.InterviewStartDateTime));
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
                        var existInterviewId = await _dbRepo.InterviewList().Where(x => x.ResumeFileUploadDetailId == request.ResumeFileUploadDetailId).Select(x => x.Id).FirstOrDefaultAsync();
                        var existInterviewRound = await _dbRepo.InterviewRoundList().AnyAsync(x => x.InterviewId == existInterviewId && x.Id != request.InterviewRoundId && (x.InterviewStartDateTime <= request.InterviewStartDateTime && x.InterviewEndDateTime >= request.InterviewStartDateTime));

                        if (!existInterviewRound)
                        {
                            var interviewRound = await _dbRepo.InterviewRoundList().FirstOrDefaultAsync(x => x.Id == request.InterviewRoundId);
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
                var data = (from interview in await _dbRepo.InterviewList().ToListAsync()
                            where interview.ResumeFileUploadDetailId == ResumeId
                            join interviewRound in _dbRepo.InterviewRoundList() on interview.Id equals interviewRound.InterviewId
                            join interviewRoundType in _dbRepo.InterviewRoundTypeList() on interviewRound.InterviewRoundTypeId equals interviewRoundType.Id into interviewRoundTypeGroup
                            join interviewRoundStatus in _dbRepo.InterviewRoundStatusList() on interviewRound.InterviewRoundStatusId equals interviewRoundStatus.Id into interviewRoundStatusGroup
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

                    var interviewRoundMst = await _dbRepo.InterviewRoundList().FirstOrDefaultAsync(x => x.Id == request.InterviewRoundId);
                    if (interviewRoundMst != null)
                    {
                        string responseMessage = "Interview status changed successfully!";
                        var interviewRoundMstList = await _dbRepo.InterviewRoundList().Where(x => x.InterviewId == interviewRoundMst.InterviewId).ToListAsync();

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
                                    var interviewMst = await _dbRepo.InterviewList().FirstOrDefaultAsync(x => x.Id == interviewRoundMst.InterviewId);
                                    if (interviewMst != null)
                                    {
                                        interviewMst.OfferStatusId = Convert.ToInt32(CommonEnums.OfferStatusMst.Cleared);
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
                                var interviewMst = await _dbRepo.InterviewList().FirstOrDefaultAsync(x => x.Id == interviewRoundMst.InterviewId);
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

        public async Task<CommonResponse> UpdateInterviewHireStatus([FromBody] UpdateInterviewHireStatusReqModel request)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    var interviewMst = await _dbRepo.InterviewList().FirstOrDefaultAsync(x => x.ResumeFileUploadDetailId == request.UploadedResumeId);
                    if (interviewMst != null)
                    {
                        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            int userId = _commonHelper.GetLoggedInUserId();
                            bool executionSuccess = false;

                            if (request.OfferStatusId == Convert.ToInt32(CommonEnums.OfferStatusMst.Offer)
                                || request.OfferStatusId == Convert.ToInt32(CommonEnums.OfferStatusMst.Hire))
                            {
                                var resumeFileUploadDetailMst = await _dbRepo.ResumeFileUploadDetailList().FirstOrDefaultAsync(x => x.Id == interviewMst.ResumeFileUploadDetailId);

                                if (resumeFileUploadDetailMst != null)
                                {
                                    resumeFileUploadDetailMst.JoinInDate = request.JoinInDate;
                                    resumeFileUploadDetailMst.OfferedPackageInLac = request.OfferedPackage;
                                    resumeFileUploadDetailMst.JoinInNote = request.Note;
                                    resumeFileUploadDetailMst.UpdatedBy = userId;
                                    resumeFileUploadDetailMst.UpdatedDate = _commonHelper.GetCurrentDateTime();

                                    _dbContext.Entry(resumeFileUploadDetailMst).State = EntityState.Modified;
                                    await _dbContext.SaveChangesAsync();

                                    interviewMst.OfferStatusId = request.OfferStatusId;

                                    interviewMst.HireStatusId = request.OfferStatusId == Convert.ToInt32(CommonEnums.OfferStatusMst.Hire) ? Convert.ToInt32(CommonEnums.HireStatusMst.To_Be_Join) : 0;

                                    executionSuccess = true;
                                }
                            }
                            else
                            {
                                interviewMst.HireStatusId = request.HireStatusId;
                                executionSuccess = true;
                            }

                            interviewMst.UpdatedBy = userId;
                            interviewMst.UpdatedDate = _commonHelper.GetCurrentDateTime();

                            _dbContext.Entry(interviewMst).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();


                            if (executionSuccess)
                            {
                                scope.Complete();

                                response.Status = true;
                                response.StatusCode = System.Net.HttpStatusCode.OK;
                                response.Message = "Candidate status updated successfully!";
                            }
                            else
                            {
                                scope.Dispose();
                                response.Message = "Resume Details not found! Please try refreshing the page!";
                            }
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

        public async Task<CommonResponse> GetOfferedDetails(int ResumeId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var data = await _dbRepo.ResumeFileUploadDetailList().Where(x => x.Id == ResumeId).Select(x => new { x.Id, x.JoinInDate, x.JoinInNote, x.OfferedPackageInLac }).FirstOrDefaultAsync();

                if (data != null)
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

        private string ExtractTextFromDocument(Body body)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var paragraph in body.Elements<Paragraph>())
            {
                foreach (var run in paragraph.Elements<Run>())
                {
                    string runText = run.InnerText;

                    if (!string.IsNullOrWhiteSpace(runText))
                    {
                        sb.AppendLine(runText.Trim());
                    }
                }

                sb.AppendLine(); // Add a line break between paragraphs
            }

            return sb.ToString().Trim();
        }

        [HttpPost]
        public async Task<CommonResponse> UploadNewResume(UploadResumeReqModel request)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var filePath = @"D:\ArcheProjects\ArcheOne\TanmayBranch\archeone\ArcheOne\wwwroot\Theme\Files\ResumeFormat_V1.docx";

                using (var doc = DocX.Load(filePath))
                {
                    var tables = doc.Tables;
                    var text = string.Empty;

                    if (tables.Count > 0)
                    {
                        var validateResumeData = ValidateResumeData(tables[0]);
                        if (validateResumeData.Status)
                        {
                            var resumeFileUploadDetailMst = validateResumeData.Data;
                            int userId = _commonHelper.GetLoggedInUserId();

                            resumeFileUploadDetailMst.IsActive = true;
                            resumeFileUploadDetailMst.IsDelete = false;
                            resumeFileUploadDetailMst.CreatedBy = userId;
                            resumeFileUploadDetailMst.UpdatedBy = userId;
                            resumeFileUploadDetailMst.CreatedDate = _commonHelper.GetCurrentDateTime();
                            resumeFileUploadDetailMst.UpdatedDate = _commonHelper.GetCurrentDateTime();

                            await _dbContext.AddAsync(resumeFileUploadDetailMst);
                            await _dbContext.SaveChangesAsync();

                            response.Status = true;
                            response.StatusCode = System.Net.HttpStatusCode.OK;
                            response.Message = "Resume uploaded successfully!";
                        }
                        else
                        {
                            return validateResumeData;
                        }
                    }
                    else
                    {
                        response.Message = "Please upload resume in given format only!";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        private CommonResponse ValidateResumeData(Xceed.Document.NET.Table table)
        {
            CommonResponse response = new CommonResponse();
            string errorMessage = string.Empty;
            Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();

            for (int i = 1; i <= table.RowCount; i++)
            {
                keyValuePairs.Add(i, table.Rows[i].Cells[1].Paragraphs.Last().Text);
            }

            if (keyValuePairs[1] == "")
            {
                errorMessage += "- Full Name can not be empty!";
            }
            if (keyValuePairs[2] == "")
            {
                errorMessage += "- Self Mobile No. can not be empty!";
            }
            if (keyValuePairs[5] == "")
            {
                errorMessage += "- Email can not be empty!";
            }
            if (keyValuePairs[6] == "")
            {
                errorMessage += "- Total experience can not be empty!";
            }

            if (errorMessage == string.Empty)
            {
                ResumeFileUploadDetailMst resumeFileUploadDetailMst = new ResumeFileUploadDetailMst();
                resumeFileUploadDetailMst.FullName = table.Rows[1].Cells[1].Paragraphs.Last().Text;
                resumeFileUploadDetailMst.Mobile1 = table.Rows[2].Cells[1].Paragraphs.Last().Text;
                resumeFileUploadDetailMst.Mobile2 = table.Rows[3].Cells[1].Paragraphs.Last().Text;
                resumeFileUploadDetailMst.Mobile3 = table.Rows[4].Cells[1].Paragraphs.Last().Text;
                resumeFileUploadDetailMst.Email1 = table.Rows[5].Cells[1].Paragraphs.Last().Text;
                resumeFileUploadDetailMst.TotalExperienceAnnual = Convert.ToDecimal(table.Rows[6].Cells[1].Paragraphs.Last().Text);
                resumeFileUploadDetailMst.RelevantExperienceYear = Convert.ToDecimal(table.Rows[7].Cells[1].Paragraphs.Last().Text);
                resumeFileUploadDetailMst.HighestQualification = table.Rows[8].Cells[1].Paragraphs.Last().Text;
                resumeFileUploadDetailMst.GapReason = table.Rows[9].Cells[1].Paragraphs.Last().Text;
                resumeFileUploadDetailMst.CurrentEmployer = table.Rows[10].Cells[1].Paragraphs.Last().Text;
                resumeFileUploadDetailMst.CurrentDesignation = table.Rows[11].Cells[1].Paragraphs.Last().Text;
                resumeFileUploadDetailMst.CurrentCtcAnnual = Convert.ToDecimal(table.Rows[12].Cells[1].Paragraphs.Last().Text);
                resumeFileUploadDetailMst.CurrentTakeHomeMonthly = Convert.ToDecimal(table.Rows[13].Cells[1].Paragraphs.Last().Text);
                resumeFileUploadDetailMst.CurrentPfdeduction = Convert.ToString(table.Rows[14].Cells[1].Paragraphs.Last().Text).ToLower() == "yes" ? true : false;
                resumeFileUploadDetailMst.LastSalaryHike = table.Rows[15].Cells[1].Paragraphs.Last().Text;
                resumeFileUploadDetailMst.ExpectedCtcAnnual = Convert.ToDecimal(table.Rows[16].Cells[1].Paragraphs.Last().Text);
                resumeFileUploadDetailMst.ExpectedTakeHomeMonthly = Convert.ToDecimal(table.Rows[17].Cells[1].Paragraphs.Last().Text);
                resumeFileUploadDetailMst.ExpectedPfdeduction = Convert.ToString(table.Rows[18].Cells[1].Paragraphs.Last().Text).ToLower() == "yes" ? true : false;
                resumeFileUploadDetailMst.SalaryHikeReason = table.Rows[19].Cells[1].Paragraphs.Last().Text;
                resumeFileUploadDetailMst.NoticePeriodDays = Convert.ToDecimal(table.Rows[20].Cells[1].Paragraphs.Last().Text);
                resumeFileUploadDetailMst.ExpectedJoinInDays = Convert.ToDecimal(table.Rows[21].Cells[1].Paragraphs.Last().Text);
                resumeFileUploadDetailMst.ReasonForEarlyJoin = table.Rows[22].Cells[1].Paragraphs.Last().Text;
                resumeFileUploadDetailMst.OfferInHand = Convert.ToString(table.Rows[23].Cells[1].Paragraphs.Last().Text).ToLower() == "yes" ? true : false;
                resumeFileUploadDetailMst.HasAllDocuments = Convert.ToString(table.Rows[24].Cells[1].Paragraphs.Last().Text).ToLower() == "yes" ? true : false;
                resumeFileUploadDetailMst.CurrentLocation = table.Rows[25].Cells[1].Paragraphs.Last().Text;
                resumeFileUploadDetailMst.WorkLocation = table.Rows[26].Cells[1].Paragraphs.Last().Text;
                resumeFileUploadDetailMst.ReasonForRelocation = table.Rows[27].Cells[1].Paragraphs.Last().Text;
                resumeFileUploadDetailMst.NativePlace = table.Rows[28].Cells[1].Paragraphs.Last().Text;
                resumeFileUploadDetailMst.Dob = Convert.ToDateTime(table.Rows[29].Cells[1].Paragraphs.Last().Text);
                resumeFileUploadDetailMst.Pan = table.Rows[30].Cells[1].Paragraphs.Last().Text;
                resumeFileUploadDetailMst.TeleInterviewTime = Convert.ToDateTime(table.Rows[31].Cells[1].Paragraphs.Last().Text);
                resumeFileUploadDetailMst.F2favailability = Convert.ToString(table.Rows[31].Cells[1].Paragraphs.Last().Text).ToLower() != "" ? true : false;
                resumeFileUploadDetailMst.F2finterviewTime = resumeFileUploadDetailMst.F2favailability ? Convert.ToDateTime(table.Rows[32].Cells[1].Paragraphs.Last().Text) : null;
                resumeFileUploadDetailMst.Skills = table.Rows[33].Cells[1].Paragraphs.Last().Text;

                response.Status = true;
                response.StatusCode = System.Net.HttpStatusCode.OK;
                response.Data = resumeFileUploadDetailMst;
                response.Message = "Validation success!";
            }
            else
            {
                response.Data = errorMessage;
            }

            return response;
        }
    }
}
