using ArcheOne.Database.Entities;
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

        [HttpGet]
        public IActionResult Sales()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SalesList()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                List<SalesLeadListResViewModel> salesLeadListResViewModels = new List<SalesLeadListResViewModel>();
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
                commonResponse.Data = salesLeadListResViewModels;
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return View(commonResponse.Data);
        }

        [HttpGet]
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
                if (!ModelState.IsValid)
                {
                    DateTime date = _commonHelper.GetCurrentDateTime();
                    int LoggedInUserId = _commonHelper.GetLoggedInUserId();
                    SalesLeadMst salesLeadMst = new SalesLeadMst();
                    List<SalesContactPersonMst> salesContactPersonMstList = new List<SalesContactPersonMst>();

                    var salesList = await _dbRepo.SalesLeadList().ToListAsync();
                    var salesContactPersonList = await _dbRepo.SalesContactPersonList().ToListAsync();

                    var duplicateCheck = salesList.Any(x => x.OrgName == saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.OrgName && x.Id != saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.Id);

                    using (TransactionScope transactionScope1 = new TransactionScope())
                    {
                        if (!duplicateCheck)
                        {
                            var salesLeadDetail = salesList.FirstOrDefault(x => x.Id == saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.Id);
                            if (salesLeadDetail != null)
                            {

                                //Edit Mode
                                //salesLeadMst.Id = saveUpdateSalesLeadReqModel.Id;
                                salesLeadDetail.Address = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.Address;
                                salesLeadDetail.OrgName = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.OrgName;
                                salesLeadDetail.CountryId = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.CountryId;
                                salesLeadDetail.StateId = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.StateId;
                                salesLeadDetail.CityId = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.CityId;
                                salesLeadDetail.Phone1 = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.Phone1;
                                salesLeadDetail.Phone2 = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.Phone2;
                                salesLeadDetail.Email1 = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.Email1;
                                salesLeadDetail.Email2 = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.Email2;
                                salesLeadDetail.WebsiteUrl = @saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.WebsiteUrl;
                                salesLeadDetail.UpdatedDate = date;
                                salesLeadDetail.UpdatedBy = LoggedInUserId;

                                _dbContext.Entry(salesLeadDetail).State = EntityState.Modified;
                                _dbContext.SaveChanges();



                                #region Edit SalesContactPerson
                                var SalesLeadContactPersonList = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadContactPersonList;

                                foreach (var item in SalesLeadContactPersonList)
                                {
                                    if (item.SalesLeadContactPersonId > 0)
                                    {

                                        var salesContactPersonDetail = salesContactPersonList.FirstOrDefault(x => x.Id == item.SalesLeadContactPersonId);


                                        salesContactPersonDetail.SalesLeadId = salesContactPersonDetail.SalesLeadId;
                                        salesContactPersonDetail.FirstName = item.FirstName;
                                        salesContactPersonDetail.LastName = item.LastName;
                                        salesContactPersonDetail.Email = item.Email;
                                        salesContactPersonDetail.Designation = item.Designation;
                                        salesContactPersonDetail.Mobile1 = item.Mobile1;
                                        salesContactPersonDetail.Mobile2 = item.Mobile2;
                                        salesContactPersonDetail.Linkedinurl = item.Linkedinurl;
                                        salesContactPersonDetail.UpdatedDate = _commonHelper.GetCurrentDateTime();
                                        salesContactPersonDetail.UpdatedBy = _commonHelper.GetLoggedInUserId();

                                        _dbContext.Entry(salesContactPersonDetail).State = EntityState.Modified;
                                        _dbContext.SaveChanges();

                                    }
                                }
                                #endregion

                                transactionScope1.Complete();
                                commonResponse.Status = true;
                                commonResponse.StatusCode = HttpStatusCode.OK;
                                commonResponse.Message = "SalesLead Updated Successfully!";
                            }
                            else
                            {
                                //Add Mode
                                salesLeadMst.Address = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.Address;
                                salesLeadMst.OrgName = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.OrgName;
                                salesLeadMst.CountryId = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.CountryId;
                                salesLeadMst.StateId = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.StateId;
                                salesLeadMst.CityId = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.CityId;
                                salesLeadMst.Phone1 = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.Phone1;
                                salesLeadMst.Phone2 = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.Phone2;
                                salesLeadMst.Email1 = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.Email1;
                                salesLeadMst.Email2 = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.Email2;
                                salesLeadMst.WebsiteUrl = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.WebsiteUrl;
                                salesLeadMst.CreatedDate = date;
                                salesLeadMst.UpdatedDate = date;
                                salesLeadMst.CreatedBy = LoggedInUserId;
                                salesLeadMst.UpdatedBy = LoggedInUserId;
                                salesLeadMst.IsActive = true;
                                salesLeadMst.IsDelete = false;
                                await _dbContext.SalesLeadMsts.AddAsync(salesLeadMst);
                                _dbContext.SaveChanges();

                                #region ADD SalesContactPerson 

                                var SalesLeadContactPersonList = saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadContactPersonList;

                                foreach (var item in SalesLeadContactPersonList)
                                {
                                    if (!string.IsNullOrWhiteSpace(item.FirstName))
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
                                        salesContactPersonMst.CreatedDate = _commonHelper.GetCurrentDateTime();
                                        salesContactPersonMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
                                        salesContactPersonMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                                        salesContactPersonMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                                        salesContactPersonMst.IsActive = true;
                                        salesContactPersonMst.IsDelete = false;
                                        salesContactPersonMstList.Add(salesContactPersonMst);
                                    }
                                }

                                await _dbContext.SalesContactPersonMsts.AddRangeAsync(salesContactPersonMstList);
                                _dbContext.SaveChanges();
                                #endregion


                                transactionScope1.Complete();
                                commonResponse.Status = true;
                                commonResponse.StatusCode = HttpStatusCode.OK;
                                commonResponse.Message = "SalesLead Added Successfully!";
                            }
                        }
                        else
                        {
                            transactionScope1.Dispose();
                            commonResponse.Message = "Organization Name Already Exist";
                        }
                    }
                    commonResponse.Data = salesLeadMst;
                }
            }
            catch (Exception e)
            {
                commonResponse.Message = e.Message;
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
            catch { throw; }
            return commonResponse;
        }
        [HttpGet]
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

    }
}
