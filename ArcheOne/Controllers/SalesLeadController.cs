using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;
using System.Transactions;

namespace ArcheOne.Controllers
{
    public class SalesLeadController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly ArcheOneDbContext _dbContext;
        private readonly CommonHelper _commonHelper;
        public SalesLeadController(DbRepo dbRepo, ArcheOneDbContext dbContext, CommonHelper commonHelper)
        {
            _dbRepo = dbRepo;
            _dbContext = dbContext;
            _commonHelper = commonHelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SalesList()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                /*List<SalesLeadListResViewModel> salesLeadListResViewModels = new List<SalesLeadListResViewModel>();
				salesLeadListResViewModels = await _dbRepo.SalesLeadList().Where(x => x.IsActive == true && x.IsDelete == false).Select(x => new SalesLeadListResViewModel
				{
					Id = x.Id,
					OrgName = x.OrgName,
					Address = x.Address,
					CountryId = x.CountryId,
					StateId = x.StateId,
					CityId = x.CityId,
					Phone1 = x.Phone1,
					Phone2 = x.Phone2,
					Email1 = x.Email1,
					Email2 = x.Email2,
					WebsiteUrl = x.WebsiteUrl
				}).ToListAsync();
				commonResponse.Data = salesLeadListResViewModels;*/

                var salesLeadListing = await (from salesLeadList in _dbRepo.SalesLeadList()
                                              join salescontactPersonList in _dbRepo.SalesContactPersonList()
                                              on salesLeadList.Id equals salescontactPersonList.SalesLeadId
                                              select new { salesLeadList, salescontactPersonList }
                                                   ).ToListAsync();
                List<SalesLeadResModel> salesContactPersonList = new List<SalesLeadResModel>();
                foreach (var item in salesLeadListing)
                {
                    string salesLeadStatus = string.Empty;
                    var salesLeadFollowUp = _dbRepo.salesLeadFollowUpMst().OrderBy(x => x.Id).LastOrDefault(x => x.SalesLeadId == item.salesLeadList.Id);
                    if (salesLeadFollowUp != null)
                    {
                        salesLeadStatus = _dbRepo.SalesLeadStatusList().FirstOrDefault(x => x.Id == salesLeadFollowUp.SalesLeadStatusId).SalesLeadStatusName;
                    }
                    else
                    {
                        salesLeadStatus = null;
                    }
                    salesContactPersonList.Add(new SalesLeadResModel
                    {
                        Id = item.salesLeadList.Id,
                        OrgName = item.salesLeadList.OrgName,
                        ContactPersonId = item.salescontactPersonList.Id,
                        FullName = $"{item.salescontactPersonList.FirstName} {item.salescontactPersonList.LastName}",
                        Mobile = item.salescontactPersonList.Mobile1,
                        Email = item.salescontactPersonList.Email,
                        Designation = item.salescontactPersonList.Designation,
                        LeadStatus = salesLeadStatus

                    });
                }

                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "Record Found";
                commonResponse.Data = salesContactPersonList;

            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Data = ex.ToString();
            }
            return Json(commonResponse);
        }

        public async Task<IActionResult> AddEditSalesLead(int SalesLeadId)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                SalesLeadAddEditResModel salesLeadAddEdit = new SalesLeadAddEditResModel();
                salesLeadAddEdit.salesLeadDetail = new SalesLeadDetail();
                salesLeadAddEdit.ContactPersonDetailList = new List<SalesLeadContactPersonDetail>();
                salesLeadAddEdit.CountryList = await _dbRepo.CountryList().Select(x => new KeyValueModel { Id = (int)x.CountryId, Name = x.CountryName }).ToListAsync();
                var SalesLeadDetails = await _dbRepo.SalesLeadList().FirstOrDefaultAsync(x => x.Id == SalesLeadId);
                if (SalesLeadDetails != null)
                {
                    salesLeadAddEdit.salesLeadDetail.SalesLeadId = SalesLeadDetails.Id;
                    salesLeadAddEdit.salesLeadDetail.OrgName = SalesLeadDetails.OrgName;
                    salesLeadAddEdit.salesLeadDetail.CountryId = SalesLeadDetails.CountryId;
                    salesLeadAddEdit.salesLeadDetail.StateId = SalesLeadDetails.StateId;
                    salesLeadAddEdit.salesLeadDetail.CityId = SalesLeadDetails.CityId;
                    salesLeadAddEdit.salesLeadDetail.Address = SalesLeadDetails.Address;
                    salesLeadAddEdit.salesLeadDetail.Phone1 = SalesLeadDetails.Phone1;
                    salesLeadAddEdit.salesLeadDetail.Phone2 = SalesLeadDetails.Phone2;
                    salesLeadAddEdit.salesLeadDetail.Email1 = SalesLeadDetails.Email1;
                    salesLeadAddEdit.salesLeadDetail.Email2 = SalesLeadDetails.Email2;
                    salesLeadAddEdit.salesLeadDetail.WebsiteUrl = SalesLeadDetails.WebsiteUrl;
                    salesLeadAddEdit.salesLeadDetail.IsActive = SalesLeadDetails.IsActive;




                    var ContactPersonList = await _dbRepo.SalesContactPersonList().Where(x => x.SalesLeadId == SalesLeadDetails.Id).Take(3).ToListAsync();

                    foreach (var item in ContactPersonList)
                    {
                        SalesLeadContactPersonDetail salesLeadContactPersonDetail = new SalesLeadContactPersonDetail();
                        salesLeadContactPersonDetail.SalesLeadContactPersonId = item.Id;
                        salesLeadContactPersonDetail.SalesLeadId = item.SalesLeadId;
                        salesLeadContactPersonDetail.FirstName = item.FirstName;
                        salesLeadContactPersonDetail.LastName = item.LastName;
                        salesLeadContactPersonDetail.Email = item.Email;
                        salesLeadContactPersonDetail.Designation = item.Designation;
                        salesLeadContactPersonDetail.Mobile1 = item.Mobile1;
                        salesLeadContactPersonDetail.Mobile2 = item.Mobile2;
                        salesLeadContactPersonDetail.Linkedinurl = item.Linkedinurl;
                        salesLeadContactPersonDetail.IsActive = item.IsActive;

                        salesLeadAddEdit.ContactPersonDetailList.Add(salesLeadContactPersonDetail);
                    }

                    if (ContactPersonList.Count < 3)
                    {
                        int count = 3 - ContactPersonList.Count;
                        for (int i = 1; i <= count; i++)
                        {
                            SalesLeadContactPersonDetail salesLeadContactPersonDetail = new SalesLeadContactPersonDetail();
                            salesLeadAddEdit.ContactPersonDetailList.Add(salesLeadContactPersonDetail);
                        }
                    }
                }
                else
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        SalesLeadContactPersonDetail salesLeadContactPersonDetail = new SalesLeadContactPersonDetail();
                        salesLeadAddEdit.ContactPersonDetailList.Add(salesLeadContactPersonDetail);
                    }
                }
                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "Success!";
                commonResponse.Data = salesLeadAddEdit;
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return View(commonResponse);
        }

        [HttpPost]
        public async Task<CommonResponse> SaveUpdateSalesLead([FromBody] SaveUpdateSalesLeadReqModel saveUpdateSalesLeadReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                DateTime currentDateTime = _commonHelper.GetCurrentDateTime();
                int loggedInUserId = _commonHelper.GetLoggedInUserId();
                bool isEditMode = false;
                if (saveUpdateSalesLeadReqModel.saveUpdateSalesLeadContactPersonDetails.Count >= 1)
                {
                    var salesLeadList = _dbRepo.SalesLeadList();
                    bool isDuplicate = await salesLeadList.FirstOrDefaultAsync(x => x.OrgName.ToLower().Trim() == saveUpdateSalesLeadReqModel.saveUpdateSalesLeadDetailModel.OrgName.ToLower().Trim() && x.Id != saveUpdateSalesLeadReqModel.saveUpdateSalesLeadDetailModel.SalesLeadId) != null ? true : false;
                    if (!isDuplicate)
                    {
                        var salesLeadDetail = await salesLeadList.FirstOrDefaultAsync(x => x.Id == saveUpdateSalesLeadReqModel.saveUpdateSalesLeadDetailModel.SalesLeadId);
                        isEditMode = salesLeadDetail != null && salesLeadDetail.Id > 0 ? true : false;
                        using (TransactionScope transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            SalesLeadMst salesLeadMst = new SalesLeadMst();
                            List<SalesContactPersonMst> contactPersonMstList = new List<SalesContactPersonMst>();
                            if (isEditMode)
                            {
                                var contactPersonList = await _dbRepo.SalesContactPersonList().Where(x => x.SalesLeadId == salesLeadDetail.Id).ToListAsync();
                                salesLeadMst = salesLeadDetail;

                                _dbContext.SalesContactPersonMsts.RemoveRange(contactPersonList);
                                await _dbContext.SaveChangesAsync();
                            }
                            else
                            {
                                salesLeadMst.CreatedBy = loggedInUserId;
                                salesLeadMst.CreatedDate = currentDateTime;
                                salesLeadMst.IsDelete = false;
                            }
                            salesLeadMst.OrgName = saveUpdateSalesLeadReqModel.saveUpdateSalesLeadDetailModel.OrgName;
                            salesLeadMst.CountryId = saveUpdateSalesLeadReqModel.saveUpdateSalesLeadDetailModel.CountryId;
                            salesLeadMst.StateId = saveUpdateSalesLeadReqModel.saveUpdateSalesLeadDetailModel.StateId;
                            salesLeadMst.CityId = saveUpdateSalesLeadReqModel.saveUpdateSalesLeadDetailModel.CityId;
                            salesLeadMst.Address = saveUpdateSalesLeadReqModel.saveUpdateSalesLeadDetailModel.Address;
                            salesLeadMst.Phone1 = saveUpdateSalesLeadReqModel.saveUpdateSalesLeadDetailModel.Phone1;
                            salesLeadMst.Phone2 = saveUpdateSalesLeadReqModel.saveUpdateSalesLeadDetailModel.Phone2;
                            salesLeadMst.Email1 = saveUpdateSalesLeadReqModel.saveUpdateSalesLeadDetailModel.Email1;
                            salesLeadMst.Email2 = saveUpdateSalesLeadReqModel.saveUpdateSalesLeadDetailModel.Email2;
                            salesLeadMst.WebsiteUrl = saveUpdateSalesLeadReqModel.saveUpdateSalesLeadDetailModel.WebsiteUrl;
                            salesLeadMst.IsActive = saveUpdateSalesLeadReqModel.saveUpdateSalesLeadDetailModel.IsActive;
                            salesLeadMst.UpdatedBy = loggedInUserId;
                            salesLeadMst.UpdatedDate = currentDateTime;

                            if (isEditMode)
                            {
                                _dbContext.Entry(salesLeadMst).State = EntityState.Modified;
                            }
                            else
                            {
                                await _dbContext.SalesLeadMsts.AddAsync(salesLeadMst);
                            }
                            await _dbContext.SaveChangesAsync();

                            foreach (var item in saveUpdateSalesLeadReqModel.saveUpdateSalesLeadContactPersonDetails)
                            {
                                SalesContactPersonMst salesContactPersonMst = new SalesContactPersonMst();
                                salesContactPersonMst.SalesLeadId = salesLeadMst.Id;
                                salesContactPersonMst.FirstName = item.FirstName;
                                salesContactPersonMst.LastName = item.LastName;
                                salesContactPersonMst.Email = item.Email;
                                salesContactPersonMst.Designation = item.Designation;
                                salesContactPersonMst.Mobile1 = item.Mobile1;
                                salesContactPersonMst.Mobile2 = item.Mobile2;
                                salesContactPersonMst.Linkedinurl = item.Linkedinurl;
                                salesContactPersonMst.IsActive = item.IsActive;
                                salesContactPersonMst.IsDelete = false;
                                salesContactPersonMst.CreatedBy = loggedInUserId;
                                salesContactPersonMst.UpdatedBy = loggedInUserId;
                                salesContactPersonMst.CreatedDate = currentDateTime;
                                salesContactPersonMst.UpdatedDate = currentDateTime;

                                contactPersonMstList.Add(salesContactPersonMst);
                            }

                            await _dbContext.SalesContactPersonMsts.AddRangeAsync(contactPersonMstList);
                            await _dbContext.SaveChangesAsync();

                            transactionScope.Complete();
                            commonResponse.Data = salesLeadMst.Id;
                            commonResponse.Message = isEditMode ? "Data updated successfully!" : "Data saved successfully!";
                            commonResponse.Status = true;
                        }
                    }
                    else
                    {
                        commonResponse.Message = "Lead already exists!";
                        commonResponse.StatusCode = HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    commonResponse.Message = "Please enter atleast one contact person details!";
                    commonResponse.StatusCode = HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return commonResponse;
        }

        [HttpPost]
        public async Task<CommonResponse> DeleteSalesLead(int Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                if (Id > 0)
                {
                    var SalesLeadDetail = await _dbRepo.SalesLeadList().FirstOrDefaultAsync(x => x.Id == Id);
                    if (SalesLeadDetail != null)
                    {
                        SalesLeadDetail.IsActive = false;
                        SalesLeadDetail.IsDelete = true;
                        SalesLeadDetail.UpdatedDate = _commonHelper.GetCurrentDateTime();
                        SalesLeadDetail.UpdatedBy = 1;

                        _dbContext.Entry(SalesLeadDetail).State = EntityState.Modified;
                        _dbContext.SaveChanges();


                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Message = "SalesLead Deleted Successfully!";

                    }
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return commonResponse;
        }

        public CommonResponse SalesConatactPersonList()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                List<SalesConatactPersonListResViewModel> salesConatactPersonListResViewModels = new List<SalesConatactPersonListResViewModel>();
                salesConatactPersonListResViewModels = _dbRepo.SalesContactPersonList().Where(x => x.IsActive == true && x.IsDelete == false).Select(x => new SalesConatactPersonListResViewModel
                {
                    Id = x.Id,
                    SalesLeadId = x.SalesLeadId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Designation = x.Designation,
                    Mobile1 = x.Mobile1,
                    Mobile2 = x.Mobile2,
                    Linkedinurl = x.Linkedinurl,

                }).ToList();
                commonResponse.Data = salesConatactPersonListResViewModels;
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return View(commonResponse.Data);
        }

        public IActionResult Actions()
        {
            return View();
        }

        public async Task<IActionResult> ActionTaken()
        {
            CommonResponse commonResponse = new CommonResponse();
            var actionTakenList = await _dbRepo.SalesLeadActionList().Select(x => new
            {
                Id = x.Id,
                Action = x.SalesLeadActionName
            }).ToListAsync();
            commonResponse.Data = actionTakenList;
            commonResponse.Status = true;
            commonResponse.StatusCode = HttpStatusCode.OK;
            commonResponse.Message = "Action List Found";
            return Json(commonResponse);
        }

        public async Task<IActionResult> OrganizationStatus()
        {
            CommonResponse commonResponse = new CommonResponse();
            var actionTakenList = await _dbRepo.SalesLeadStatusList().Select(x => new
            {
                Id = x.Id,
                Status = x.SalesLeadStatusName
            }).ToListAsync();
            commonResponse.Data = actionTakenList;
            commonResponse.Status = true;
            commonResponse.StatusCode = HttpStatusCode.OK;
            commonResponse.Message = "Status List Found";
            return Json(commonResponse);
        }
        [HttpPost]
        public async Task<IActionResult> AddAction([FromBody] SalesLeadFollowUpReqModel salesLeadFollowUpReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    SalesLeadFollowUpMst salesLeadFollowUpMst = new SalesLeadFollowUpMst();
                    if (salesLeadFollowUpReqModel.NextFollowUpDate.Date >= salesLeadFollowUpReqModel.FollowUpDate.Date)
                    {
                        salesLeadFollowUpMst.SalesLeadId = salesLeadFollowUpReqModel.SalesLeadId;
                        salesLeadFollowUpMst.SalesContactPersonId = salesLeadFollowUpReqModel.SalesContactPersonId;
                        salesLeadFollowUpMst.FollowUpDateTime = salesLeadFollowUpReqModel.FollowUpDate;
                        salesLeadFollowUpMst.NextFollowUpDateTime = salesLeadFollowUpReqModel.NextFollowUpDate;
                        salesLeadFollowUpMst.SalesLeadActionId = salesLeadFollowUpReqModel.SalesLeadActionId;
                        salesLeadFollowUpMst.NextFollowUpActionId = salesLeadFollowUpReqModel.SalesLeadNextActionId;
                        salesLeadFollowUpMst.Notes = salesLeadFollowUpReqModel.Notes;
                        salesLeadFollowUpMst.NextFollowUpNotes = salesLeadFollowUpReqModel.NextFollowUpNotes;
                        salesLeadFollowUpMst.SalesLeadStatusId = salesLeadFollowUpReqModel.SalesLeadStatusId;
                        salesLeadFollowUpMst.IsActive = true;
                        salesLeadFollowUpMst.IsDelete = false;
                        salesLeadFollowUpMst.CreatedDate = _commonHelper.GetCurrentDateTime();
                        salesLeadFollowUpMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
                        salesLeadFollowUpMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                        salesLeadFollowUpMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                        _dbContext.SalesLeadFollowUpMsts.Add(salesLeadFollowUpMst);
                        await _dbContext.SaveChangesAsync();
                        if (salesLeadFollowUpReqModel.SalesLeadStatusId == 5)
                        {
                            bool result = await ConvertToOppertunity(salesLeadFollowUpReqModel.SalesLeadId);
                            if (result)
                            {
                                transactionScope.Complete();
                            }
                        }
                        else
                        {
                            transactionScope.Complete();
                        }
                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Message = "FollowUp Action Added Successfully";
                    }
                    else
                    {
                        commonResponse.Message = "Please valid enter the next followup date";
                    }

                }
                catch (Exception ex)
                {
                    commonResponse.Message = ex.Message.ToString();
                    commonResponse.Data = ex.ToString();
                }
            }
            return Json(commonResponse);
        }
        [HttpGet]
        public async Task<IActionResult> LeadNContactPersonDetails(int id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var leadNContactPersonDetail = await (from contactPerson in _dbRepo.SalesContactPersonList()
                                                      where contactPerson.Id == id
                                                      join leadDetail in _dbRepo.SalesLeadList() on contactPerson.SalesLeadId equals leadDetail.Id
                                                      join country in _dbRepo.CountryList() on leadDetail.CountryId equals country.CountryId
                                                      select new { contactPerson, leadDetail, country }
                                                ).Select(x => new
                                                {
                                                    LeadId = x.leadDetail.Id,
                                                    LeadName = x.leadDetail.OrgName,
                                                    Website = x.leadDetail.WebsiteUrl,
                                                    OfficePhone = x.leadDetail.Phone1,
                                                    Speciality = "-",
                                                    Country = x.country.CountryName,
                                                    ContactPersonId = x.contactPerson.Id,
                                                    ContactPersonName = $"{x.contactPerson.FirstName} {x.contactPerson.LastName}",
                                                    Designation = x.contactPerson.Designation,
                                                    Mobile = x.contactPerson.Mobile1,
                                                    Email = x.contactPerson.Email
                                                }).FirstOrDefaultAsync();
                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "Record found";
                commonResponse.Data = leadNContactPersonDetail;
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message.ToString();
                commonResponse.Data = ex.ToString();
            }
            return Json(commonResponse);
        }

        [HttpGet]
        public async Task<IActionResult> SalesLeadFollowUpList(int id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var salesLeadFollowUpList = (from followUp in _dbRepo.salesLeadFollowUpMst().ToList()
                                             where followUp.SalesContactPersonId == id
                                             join followupAction in _dbRepo.SalesLeadActionList().ToList() on followUp.SalesLeadActionId equals followupAction.Id
                                             join nextfolloupaction in _dbRepo.SalesLeadActionList().ToList() on followUp.NextFollowUpActionId equals nextfolloupaction.Id
                                             join leadStatus in _dbRepo.SalesLeadStatusList().ToList() on followUp.SalesLeadStatusId equals leadStatus.Id
                                             join lead in _dbRepo.SalesLeadList().ToList() on followUp.SalesLeadId equals lead.Id
                                             join contactPerson in _dbRepo.SalesContactPersonList().ToList() on followUp.SalesContactPersonId equals contactPerson.Id
                                             select new { followUp, followupAction, nextfolloupaction, leadStatus, lead, contactPerson }
                                                  ).Select(x => new
                                                  {

                                                      Id = x.followUp.Id,
                                                      FollowUpDate = x.followUp.FollowUpDateTime,
                                                      NextFollowUpDate = x.followUp.NextFollowUpDateTime,
                                                      Action = x.followupAction.SalesLeadActionName,
                                                      NextAction = x.nextfolloupaction.SalesLeadActionName,
                                                      Status = x.leadStatus.SalesLeadStatusName,
                                                      Organization = x.lead.OrgName,
                                                      ContactPerson = $"{x.contactPerson.FirstName} {x.contactPerson.LastName}"

                                                  }).ToList();
                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "Record found";
                commonResponse.Data = salesLeadFollowUpList;
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message.ToString();
                commonResponse.Data = ex.ToString();
            }
            return Json(commonResponse);
        }

        public async Task<IActionResult> DeleteFollowUpAction(int id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var IsFollowUpActionExists = await _dbRepo.salesLeadFollowUpMst().FirstOrDefaultAsync(x => x.Id == id);
                if (IsFollowUpActionExists != null)
                {
                    IsFollowUpActionExists.IsDelete = true;
                    _dbContext.SalesLeadFollowUpMsts.Update(IsFollowUpActionExists);
                    await _dbContext.SaveChangesAsync();
                    commonResponse.Status = true;
                    commonResponse.StatusCode = HttpStatusCode.OK;
                    commonResponse.Message = "SalesLead Follow Up Action Deleted Successfully.";
                }
                else
                {
                    commonResponse.Message = "Fail To Delete SalesLead Follow Up Action";
                }

            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message.ToString();
                commonResponse.Data = ex.Data;
            }
            return Json(commonResponse);
        }

        public async Task<IActionResult> EditFollowUpAction(int id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var IsFollowUpActionExists = await _dbRepo.salesLeadFollowUpMst().FirstOrDefaultAsync(x => x.Id == id);
                if (IsFollowUpActionExists != null)
                {
                    commonResponse.Status = true;
                    commonResponse.StatusCode = HttpStatusCode.OK;
                    commonResponse.Message = "SalesLead Follow Up Action Found";
                    commonResponse.Data = IsFollowUpActionExists;
                }
                else
                {
                    commonResponse.Message = "SalesLead Follow Up Action Not Found";
                }

            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message.ToString();
                commonResponse.Data = ex.Data;
            }
            return Json(commonResponse);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAction([FromBody] SalesLeadFollowUpEditReqModel salesLeadFollowUpEditReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (salesLeadFollowUpEditReqModel.NextFollowUpDate.Date >= salesLeadFollowUpEditReqModel.FollowUpDate.Date)
                    {
                        var isFolloUpExits = await _dbRepo.salesLeadFollowUpMst().FirstOrDefaultAsync(x => x.Id == salesLeadFollowUpEditReqModel.FollowUpId);
                        if (isFolloUpExits != null)
                        {
                            isFolloUpExits.FollowUpDateTime = salesLeadFollowUpEditReqModel.FollowUpDate;
                            isFolloUpExits.NextFollowUpDateTime = salesLeadFollowUpEditReqModel.NextFollowUpDate;
                            isFolloUpExits.SalesLeadActionId = salesLeadFollowUpEditReqModel.SalesLeadActionId;
                            isFolloUpExits.NextFollowUpActionId = salesLeadFollowUpEditReqModel.SalesLeadNextActionId;
                            isFolloUpExits.Notes = salesLeadFollowUpEditReqModel.Notes;
                            isFolloUpExits.NextFollowUpNotes = salesLeadFollowUpEditReqModel.NextFollowUpNotes;
                            isFolloUpExits.SalesLeadStatusId = salesLeadFollowUpEditReqModel.SalesLeadStatusId;
                            isFolloUpExits.UpdatedDate = _commonHelper.GetCurrentDateTime();
                            isFolloUpExits.UpdatedBy = _commonHelper.GetLoggedInUserId();
                            _dbContext.SalesLeadFollowUpMsts.Update(isFolloUpExits);
                            await _dbContext.SaveChangesAsync();
                            commonResponse.Status = true;
                            commonResponse.StatusCode = HttpStatusCode.OK;
                            commonResponse.Message = "FollowUp Action Updated Successfully";
                            if (salesLeadFollowUpEditReqModel.SalesLeadStatusId == 5)
                            {
                                bool result = await ConvertToOppertunity(isFolloUpExits.SalesLeadId);
                                if (result)
                                {
                                    transactionScope.Complete();
                                }

                            }
                            else
                            {
                                transactionScope.Complete();
                            }
                        }
                        else
                        {
                            commonResponse.Message = "FollowUp Action Not Found";
                        }
                    }
                    else
                    {
                        commonResponse.Message = "Please valid enter the next followup date";
                    }
                }
                catch (Exception ex)
                {
                    commonResponse.Message = ex.Message.ToString();
                    commonResponse.Data = ex.ToString();
                }
            }
            return Json(commonResponse);
        }

        public IActionResult Countries()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var countries = _dbRepo.CountryList().Select(x => new
                {
                    Id = x.CountryId,
                    CountryName = x.CountryName

                }).ToList();
                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "Country List Found";
                commonResponse.Data = countries;
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message.ToString();
                commonResponse.Data = ex.ToString();
            }
            return Json(commonResponse);
        }
        public async Task<IActionResult> States(int id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var states = await _dbRepo.StateList().Where(x => x.CountryId == id).Select(x => new
                {
                    Id = x.StateId,
                    StateName = x.StateName

                }).ToListAsync();
                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "State List Found";
                commonResponse.Data = states;
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message.ToString();
                commonResponse.Data = ex.ToString();
            }
            return Json(commonResponse);
        }
        public async Task<IActionResult> Cities(int id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var cities = await _dbRepo.CityList().Where(x => x.StateId == id).Select(x => new
                {
                    Id = x.CityId,
                    CityName = x.CityName

                }).ToListAsync();
                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "City List Found";
                commonResponse.Data = cities;
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message.ToString();
                commonResponse.Data = ex.ToString();
            }
            return Json(commonResponse);
        }

        public async Task<bool> ConvertToOppertunity(int SalesLeadId)
        {
            bool result = false;
            try
            {
                var salesLeadDetail = await (from salesLead in _dbRepo.SalesLeadList()
                                             where salesLead.Id == SalesLeadId
                                             join salesContactPerson in _dbRepo.SalesContactPersonList() on salesLead.Id equals salesContactPerson.SalesLeadId
                                             select new { salesLead, salesContactPerson }
                                             ).ToListAsync();
                if (salesLeadDetail != null)
                {
                    ClientMst clientMst = new ClientMst();
                    clientMst.ClientName = salesLeadDetail[0].salesLead.OrgName;
                    clientMst.ClientCode = salesLeadDetail[0].salesLead.OrgName;
                    clientMst.CompanyId = _dbRepo.GetLoggedInUserDetails() != null ? _dbRepo.GetLoggedInUserDetails().CompanyId : 1;
                    clientMst.EmailId = salesLeadDetail[0].salesLead.Email1;
                    clientMst.MobileNo = salesLeadDetail[0].salesLead.Phone1;
                    clientMst.PocnamePrimary = $"{salesLeadDetail[0].salesContactPerson.FirstName} {salesLeadDetail[0].salesContactPerson.LastName}";
                    clientMst.PocnumberPrimary = salesLeadDetail[0].salesContactPerson.Mobile1;
                    clientMst.PocemailPrimary = salesLeadDetail[0].salesContactPerson.Email;
                    if (salesLeadDetail.Count > 1)
                    {
                        clientMst.PocnameSecondary = $"{salesLeadDetail[1].salesContactPerson.FirstName} {salesLeadDetail[1].salesContactPerson.LastName}";
                        clientMst.PocnumberSecondary = salesLeadDetail[1].salesContactPerson.Mobile1;
                        clientMst.PocemailSecondary = salesLeadDetail[1].salesContactPerson.Email;
                    }
                    clientMst.IsActive = true;
                    clientMst.IsDelete = false;
                    clientMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                    clientMst.CreatedDate = _commonHelper.GetCurrentDateTime();
                    clientMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                    clientMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
                    _dbContext.ClientMsts.Add(clientMst);
                    await _dbContext.SaveChangesAsync();
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        #region UploadSalesSheet

        [HttpPost]
        public async Task<IActionResult> UploadSalesSheet(UploadSalesSheetReqModel uploadSalesSheetReqModel)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                IExcelDataReader reader;
                // string dataFileName = System.IO.Path.GetFileName(uploadSalesSheetReqModel.UserSheet.FileName);
                var countrylist = _dbRepo.CountryList().ToList();
                var statelist = _dbRepo.StateList().ToList();
                // var cityist = _dbRepo.CityList().ToList();

                string extension = System.IO.Path.GetExtension(uploadSalesSheetReqModel.SalesSheet.FileName);
                Stream stream = uploadSalesSheetReqModel.SalesSheet.OpenReadStream();
                MemoryStream ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                ms.Position = 0;
                using (ms)
                {
                    if (extension == ".xls")
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    DataSet ds = new DataSet();
                    ds = reader.AsDataSet();

                    reader.Close();
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            // Read the the Table
                            System.Data.DataTable salesDataTable = ds.Tables[0];


                            string OrganizationName = !string.IsNullOrEmpty(salesDataTable.Rows[1][0].ToString()) ? salesDataTable.Rows[1][0].ToString() : "";
                            string Country = !string.IsNullOrEmpty(salesDataTable.Rows[1][1].ToString()) ? salesDataTable.Rows[1][1].ToString() : "";
                            string State = !string.IsNullOrEmpty(salesDataTable.Rows[1][2].ToString()) ? salesDataTable.Rows[1][2].ToString() : "";
                            string City = !string.IsNullOrEmpty(salesDataTable.Rows[1][3].ToString()) ? salesDataTable.Rows[1][3].ToString() : "";
                            string SalesLeadAddress = !string.IsNullOrEmpty(salesDataTable.Rows[1][4].ToString()) ? salesDataTable.Rows[1][4].ToString() : "";
                            string Phone1 = !string.IsNullOrEmpty(salesDataTable.Rows[1][5].ToString()) ? salesDataTable.Rows[1][5].ToString() : "";
                            string Phone2 = !string.IsNullOrEmpty(salesDataTable.Rows[1][6].ToString()) ? salesDataTable.Rows[1][6].ToString() : "";
                            string Email1 = !string.IsNullOrEmpty(salesDataTable.Rows[1][7].ToString()) ? salesDataTable.Rows[1][7].ToString() : "";
                            string Email2 = !string.IsNullOrEmpty(salesDataTable.Rows[1][8].ToString()) ? salesDataTable.Rows[1][8].ToString() : "";
                            string Website = !string.IsNullOrEmpty(salesDataTable.Rows[1][9].ToString()) ? salesDataTable.Rows[1][9].ToString() : "";

                            string FirstName = !string.IsNullOrEmpty(salesDataTable.Rows[1][10].ToString()) ? salesDataTable.Rows[1][10].ToString() : "";
                            string LastName = !string.IsNullOrEmpty(salesDataTable.Rows[1][11].ToString()) ? salesDataTable.Rows[1][11].ToString() : "";
                            string Email = !string.IsNullOrEmpty(salesDataTable.Rows[1][12].ToString()) ? salesDataTable.Rows[1][12].ToString() : "";
                            string Designation = !string.IsNullOrEmpty(salesDataTable.Rows[1][13].ToString()) ? salesDataTable.Rows[1][13].ToString() : "";
                            string Mobile1 = !string.IsNullOrEmpty(salesDataTable.Rows[1][14].ToString()) ? salesDataTable.Rows[1][14].ToString() : "";
                            string Linkedinurl = !string.IsNullOrEmpty(salesDataTable.Rows[1][15].ToString()) ? salesDataTable.Rows[1][15].ToString() : "";



                            if (OrganizationName.ToLower() == "organizationname" && Country.ToLower() == "country" && State.ToLower() == "state" && City.ToLower() == "city" && SalesLeadAddress.ToLower() == "salesleadaddress" && Phone1.ToLower() == "phone1" && Phone2.ToLower() == "phone2" && Email1.ToLower() == "email1" && Email2.ToLower() == "email2" && Website.ToLower() == "website" && FirstName.ToLower() == "firstname" && LastName.ToLower() == "lastname" && Email.ToLower() == "email" && Designation.ToLower() == "designation" && Mobile1.ToLower() == "mobile1" && Linkedinurl.ToLower() == "linkedinurl")
                            {
                                SalesLeadMst salesLeadMst = new SalesLeadMst();
                                List<SalesLeadMst> salesLeadslist = new List<SalesLeadMst>();
                                List<SalesContactPersonMst> SalesContactPerson = new List<SalesContactPersonMst>();
                                for (int i = 2; i < salesDataTable.Rows.Count; i++)
                                {
                                    OrganizationName = !string.IsNullOrEmpty(salesDataTable.Rows[i][0].ToString()) ? salesDataTable.Rows[i][0].ToString() : "";
                                    Email = !string.IsNullOrEmpty(salesDataTable.Rows[i][7].ToString()) ? salesDataTable.Rows[i][7].ToString() : "";
                                    Phone1 = !string.IsNullOrEmpty(salesDataTable.Rows[i][5].ToString()) ? salesDataTable.Rows[i][5].ToString() : "";
                                    if (OrganizationName != null && OrganizationName != "" || Email != null && Email != "" || Phone1 != null && Phone1 != "")
                                    {
                                        var duplicateCheck = await _dbRepo.SalesLeadList().Where(x => x.OrgName == OrganizationName || x.Email1 == Email || x.Phone1 == Phone1).ToListAsync();
                                        if (duplicateCheck.Count == 0)
                                        {

                                            string countryName = !string.IsNullOrEmpty(salesDataTable.Rows[i][1].ToString()) ? salesDataTable.Rows[i][1].ToString() : "";
                                            string stateName = !string.IsNullOrEmpty(salesDataTable.Rows[i][2].ToString()) ? salesDataTable.Rows[i][2].ToString() : "";
                                            string cityName = !string.IsNullOrEmpty(salesDataTable.Rows[i][3].ToString()) ? salesDataTable.Rows[i][3].ToString() : "";

                                            countryName = countryName.Replace("_", " ");
                                            stateName = stateName.Replace("_", " ");
                                            cityName = cityName.Replace("_", " ");

                                            int countryId = (int)countrylist.FirstOrDefault(x => x.CountryName == countryName).CountryId;
                                            int stateId = (int)statelist.FirstOrDefault(x => x.StateName == stateName).StateId;
                                            int cityId = (int)_dbRepo.CityList().FirstOrDefault(x => x.CityName == cityName).CityId;




                                            if (!string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][0])) && Convert.ToString(salesDataTable.Rows[i][0]).All(char.IsDigit) || !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][5])) && Convert.ToString(salesDataTable.Rows[i][5]).All(char.IsDigit) || !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][7])) && Convert.ToString(salesDataTable.Rows[i][7]).All(char.IsDigit) || !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][10])) && Convert.ToString(salesDataTable.Rows[i][10]).All(char.IsDigit) || !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][11])) && Convert.ToString(salesDataTable.Rows[i][11]).All(char.IsDigit) || !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][12])) && Convert.ToString(salesDataTable.Rows[i][12]).All(char.IsDigit) || !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][13])) && Convert.ToString(salesDataTable.Rows[i][13]).All(char.IsDigit) || !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][14])) && Convert.ToString(salesDataTable.Rows[i][14]).All(char.IsDigit))
                                            {
                                                salesLeadMst = new SalesLeadMst();

                                                salesLeadMst.CountryId = countryId > 0 ? countryId : 0;
                                                salesLeadMst.StateId = stateId > 0 ? stateId : 0;
                                                salesLeadMst.CityId = cityId > 0 ? cityId : 0;
                                                salesLeadMst.OrgName = !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][0])) ? Convert.ToString(salesDataTable.Rows[i][0]) : "";
                                                salesLeadMst.Address = !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][4])) ? Convert.ToString(salesDataTable.Rows[i][4]) : "";
                                                salesLeadMst.Phone1 = !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][5])) ? Convert.ToString(salesDataTable.Rows[i][5]) : "";
                                                salesLeadMst.Phone2 = !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][6])) ? Convert.ToString(salesDataTable.Rows[i][6]) : "";
                                                salesLeadMst.Email1 = !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][7])) ? Convert.ToString(salesDataTable.Rows[i][7]) : "";
                                                salesLeadMst.Email2 = !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][8])) ? Convert.ToString(salesDataTable.Rows[i][8]) : "";
                                                salesLeadMst.WebsiteUrl = !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][9])) ? Convert.ToString(salesDataTable.Rows[i][9]) : "";
                                                salesLeadMst.IsActive = true;
                                                salesLeadMst.IsDelete = false;
                                                salesLeadMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                                                salesLeadMst.CreatedDate = _commonHelper.GetCurrentDateTime();
                                                salesLeadMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                                                salesLeadMst.UpdatedDate = _commonHelper.GetCurrentDateTime();

                                                await _dbContext.SalesLeadMsts.AddAsync(salesLeadMst);
                                                await _dbContext.SaveChangesAsync();

                                                SalesContactPerson.Add(new SalesContactPersonMst
                                                {
                                                    SalesLeadId = salesLeadMst.Id,
                                                    FirstName = !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][10])) ? Convert.ToString(salesDataTable.Rows[i][10]) : "",
                                                    LastName = !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][11])) ? Convert.ToString(salesDataTable.Rows[i][11]) : "",
                                                    Email = !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][12])) ? Convert.ToString(salesDataTable.Rows[i][12]) : "",
                                                    Designation = !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][13])) ? Convert.ToString(salesDataTable.Rows[i][13]) : "",
                                                    Mobile1 = !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][14])) ? Convert.ToString(salesDataTable.Rows[i][14]) : "",
                                                    Linkedinurl = !string.IsNullOrEmpty(Convert.ToString(salesDataTable.Rows[i][15])) ? Convert.ToString(salesDataTable.Rows[i][15]) : "",
                                                    IsActive = true,
                                                    IsDelete = false,
                                                    CreatedBy = _commonHelper.GetLoggedInUserId(),
                                                    CreatedDate = _commonHelper.GetCurrentDateTime(),
                                                    UpdatedBy = _commonHelper.GetLoggedInUserId(),
                                                    UpdatedDate = _commonHelper.GetCurrentDateTime(),
                                                });

                                            }
                                            else
                                            {
                                                response.Message = "Please Enter Valid Excel Data";
                                            }
                                        }
                                        else
                                        {

                                            response.Message = "OrganizationName, Email OR Mobile Already Exist";
                                        }
                                    }
                                    else
                                    {
                                        response.Message = "No records found";
                                    }

                                }
                                if (SalesContactPerson.Count > 0)
                                {
                                    await _dbContext.SalesContactPersonMsts.AddRangeAsync(SalesContactPerson);
                                    await _dbContext.SaveChangesAsync();
                                    scope.Complete();
                                    response.Status = true;
                                    response.StatusCode = HttpStatusCode.OK;
                                    response.Data = SalesContactPerson.Select(x => x.SalesLeadId).ToList();
                                    response.Message = "Sales sheet uploaded successfully";
                                }
                            }
                            else
                            {
                                response.Message = "Please Enter Valid Excel";
                            }
                        }
                    }
                    else
                    {
                        response.Message = "No records found";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Data = ex;
            }
            return Json(response);
        }
        #endregion
    }
}
