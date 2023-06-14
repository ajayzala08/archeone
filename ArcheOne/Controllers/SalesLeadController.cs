using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

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
        public async Task<IActionResult> Sales()
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
        public async Task<IActionResult> AddEditSalesLead(int Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                SalesLeadAddEditReqModel salesLeadAddEdit = new SalesLeadAddEditReqModel();
                salesLeadAddEdit.salesLeadDetail = new SalesLeadDetail();
                salesLeadAddEdit.SalesLeadContactPersonDetail = new SalesLeadContactPersonDetail();
                List<SalesLeadContactPersonDetail> salesLeadContactPersonList = new List<SalesLeadContactPersonDetail>();
                // salesLeadAddEdit.CountryList = await _dbRepo.GetCountryList();
                if (Id > 0)
                {
                    var SalesLeadDetails = await _dbRepo.SalesLeadList().FirstOrDefaultAsync(x => x.Id == Id);
                    if (SalesLeadDetails != null)
                    {
                        salesLeadAddEdit.salesLeadDetail = new SalesLeadDetail();
                        salesLeadAddEdit.salesLeadDetail.Id = SalesLeadDetails.Id;
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

                    }
                }
                salesLeadContactPersonList = _dbRepo.SalesContactPersonList().Where(x => x.IsActive == true && x.IsDelete == false).Select(x => new SalesLeadContactPersonDetail
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

                salesLeadAddEdit.SalesLeadContactPersonList = salesLeadContactPersonList;

                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "Success!";
                commonResponse.Data = salesLeadAddEdit;
            }
            catch { throw; }
            return View(commonResponse.Data);
        }

        [HttpPost]
        public async Task<CommonResponse> SaveUpdateSalesLead([FromBody] SaveUpdateSalesLeadReqModel saveUpdateSalesLeadReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();

            SalesLeadMst salesLeadMst = new SalesLeadMst();
            var saleslist = await _dbRepo.SalesLeadList().ToListAsync();
            var duplicateCheck = saleslist.Any(x => x.OrgName == saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.OrgName && x.Id != saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.Id);

            if (!duplicateCheck)
            {
                var salesLeadDetail = saleslist.FirstOrDefault(x => x.Id == saveUpdateSalesLeadReqModel.SaveUpdateSalesLeadDetails.Id);
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
                    salesLeadDetail.UpdatedDate = _commonHelper.GetCurrentDateTime();
                    salesLeadDetail.UpdatedBy = _commonHelper.GetLoggedInUserId();

                    _dbContext.Entry(salesLeadDetail).State = EntityState.Modified;
                    _dbContext.SaveChanges();

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
                    salesLeadMst.CreatedDate = _commonHelper.GetCurrentDateTime();
                    salesLeadMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
                    salesLeadMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                    salesLeadMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                    salesLeadMst.IsActive = true;
                    salesLeadMst.IsDelete = false;
                    _dbContext.SalesLeadMsts.Add(salesLeadMst);
                    _dbContext.SaveChanges();


                    commonResponse.Status = true;
                    commonResponse.StatusCode = HttpStatusCode.OK;
                    commonResponse.Message = "SalesLead Added Successfully!";
                }
            }
            else
            {
                commonResponse.Message = "Organization Name Already Exist";
            }
            commonResponse.Data = salesLeadMst;

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
