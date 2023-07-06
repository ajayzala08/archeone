﻿using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

                var salesContactPersonList = (from salesLeadList in _dbRepo.SalesLeadList()
                                              join salescontactPersonList in _dbRepo.SalesContactPersonList()
                                              on salesLeadList.Id equals salescontactPersonList.SalesLeadId
                                              select new { salesLeadList, salescontactPersonList }
                                                   ).Select(x => new
                                                   {
                                                       Id = x.salesLeadList.Id,
                                                       OrgName = x.salesLeadList.OrgName,
                                                       ContactPersonId = x.salescontactPersonList.Id,
                                                       FullName = $"{x.salescontactPersonList.FirstName} {x.salescontactPersonList.LastName}",
                                                       Mobile = x.salescontactPersonList.Mobile1,
                                                       Email = x.salescontactPersonList.Email,
                                                       Designation = x.salescontactPersonList.Designation

                                                   }).ToList();
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
            var actionTakenList = _dbRepo.SalesLeadActionList().Select(x => new
            {
                Id = x.Id,
                Action = x.SalesLeadActionName
            }).ToList();
            commonResponse.Data = actionTakenList;
            commonResponse.Status = true;
            commonResponse.StatusCode = HttpStatusCode.OK;
            commonResponse.Message = "Action List Found";
            return Json(commonResponse);
        }

        public async Task<IActionResult> OrganizationStatus()
        {
            CommonResponse commonResponse = new CommonResponse();
            var actionTakenList = _dbRepo.SalesLeadStatusList().Select(x => new
            {
                Id = x.Id,
                Status = x.SalesLeadStatusName
            }).ToList();
            commonResponse.Data = actionTakenList;
            commonResponse.Status = true;
            commonResponse.StatusCode = HttpStatusCode.OK;
            commonResponse.Message = "Status List Found";
            return Json(commonResponse);
        }
        [HttpPost]
        public async Task<IActionResult> AddAction()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {


            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message.ToString();
                commonResponse.Data = ex.ToString();
            }
            return Json(commonResponse);
        }
    }
}
