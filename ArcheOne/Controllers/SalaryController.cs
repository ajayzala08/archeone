using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
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
                List<SearchSalaryResModel> searchSalaryResModels = new List<SearchSalaryResModel>();
                searchSalaryResModels = (from salaryList in _dbRepo.SalaryList()
                                         join
                                         userDetail in _dbRepo.UserDetailList() on salaryList.EmployeeCode.ToString() equals userDetail.EmployeeCode
                                         join
                                         userMst in _dbRepo.UserMstList() on userDetail.UserId equals userMst.Id
                                         select new { salaryList, userDetail, userMst }).Select(x => new SearchSalaryResModel
                                         {
                                             SalaryId = x.salaryList.Id,
                                             EmployeeCode = x.salaryList.EmployeeCode,
                                             EmployeeName = x.userMst.FirstName + " " + x.userMst.MiddleName + " " + x.userMst.LastName
                                         }).Distinct().ToList();

                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "Salary list found.";
                commonResponse.Data = searchSalaryResModels;
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
                // string dataFileName = System.IO.Path.GetFileName(uploadSalarySheetReqModel.SalarySheet.FileName);

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
                        string? companyName = !string.IsNullOrEmpty(salaryDataTable.Rows[0][1].ToString()) ? salaryDataTable.Rows[0][1].ToString() : "";
                        if (companyName != null && companyName != "")
                        {
                            int comnpayId = companyName != "" && companyName != null ? _dbRepo.CompanyMstList().FirstOrDefault(x => x.CompanyName == companyName).Id : 0;
                            if (salaryDataTable.Rows[1][5] != null && Convert.ToString(salaryDataTable.Rows[1][5]) != "")
                            {
                                string[] yearMonth = Convert.ToString(salaryDataTable.Rows[1][5]).Split('-');
                                string salaryMonth = yearMonth[0];
                                int salaryYear = int.Parse(yearMonth[1]);

                                List<SalaryMst> salaries = new List<SalaryMst>();
                                for (int i = 1; i < salaryDataTable.Rows.Count; i++)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][1])) && Convert.ToString(salaryDataTable.Rows[i][1]).All(char.IsDigit))

                                    {
                                        salaries.Add(new SalaryMst
                                        {
                                            EmployeeCode = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][1])) ? Convert.ToInt32(salaryDataTable.Rows[i][1].ToString()) : 0,
                                            CompanyId = comnpayId,
                                            SalartMonth = salaryMonth,
                                            SalaryYear = salaryYear,
                                            CreatedBy = _commonHelper.GetLoggedInUserId(),
                                            CreatedDate = _commonHelper.GetCurrentDateTime(),
                                            UpdatedBy = _commonHelper.GetLoggedInUserId(),
                                            UpdatedDate = _commonHelper.GetCurrentDateTime(),
                                            Ctc = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][4])) ? Convert.ToDecimal(salaryDataTable.Rows[i][4].ToString()) : 0,
                                            BasicSalary = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][5])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][5])) : 0,
                                            FixedHra = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][6])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][6])) : 0,
                                            FixedConveyanceAllowance = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][7])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][7])) : 0,
                                            FixedMedicalAllowance = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][8])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][8])) : 0,
                                            AdditionalHraallowance = !string.IsNullOrEmpty(salaryDataTable.Rows[i][9].ToString()) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][9])) : 0,
                                            TotalDays = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][10])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][10])) : 0,
                                            PaidLeave = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][11])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][11])) : 0,
                                            UnpaidLeave = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][12])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][12])) : 0,
                                            PayableDays = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][13])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][13])) : 0,
                                            GrossSalaryPayable = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][14])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][14])) : 0,
                                            Basic = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][15])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][15])) : 0,
                                            Hra = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][16])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][16])) : 0,
                                            EmployerContributionToPf = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][17])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][17])) : 0,
                                            ConveyanceAllowance = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][18])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][18])) : 0,
                                            MedicalAllowance = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][19])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][19])) : 0,
                                            Hraallowance = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][20])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][20])) : 0,
                                            FlexibleAllowance = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][21])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][21])) : 0,
                                            IncentiveAllowance = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][22])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][22])) : 0,
                                            TotalEarning = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][23])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][23])) : 0,
                                            Pfemployer = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][24])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][24])) : 0,
                                            Pfemployee = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][25])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][25])) : 0,
                                            Esicemployer = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][26])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][26])) : 0,
                                            Esicemployee = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][27])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][27])) : 0,
                                            ProfessionalTax = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][28])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][28])) : 0,
                                            Advances = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][29])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][29])) : 0,
                                            IncomeTax = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][30])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][30])) : 0,
                                            TotalDeduction = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][31])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][31])) : 0,
                                            NetPayable = !string.IsNullOrEmpty(Convert.ToString(salaryDataTable.Rows[i][32])) ? Convert.ToDecimal(Convert.ToString(salaryDataTable.Rows[i][32])) : 0
                                        });
                                    }

                                }
                                await _dbContext.SalaryMsts.AddRangeAsync(salaries);
                                await _dbContext.SaveChangesAsync();
                                commonResponse.Status = true;
                                commonResponse.StatusCode = HttpStatusCode.OK;
                                commonResponse.Data = salaries;
                                commonResponse.Message = "Salary sheet uploaded successfully";
                            }
                            else
                            {
                                commonResponse.Message = "Month-Year not exists in sheet";
                            }
                        }
                        else
                        {
                            commonResponse.Message = "Company Name not exists in sheet";
                        }
                    }
                    else
                    {
                        commonResponse.Message = "No records found";
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

        [HttpPost]
        public async Task<IActionResult> DeleteSalary(int id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var salaryDetail = await _dbRepo.SalaryList().FirstOrDefaultAsync(x => x.Id == id);
                if (salaryDetail != null)
                {
                    salaryDetail.IsDelete = true;
                    _dbContext.SalaryMsts.Update(salaryDetail);
                    _dbContext.SaveChanges();
                    commonResponse.Status = true;
                    commonResponse.Message = "Salary data deleted successfully";
                    commonResponse.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    commonResponse.Message = "Salary data not found";
                }


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
