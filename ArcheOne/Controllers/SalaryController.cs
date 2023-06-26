using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using System.Net;

namespace ArcheOne.Controllers
{
    public class SalaryController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly CommonHelper _commonHelper;
        private readonly ArcheOneDbContext _dbContext;

        public SalaryController(DbRepo dbRepo, CommonHelper commonHelper, ArcheOneDbContext dbContext)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
            _dbContext = dbContext;
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
        public IActionResult SearchSalary([FromBody] SalaryReqModel salaryReqModel)
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

        [HttpPost]
        public async Task<IActionResult> UploadSalarySheet(UploadSalarySheetReqModel uploadSalarySheetReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                IExcelDataReader reader;
                string dataFileName = System.IO.Path.GetFileName(uploadSalarySheetReqModel.SalarySheet.FileName);

                string extension = System.IO.Path.GetExtension(uploadSalarySheetReqModel.SalarySheet.FileName);
                Stream stream = uploadSalarySheetReqModel.SalarySheet.OpenReadStream();
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
                        // Read the the Table
                        DataTable salaryDataTable = ds.Tables[0];
                        string companyName = salaryDataTable.Rows[0][1].ToString();
                        int comnpayId = _dbRepo.CompanyMstList().FirstOrDefault(x => x.CompanyName == companyName).Id;
                        string[] yearMonth = salaryDataTable.Rows[1][5].ToString().Split('-');
                        string salaryMonth = yearMonth[0];
                        int salaryYear = int.Parse(yearMonth[1]);

                        List<SalaryMst> salaries = new List<SalaryMst>();
                        for (int i = 1; i < salaryDataTable.Rows.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(salaryDataTable.Rows[i][1].ToString()) && salaryDataTable.Rows[i][1].ToString().All(char.IsDigit))

                            {
                                salaries.Add(new SalaryMst
                                {
                                    EmployeeCode = salaryDataTable.Rows[i][1].ToString() != null ? Convert.ToInt32(salaryDataTable.Rows[i][1].ToString()) : 0,
                                    CompanyId = comnpayId,
                                    SalartMonth = salaryMonth,
                                    SalaryYear = salaryYear,
                                    CreatedBy = _commonHelper.GetLoggedInUserId(),
                                    CreatedDate = _commonHelper.GetCurrentDateTime(),
                                    UpdatedBy = _commonHelper.GetLoggedInUserId(),
                                    UpdatedDate = _commonHelper.GetCurrentDateTime(),
                                    Ctc = !string.IsNullOrEmpty(salaryDataTable.Rows[i][4].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][4].ToString()) : 0,
                                    BasicSalary = !string.IsNullOrEmpty(salaryDataTable.Rows[i][5].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][5].ToString()) : 0,
                                    FixedHra = !string.IsNullOrEmpty(salaryDataTable.Rows[i][6].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][6].ToString()) : 0,
                                    FixedConveyanceAllowance = !string.IsNullOrEmpty(salaryDataTable.Rows[i][7].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][7].ToString()) : 0,
                                    FixedMedicalAllowance = !string.IsNullOrEmpty(salaryDataTable.Rows[i][8].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][8].ToString()) : 0,
                                    AdditionalHraallowance = !string.IsNullOrEmpty(salaryDataTable.Rows[i][9].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][9].ToString()) : 0,
                                    TotalDays = !string.IsNullOrEmpty(salaryDataTable.Rows[i][10].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][10].ToString()) : 0,
                                    PaidLeave = !string.IsNullOrEmpty(salaryDataTable.Rows[i][11].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][11].ToString()) : 0,
                                    UnpaidLeave = !string.IsNullOrEmpty(salaryDataTable.Rows[i][12].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][12].ToString()) : 0,
                                    PayableDays = !string.IsNullOrEmpty(salaryDataTable.Rows[i][13].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][13].ToString()) : 0,
                                    GrossSalaryPayable = !string.IsNullOrEmpty(salaryDataTable.Rows[i][14].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][14].ToString()) : 0,
                                    Basic = !string.IsNullOrEmpty(salaryDataTable.Rows[i][15].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][15].ToString()) : 0,
                                    Hra = !string.IsNullOrEmpty(salaryDataTable.Rows[i][16].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][16].ToString()) : 0,
                                    EmployerContributionToPf = !string.IsNullOrEmpty(salaryDataTable.Rows[i][17].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][17].ToString()) : 0,
                                    ConveyanceAllowance = !string.IsNullOrEmpty(salaryDataTable.Rows[i][18].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][18].ToString()) : 0,
                                    MedicalAllowance = !string.IsNullOrEmpty(salaryDataTable.Rows[i][19].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][19].ToString()) : 0,
                                    Hraallowance = !string.IsNullOrEmpty(salaryDataTable.Rows[i][20].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][20].ToString()) : 0,
                                    FlexibleAllowance = !string.IsNullOrEmpty(salaryDataTable.Rows[i][21].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][21].ToString()) : 0,
                                    IncentiveAllowance = !string.IsNullOrEmpty(salaryDataTable.Rows[i][22].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][22].ToString()) : 0,
                                    TotalEarning = !string.IsNullOrEmpty(salaryDataTable.Rows[i][23].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][23].ToString()) : 0,
                                    Pfemployer = !string.IsNullOrEmpty(salaryDataTable.Rows[i][24].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][24].ToString()) : 0,
                                    Pfemployee = !string.IsNullOrEmpty(salaryDataTable.Rows[i][25].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][25].ToString()) : 0,
                                    Esicemployer = !string.IsNullOrEmpty(salaryDataTable.Rows[i][26].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][26].ToString()) : 0,
                                    Esicemployee = !string.IsNullOrEmpty(salaryDataTable.Rows[i][27].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][27].ToString()) : 0,
                                    ProfessionalTax = !string.IsNullOrEmpty(salaryDataTable.Rows[i][28].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][28].ToString()) : 0,
                                    Advances = !string.IsNullOrEmpty(salaryDataTable.Rows[i][29].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][29].ToString()) : 0,
                                    IncomeTax = !string.IsNullOrEmpty(salaryDataTable.Rows[i][30].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][30].ToString()) : 0,
                                    TotalDeduction = !string.IsNullOrEmpty(salaryDataTable.Rows[i][31].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][31].ToString()) : 0,
                                    NetPayable = !string.IsNullOrEmpty(salaryDataTable.Rows[i][32].ToString()) ? Convert.ToDecimal(salaryDataTable.Rows[i][32].ToString()) : 0,
                                });
                            }

                        }
                        _dbContext.SalaryMsts.AddRangeAsync(salaries);
                        await _dbContext.SaveChangesAsync();
                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Data = salaries;
                        commonResponse.Message = "Salary sheet uploaded successfully";
                    }
                    else
                    {
                        commonResponse.Message = "Fail to upload Salary sheet";
                    }
                }
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Data = ex;
            }
            return Json(commonResponse);
        }

    }
}
