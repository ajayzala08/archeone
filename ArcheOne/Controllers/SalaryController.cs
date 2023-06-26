using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ArcheOne.Controllers
{
    public class SalaryController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly CommonHelper _commonHelper;

        public SalaryController(DbRepo dbRepo, CommonHelper commonHelper)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
        }

        public IActionResult Salary()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Companies()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var companyList = await _dbRepo.CompanyMstList().Select(x => new { x.Id, x.CompanyName }).ToListAsync();
                if (companyList.Count > 0)
                {
                    commonResponse.Status = true;
                    commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
                    commonResponse.Message = "Company list found";
                    commonResponse.Data = companyList;
                }
                else
                {
                    commonResponse.Message = "Company list not found";
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message.ToString();
            }

            return Json(commonResponse);
        }

        [HttpGet]
        public IActionResult Years()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var yearList = Enumerable.Range(2010, (_commonHelper.GetCurrentDateTime().Year - 2010) + 1).Select((index, x) => new { Id = x + 1, SalaryYear = index }).ToList();
                if (yearList.Count > 0)
                {
                    commonResponse.Status = true;
                    commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
                    commonResponse.Message = "Year list found";
                    commonResponse.Data = yearList;
                }
                else
                {
                    commonResponse.Message = "Year list not found";
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message.ToString();
            }

            return Json(commonResponse);
        }


        [HttpGet]
        public IActionResult Months()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var monthList = DateTimeFormatInfo.CurrentInfo.MonthNames.ToList().Select((x, index) => new { Id = index + 1, salaryMonth = x }).ToList();
                if (monthList.Count > 0)
                {
                    commonResponse.Status = true;
                    commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
                    commonResponse.Message = "Month list found";
                    commonResponse.Data = monthList;
                }
                else
                {
                    commonResponse.Message = "Month list not found";
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message.ToString();
            }

            return Json(commonResponse);
        }

        [HttpPost]
        public IActionResult SearchSalary(SalaryReqModel salaryReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {


                commonResponse.Data = "";
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message.ToString();
                commonResponse.Data = ex;
            }
            return Json(commonResponse);
        }

    }
}
