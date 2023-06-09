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
    public class SalesController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly ArcheOneDbContext _dbContext;
        private readonly CommonHelper _commonHelper;
        public SalesController(DbRepo dbRepo, ArcheOneDbContext dbContext, CommonHelper commonHelper)
        {
            _dbRepo = dbRepo;
            _dbContext = dbContext;
            _commonHelper = commonHelper;
        }
        public IActionResult Sales()
        {
            return View();
        }

        public IActionResult SalesList()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                List<SalesLeadListResViewModel> salesLeadListResViewModels = new List<SalesLeadListResViewModel>();
                salesLeadListResViewModels = _dbRepo.SalesLeadList().Where(x => x.IsActive == true && x.IsDelete == false).Select(x => new SalesLeadListResViewModel
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
                }).ToList();
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
                SalesLeadAddEditReqViewModel salesLeadAddEdit = new SalesLeadAddEditReqViewModel();
                salesLeadAddEdit.SalesLeadDetailes = new SalesLeadDetailes();
                salesLeadAddEdit.SalesLeadContactPersonDetailes = new SalesLeadContactPersonDetailes();
                // salesLeadAddEdit.CountryList = await _dbRepo.GetCountryList();
                if (Id > 0)
                {
                    var SalesLeadDetails = await _dbRepo.SalesLeadList().FirstOrDefaultAsync(x => x.Id == Id);
                    if (SalesLeadDetails != null)
                    {
                        salesLeadAddEdit.SalesLeadDetailes = new SalesLeadDetailes();
                        salesLeadAddEdit.SalesLeadDetailes.Id = SalesLeadDetails.Id;
                        salesLeadAddEdit.SalesLeadDetailes.OrgName = SalesLeadDetails.OrgName;
                        salesLeadAddEdit.SalesLeadDetailes.CountryId = SalesLeadDetails.CountryId;
                        salesLeadAddEdit.SalesLeadDetailes.StateId = SalesLeadDetails.StateId;
                        salesLeadAddEdit.SalesLeadDetailes.CityId = SalesLeadDetails.CityId;
                        salesLeadAddEdit.SalesLeadDetailes.Address = SalesLeadDetails.Address;
                        salesLeadAddEdit.SalesLeadDetailes.Phone1 = SalesLeadDetails.Phone1;
                        salesLeadAddEdit.SalesLeadDetailes.Phone2 = SalesLeadDetails.Phone2;
                        salesLeadAddEdit.SalesLeadDetailes.Email1 = SalesLeadDetails.Email1;
                        salesLeadAddEdit.SalesLeadDetailes.Email2 = SalesLeadDetails.Email2;
                        salesLeadAddEdit.SalesLeadDetailes.WebsiteUrl = SalesLeadDetails.WebsiteUrl;
                        salesLeadAddEdit.SalesLeadDetailes.IsActive = SalesLeadDetails.IsActive;

                    }
                }

                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "Success!";
                commonResponse.Data = salesLeadAddEdit;
            }
            catch { throw; }
            return View(commonResponse.Data);
        }

        [HttpPost]
        public async Task<CommonResponse> SaveUpdateSalesLead([FromBody] SaveUpdateSalesLead saveUpdateSalesLead)
        {
            CommonResponse commonResponse = new CommonResponse();

            SalesLeadMst salesLeadMst = new SalesLeadMst();
            bool IsDuplicate = false;
            var duplicateCheck = await _dbRepo.SalesLeadList().FirstOrDefaultAsync(x => x.OrgName == salesLeadMst.OrgName && x.Id != salesLeadMst.Id);
            IsDuplicate = duplicateCheck != null;
            if (!IsDuplicate)
            {
                var SalesLeadDetail = await _dbRepo.SalesLeadList().FirstOrDefaultAsync(x => x.Id == saveUpdateSalesLead.Id);
                if (SalesLeadDetail != null)
                {
                    //Edit Mode
                    //salesLeadMst.Id = saveUpdateSalesLead.Id;
                    SalesLeadDetail.Address = saveUpdateSalesLead.Address;
                    SalesLeadDetail.OrgName = saveUpdateSalesLead.OrgName;
                    SalesLeadDetail.CountryId = saveUpdateSalesLead.CountryId;
                    SalesLeadDetail.StateId = saveUpdateSalesLead.StateId;
                    SalesLeadDetail.CityId = saveUpdateSalesLead.CityId;
                    SalesLeadDetail.Phone1 = saveUpdateSalesLead.Phone1;
                    SalesLeadDetail.Phone2 = saveUpdateSalesLead.Phone2;
                    SalesLeadDetail.Email1 = saveUpdateSalesLead.Email1;
                    SalesLeadDetail.Email2 = saveUpdateSalesLead.Email2;
                    SalesLeadDetail.WebsiteUrl = @saveUpdateSalesLead.WebsiteUrl;

                    SalesLeadDetail.CreatedDate = Convert.ToDateTime(SalesLeadDetail.CreatedDate);
                    SalesLeadDetail.CreatedBy = SalesLeadDetail.CreatedBy;
                    SalesLeadDetail.IsActive = SalesLeadDetail.IsActive;
                    SalesLeadDetail.IsDelete = SalesLeadDetail.IsDelete;

                    SalesLeadDetail.UpdatedDate = _commonHelper.GetCurrentDateTime();
                    SalesLeadDetail.UpdatedBy = _commonHelper.GetLoggedInUserId();

                    _dbContext.Entry(SalesLeadDetail).State = EntityState.Modified;
                    _dbContext.SaveChanges();

                    commonResponse.Status = true;
                    commonResponse.StatusCode = HttpStatusCode.OK;
                    commonResponse.Message = "SalesLead Updated Successfully!";
                }
                else
                {
                    //Add Mode
                    salesLeadMst.Address = saveUpdateSalesLead.Address;
                    salesLeadMst.OrgName = saveUpdateSalesLead.OrgName;
                    salesLeadMst.CountryId = saveUpdateSalesLead.CountryId;
                    salesLeadMst.StateId = saveUpdateSalesLead.StateId;
                    salesLeadMst.CityId = saveUpdateSalesLead.CityId;
                    salesLeadMst.Phone1 = saveUpdateSalesLead.Phone1;
                    salesLeadMst.Phone2 = saveUpdateSalesLead.Phone2;
                    salesLeadMst.Email1 = saveUpdateSalesLead.Email1;
                    salesLeadMst.Email2 = saveUpdateSalesLead.Email2;
                    salesLeadMst.WebsiteUrl = saveUpdateSalesLead.WebsiteUrl;
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

        public IActionResult SalesConatactPersonList()
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
