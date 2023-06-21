using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult UploadedResume(int RequirementId)
        {
            return View(RequirementId);
        }

        public async Task<CommonResponse> GetUploadedResumeList(int ResumeFileUploadId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var data = await (from resumeFileUploadDetail in _dbRepo.ResumeFileUploadDetailList()
                                  where resumeFileUploadDetail.RequirementId == ResumeFileUploadId
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

        public async Task<CommonResponse> UploadNewResume(UploadResumeReqModel request)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var fileUpload = _commonHelper.UploadFile(request.ResumeFile, @"Temp", string.Empty, false, true, true);
                string filePath = fileUpload.Data.PhysicalPath;

                using (var doc = DocX.Load(filePath))
                {
                    var tables = doc.Tables;
                    var text = string.Empty;

                    if (tables.Count > 0)
                    {
                        var validateResumeData = ValidateResumeData(tables[0]);
                        if (validateResumeData.Status)
                        {
                            System.IO.File.Delete(filePath);

                            fileUpload = _commonHelper.UploadFile(request.ResumeFile, @"Resumes", string.Empty, false, true, true);
                            filePath = fileUpload.Data.RelativePath;

                            int userId = _commonHelper.GetLoggedInUserId();

                            ResumeFileUploadDetailMst resumeFileUploadDetailMst = validateResumeData.Data;
                            resumeFileUploadDetailMst.RequirementId = request.RequirementId;
                            resumeFileUploadDetailMst.FilePath = filePath;
                            resumeFileUploadDetailMst.FileName = filePath.Split("\\").Last();

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

            for (int i = 1; i < table.RowCount; i++)
            {
                keyValuePairs.Add(i, table.Rows[i].Cells[1].Paragraphs.Last().Text);
            }

            string NotNull = CommonEnums.ValidationTypes.NotNullOrEmpty.ToString();
            string OnlyDecimal = CommonEnums.ValidationTypes.OnlyDecimal.ToString();
            string OnlyDateTime = CommonEnums.ValidationTypes.OnlyDateTime.ToString();

            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[1]) ? "" : "- Full Name can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[2]) ? "" : "- Self Mobile No. can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[5]) ? "" : "- Email can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[6]) ? CheckFieldValidation(OnlyDecimal, keyValuePairs[6]) ? "" : "- Only Decimal value is allowed in Total experience!\n" : "- Total experience can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[7]) ? CheckFieldValidation(OnlyDecimal, keyValuePairs[7]) ? "" : "- Only Decimal value is allowed in Relevant experience!\n" : "- Relevant experience can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[8]) ? "" : "- Highest qualification can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[12]) ? CheckFieldValidation(OnlyDecimal, keyValuePairs[12]) ? "" : "- Only Decimal value is allowed in Current CTC (Annual)!\n" : "- Current CTC (Annual) can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[13]) ? CheckFieldValidation(OnlyDecimal, keyValuePairs[13]) ? "" : "- Only Decimal value is allowed in Current take home (Monthly)!\n" : "- Current take home (Monthly) can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[16]) ? CheckFieldValidation(OnlyDecimal, keyValuePairs[16]) ? "" : "- Only Decimal value is allowed in Expected CTC (Annual)!\n" : "- Expected CTC (Annual) can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[17]) ? CheckFieldValidation(OnlyDecimal, keyValuePairs[17]) ? "" : "- Only Decimal value is allowed in Expected take home (Monthly)!\n" : "- Expected take home (Monthly) can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[21]) ? CheckFieldValidation(OnlyDecimal, keyValuePairs[20]) ? "" : "- Only Decimal value is allowed in Official notice period (Days)!\n" : "- Official notice period (Days) can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[21]) ? CheckFieldValidation(OnlyDecimal, keyValuePairs[21]) ? "" : "- Only Decimal value is allowed in Can join in (Days)!\n" : "- Can join in (Days) can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[24]) ? "" : "- Does candidate have documents can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[25]) ? "" : "- Current location can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[26]) ? "" : "- Work location can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[28]) ? "" : "- Native place can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[29]) ? CheckFieldValidation(OnlyDateTime, keyValuePairs[29]) ? "" : "- Only dd/MM/yyyy format is allowed in Date of birth!\n" : "- Date of birth can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[30]) ? "" : "- PAN Card No. can not be empty!\n";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[31]) ? "" : CheckFieldValidation(OnlyDateTime, keyValuePairs[31], "dd/MM/yyyy hh:mm tt") ? "- Only dd/MM/yyyy hh:mm tt (31/12/2000 01:59 PM) format is allowed in Telephonic interview timing!\n" : "";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[32]) ? "" : CheckFieldValidation(OnlyDateTime, keyValuePairs[32], "dd/MM/yyyy hh:mm tt") ? "- Only dd/MM/yyyy hh:mm tt (31/12/2000 01:59 PM) format is allowed in F2F interview availability!\n" : "";
            errorMessage += CheckFieldValidation(NotNull, keyValuePairs[33]) ? "" : "- Skills can not be empty!\n";

            if (errorMessage == string.Empty)
            {
                ResumeFileUploadDetailMst resumeFileUploadDetailMst = new ResumeFileUploadDetailMst();
                resumeFileUploadDetailMst.FullName = keyValuePairs[1];
                resumeFileUploadDetailMst.Mobile1 = keyValuePairs[2];
                resumeFileUploadDetailMst.Mobile2 = keyValuePairs[3];
                resumeFileUploadDetailMst.Mobile3 = keyValuePairs[4];
                resumeFileUploadDetailMst.Email1 = keyValuePairs[5];
                resumeFileUploadDetailMst.TotalExperienceAnnual = Convert.ToDecimal(keyValuePairs[6]);
                resumeFileUploadDetailMst.RelevantExperienceYear = Convert.ToDecimal(keyValuePairs[7]);
                resumeFileUploadDetailMst.HighestQualification = keyValuePairs[8];
                resumeFileUploadDetailMst.GapReason = keyValuePairs[9];
                resumeFileUploadDetailMst.CurrentEmployer = keyValuePairs[10];
                resumeFileUploadDetailMst.CurrentDesignation = keyValuePairs[11];
                resumeFileUploadDetailMst.CurrentCtcAnnual = Convert.ToDecimal(keyValuePairs[12]);
                resumeFileUploadDetailMst.CurrentTakeHomeMonthly = Convert.ToDecimal(keyValuePairs[13]);
                resumeFileUploadDetailMst.CurrentPfdeduction = Convert.ToString(keyValuePairs[14]).ToLower() == "yes" ? true : false;
                resumeFileUploadDetailMst.LastSalaryHike = keyValuePairs[15];
                resumeFileUploadDetailMst.ExpectedCtcAnnual = Convert.ToDecimal(keyValuePairs[16]);
                resumeFileUploadDetailMst.ExpectedTakeHomeMonthly = Convert.ToDecimal(keyValuePairs[17]);
                resumeFileUploadDetailMst.ExpectedPfdeduction = Convert.ToString(keyValuePairs[18]).ToLower() == "yes" ? true : false;
                resumeFileUploadDetailMst.SalaryHikeReason = keyValuePairs[19];
                resumeFileUploadDetailMst.NoticePeriodDays = Convert.ToDecimal(keyValuePairs[20]);
                resumeFileUploadDetailMst.ExpectedJoinInDays = Convert.ToDecimal(keyValuePairs[21]);
                resumeFileUploadDetailMst.ReasonForEarlyJoin = keyValuePairs[22];
                resumeFileUploadDetailMst.OfferInHand = Convert.ToString(keyValuePairs[23]).ToLower() == "yes" ? true : false;
                resumeFileUploadDetailMst.HasAllDocuments = Convert.ToString(keyValuePairs[24]).ToLower() == "yes" ? true : false;
                resumeFileUploadDetailMst.CurrentLocation = keyValuePairs[25];
                resumeFileUploadDetailMst.WorkLocation = keyValuePairs[26];
                resumeFileUploadDetailMst.ReasonForRelocation = keyValuePairs[27];
                resumeFileUploadDetailMst.NativePlace = keyValuePairs[28];
                resumeFileUploadDetailMst.Dob = Convert.ToDateTime(keyValuePairs[29]);
                resumeFileUploadDetailMst.Pan = keyValuePairs[30];

                DateTime interviewTime;
                resumeFileUploadDetailMst.TeleInterviewTime = _commonHelper.IsValidDateTime(keyValuePairs[31], "dd/MM/yyyy hh:mm tt", out interviewTime) ? interviewTime : null;
                resumeFileUploadDetailMst.F2favailability = resumeFileUploadDetailMst.TeleInterviewTime != null ? true : false;
                resumeFileUploadDetailMst.F2finterviewTime = _commonHelper.IsValidDateTime(keyValuePairs[32], "dd/MM/yyyy hh:mm tt", out interviewTime) ? interviewTime : null;
                resumeFileUploadDetailMst.Skills = keyValuePairs[33];

                response.Status = true;
                response.StatusCode = System.Net.HttpStatusCode.OK;
                response.Data = resumeFileUploadDetailMst;
                response.Message = "Validation success!";
            }
            else
            {
                response.Data = errorMessage;
                response.Message = "Please verify and correct all the errors and validation from the uploaded resume!";
            }

            return response;
        }

        private bool CheckFieldValidation(string fieldValidationType, string value, string? dateTimeFormat = "dd/MM/yyyy")
        {
            if (fieldValidationType.Contains(CommonEnums.ValidationTypes.NotNullOrEmpty.ToString()))
            {
                return !string.IsNullOrEmpty(value);
            }
            else if (fieldValidationType.Contains(CommonEnums.ValidationTypes.OnlyDecimal.ToString()))
            {
                return _commonHelper.IsValidDecimal(value);
            }
            else if (fieldValidationType.Contains(CommonEnums.ValidationTypes.OnlyDateTime.ToString()))
            {
                return _commonHelper.IsValidDateTime(value, dateTimeFormat, out _);
            }
            else { return false; }
        }
    }
}
