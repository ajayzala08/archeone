using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
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
        public IActionResult SalesList()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AddEditSalesLead(int Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                SalesLeadAddEditReqViewModel salesLeadAddEdit = new SalesLeadAddEditReqViewModel();
                salesLeadAddEdit.SalesLeadDetailes = new SalesLeadDetailes();
                // salesLeadAddEdit.CountryList = await _dbRepo.GetCountryList();
                if (Id > 0)
                {
                    var SalesLeadDetails = await _dbRepo.SalesLeadMst().FirstOrDefaultAsync(x => x.Id == Id);
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
    }
}
