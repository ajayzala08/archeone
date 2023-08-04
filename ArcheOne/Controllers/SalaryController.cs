using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
using ExcelDataReader;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SalaryController(DbRepo dbRepo, CommonHelper commonHelper, ArcheOneDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Salary()
        {
            int userId = _commonHelper.GetLoggedInUserId();
            bool showAddPolicyButton = false;

            CommonResponse departmentDetailsResponse = await new CommonController(_dbRepo, _dbContext, _commonHelper).GetDepartmentByUserId(userId);

            if (departmentDetailsResponse.Status)
            {
                showAddPolicyButton = departmentDetailsResponse.Data.DepartmentCode == CommonEnums.DepartmentMst.Human_Resource.ToString();
            }
            showAddPolicyButton = !showAddPolicyButton ? _commonHelper.CheckHasPermission(CommonEnums.PermissionMst.Policy_Add_View) : showAddPolicyButton;

            return View(showAddPolicyButton);
        }

        [HttpGet]
        public async Task<IActionResult> Companies()
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var companyList = await _dbRepo.CompanyMstList().Select(x => new { x.Id, x.CompanyName }).ToListAsync();
                if (companyList.Count > 0)
                {
                    response.Status = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = "Company list found";
                    response.Data = companyList;
                }
                else
                {
                    response.Message = "Company list not found";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
            }

            return Json(response);
        }

        [HttpGet]
        public IActionResult Years()
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var yearList = Enumerable.Range(2010, (_commonHelper.GetCurrentDateTime().Year - 2010) + 1).Select((index, x) => new { Id = x + 1, SalaryYear = index }).ToList();
                if (yearList.Count > 0)
                {
                    response.Status = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = "Year list found";
                    response.Data = yearList;
                }
                else
                {
                    response.Message = "Year list not found";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
            }

            return Json(response);
        }


        [HttpGet]
        public IActionResult Months()
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var monthList = DateTimeFormatInfo.CurrentInfo.MonthNames.ToList().Select((x, index) => new { Id = index + 1, salaryMonth = x }).ToList();
                if (monthList.Count > 0)
                {
                    response.Status = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = "Month list found";
                    response.Data = monthList;
                }
                else
                {
                    response.Message = "Month list not found";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
            }

            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> SearchSalary([FromBody] SalaryReqModel salaryReqModel)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                int userId = _commonHelper.GetLoggedInUserId();
                bool isUserHR = false;

                CommonResponse departmentDetailsResponse = await new CommonController(_dbRepo, _dbContext, _commonHelper).GetDepartmentByUserId(userId);

                if (departmentDetailsResponse.Status)
                {
                    isUserHR = departmentDetailsResponse.Data.DepartmentCode == CommonEnums.DepartmentMst.Human_Resource.ToString();
                }
                isUserHR = !isUserHR ? _commonHelper.CheckHasPermission(CommonEnums.PermissionMst.Policy_Delete_View) : isUserHR;


                SearchSalaryResModel searchSalaryResModels = new SearchSalaryResModel();

                searchSalaryResModels.IsDeletable = isUserHR;

                searchSalaryResModels.SalaryDetails = new List<SearchSalaryResModel.SalaryDetail>();
                if (isUserHR)
                {
                    searchSalaryResModels.SalaryDetails = await (from salaryList in _dbRepo.SalaryList()
                                                                 where salaryList.CompanyId == salaryReqModel.CompanyId && salaryList.SalartMonth == salaryReqModel.SalaryMonth && salaryList.SalaryYear == salaryReqModel.SalaryYear
                                                                 join userDetail in _dbRepo.UserDetailList() on salaryList.EmployeeCode equals Convert.ToInt32(userDetail.EmployeeCode) into userDetailGroup
                                                                 from userDetailItem in userDetailGroup.DefaultIfEmpty()
                                                                 join userMst in _dbRepo.UserMstList() on userDetailItem.UserId equals userMst.Id into userMstGroup
                                                                 from userMstItem in userMstGroup.DefaultIfEmpty()
                                                                 select new SearchSalaryResModel.SalaryDetail
                                                                 {
                                                                     SalaryId = salaryList.Id,
                                                                     EmployeeCode = salaryList.EmployeeCode,
                                                                     EmployeeName = userMstItem.FirstName + " " + userMstItem.MiddleName + " " + userMstItem.LastName
                                                                 }).Distinct().ToListAsync();
                }
                else
                {
                    searchSalaryResModels.SalaryDetails = await (from userMst in _dbRepo.UserMstList().Where(x => x.Id == userId)
                                                                 join userDetail in _dbRepo.UserDetailList() on userMst.Id equals userDetail.UserId into userDetailGroup
                                                                 from userDetailItem in userDetailGroup.DefaultIfEmpty()
                                                                 join salary in _dbRepo.SalaryList().Where(x => x.CompanyId == salaryReqModel.CompanyId && x.SalartMonth == salaryReqModel.SalaryMonth && x.SalaryYear == salaryReqModel.SalaryYear)
                                                                 on Convert.ToInt32(userDetailItem.EmployeeCode) equals salary.EmployeeCode into salaryGroup
                                                                 from salaryItem in salaryGroup.DefaultIfEmpty()
                                                                 select new SearchSalaryResModel.SalaryDetail
                                                                 {
                                                                     SalaryId = salaryItem.Id,
                                                                     EmployeeCode = salaryItem.EmployeeCode,
                                                                     EmployeeName = userMst.FirstName + " " + userMst.MiddleName + " " + userMst.LastName
                                                                 }).Distinct().ToListAsync();
                }

                response.Data = searchSalaryResModels;
                if (searchSalaryResModels.SalaryDetails.Count > 0)
                {
                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Salary list found!";
                }
                else
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "Search record not found!";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
            }
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> UploadSalarySheet(UploadSalarySheetReqModel uploadSalarySheetReqModel)
        {
            CommonResponse response = new CommonResponse();
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
                        string companyName = !string.IsNullOrEmpty(salaryDataTable.Rows[0][1].ToString()) ? salaryDataTable.Rows[0][1].ToString() : "";
                        if (companyName != null && companyName != "")
                        {
                            var compneyDetailes = _dbRepo.CompanyMstList().FirstOrDefault(x => x.CompanyName.ToLower() == companyName.ToLower());

                            int comnpayId = companyName != "" && companyName != null ? compneyDetailes.Id : 0;
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
                                response.Status = true;
                                response.StatusCode = HttpStatusCode.OK;
                                response.Data = salaries;
                                response.Message = "Salary sheet uploaded successfully";
                            }
                            else
                            {
                                response.Message = "Month-Year not exists in sheet";
                            }
                        }
                        else
                        {
                            response.Message = "Company Name not exists in sheet";
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

        [HttpPost]
        public async Task<IActionResult> DeleteSalary(int id)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var salaryDetail = await _dbRepo.SalaryList().FirstOrDefaultAsync(x => x.Id == id);
                if (salaryDetail != null)
                {
                    salaryDetail.IsDelete = true;
                    _dbContext.SalaryMsts.Update(salaryDetail);
                    _dbContext.SaveChanges();
                    response.Status = true;
                    response.Message = "Salary data deleted successfully";
                    response.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    response.Message = "Salary data not found";
                }


            }
            catch (Exception ex)
            {
                response.Message = ex.Message.ToString();
                response.Data = ex;
            }
            return Json(response);

        }

        [HttpGet]
        public async Task<IActionResult> DownloadSalarySlip(int id)
        {
            string filePath = string.Empty;

            iTextSharp.text.Rectangle pageSize = new iTextSharp.text.Rectangle(iTextSharp.text.PageSize.A4);
            Document document = new Document(pageSize);

            var salaryDetail = (from salaryList in _dbRepo.SalaryList()
                                where salaryList.Id == id
                                join
                                userDetail in _dbRepo.UserDetailList() on salaryList.EmployeeCode.ToString() equals userDetail.EmployeeCode
                                join
                                userMst in _dbRepo.UserMstList() on userDetail.UserId equals userMst.Id
                                join dept in _dbRepo.DepartmentList() on userMst.DepartmentId equals dept.Id
                                join designation in _dbRepo.DesignationList() on userMst.DesignationId equals designation.Id

                                select new { salaryList, userDetail, userMst, dept, designation }).FirstOrDefault();

            var leaveBalance = _dbRepo.LeaveBalanceLists().Where(x => x.UserId == salaryDetail.userMst.Id).OrderByDescending(x => x.Id).FirstOrDefault();
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {

                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();
                var fontTableHeader = FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                var fontTableRow = FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL, BaseColor.DARK_GRAY);
                PdfContentByte content = writer.DirectContentUnder;


                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 90;
                table.HorizontalAlignment = Rectangle.ALIGN_CENTER;

                #region Company Logo and Header
                var contentRootPath = new PhysicalFileProvider(_webHostEnvironment.WebRootPath);
                var imagePhysicalPath = Path.Combine(contentRootPath.Root, "Theme\\Logo\\Reyna_Header.jpg");
                iTextSharp.text.Image logoImage = iTextSharp.text.Image.GetInstance(imagePhysicalPath);
                logoImage.ScaleToFit(485f, 200f);
                PdfPCell imageCell = new PdfPCell(logoImage);
                imageCell.Border = Rectangle.NO_BORDER;
                imageCell.Colspan = 4;
                table.AddCell(imageCell);

                /*  PdfPCell titleandAddressCell = new PdfPCell(new Phrase("\n\n"));
                  titleandAddressCell.Colspan = 3;
                  titleandAddressCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                  titleandAddressCell.Border = Rectangle.NO_BORDER;
                  table.AddCell(titleandAddressCell);*/
                #endregion

                #region Company details & address
                PdfPCell companyNameCell = new PdfPCell(new Phrase("Reyna Solutions LLP", fontTableHeader));
                companyNameCell.Colspan = 4;
                companyNameCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                companyNameCell.Border = Rectangle.NO_BORDER;
                table.AddCell(companyNameCell);

                PdfPCell companyAddressCell = new PdfPCell(new Phrase("202/401, Iscon Atria 2, Opp GET, Gotri, Vadodara-390021", fontTableHeader));
                companyAddressCell.Colspan = 4;
                companyAddressCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                companyAddressCell.Border = Rectangle.NO_BORDER;
                table.AddCell(companyAddressCell);


                PdfPCell monthYearCell = new PdfPCell(new Phrase($"Pay Slip for the month of {salaryDetail.salaryList.SalartMonth}/{salaryDetail.salaryList.SalaryYear}", fontTableHeader));
                monthYearCell.Colspan = 4;
                monthYearCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                monthYearCell.Border = Rectangle.NO_BORDER;
                table.AddCell(monthYearCell);
                #endregion

                #region BlankCell
                PdfPCell blankCell = new PdfPCell(new Phrase("\n\n"));
                blankCell.Colspan = 4;
                blankCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                blankCell.Border = Rectangle.NO_BORDER;
                table.AddCell(blankCell);
                #endregion

                #region EmpId
                PdfPCell empIdTitleCell = new PdfPCell(new Phrase("Emp ID", fontTableRow));
                empIdTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                empIdTitleCell.Border = Rectangle.NO_BORDER;
                table.AddCell(empIdTitleCell);

                PdfPCell empIdValueCell = new PdfPCell(new Phrase($"{salaryDetail.userDetail.EmployeeCode}", fontTableRow));
                empIdValueCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                empIdValueCell.Border = Rectangle.NO_BORDER;
                table.AddCell(empIdValueCell);

                PdfPCell empNameTitleCell = new PdfPCell(new Phrase("Employee Name", fontTableRow));
                empNameTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                empNameTitleCell.Border = Rectangle.NO_BORDER;
                empNameTitleCell.PaddingLeft = 15f;
                table.AddCell(empNameTitleCell);

                PdfPCell empNameValueCell = new PdfPCell(new Phrase($"{salaryDetail.userMst.FirstName} {salaryDetail.userMst.MiddleName} {salaryDetail.userMst.LastName}", fontTableRow));
                empNameValueCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                empNameValueCell.Border = Rectangle.NO_BORDER;
                table.AddCell(empNameValueCell);
                #endregion

                #region PayDays
                PdfPCell payDaysTitleCell = new PdfPCell(new Phrase("Pay Days", fontTableRow));
                payDaysTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                payDaysTitleCell.Border = Rectangle.NO_BORDER;
                table.AddCell(payDaysTitleCell);

                PdfPCell payDaysValueCell = new PdfPCell(new Phrase($"{salaryDetail.salaryList.TotalDays}", fontTableRow));
                payDaysValueCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                payDaysValueCell.Border = Rectangle.NO_BORDER;
                table.AddCell(payDaysValueCell);

                PdfPCell presentDaysTitleCell = new PdfPCell(new Phrase("Present Days", fontTableRow));
                presentDaysTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                presentDaysTitleCell.Border = Rectangle.NO_BORDER;
                presentDaysTitleCell.PaddingLeft = 15f;
                table.AddCell(presentDaysTitleCell);

                PdfPCell presentDaysValueCell = new PdfPCell(new Phrase($"{salaryDetail.salaryList.PayableDays}", fontTableRow));
                presentDaysValueCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                presentDaysValueCell.Border = Rectangle.NO_BORDER;
                table.AddCell(presentDaysValueCell);
                #endregion

                #region DOJ
                PdfPCell dojTitleCell = new PdfPCell(new Phrase("DOJ", fontTableRow));
                dojTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                dojTitleCell.Border = Rectangle.NO_BORDER;
                table.AddCell(dojTitleCell);

                PdfPCell dojValueCell = new PdfPCell(new Phrase($"{salaryDetail.userDetail.JoinDate.ToString("dd/MM/yyyy").Replace('-', '/')}", fontTableRow));
                dojValueCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                dojValueCell.Border = Rectangle.NO_BORDER;
                table.AddCell(dojValueCell);

                PdfPCell designationTitleCell = new PdfPCell(new Phrase("Designation", fontTableRow));
                designationTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                designationTitleCell.Border = Rectangle.NO_BORDER;
                designationTitleCell.PaddingLeft = 15f;
                table.AddCell(designationTitleCell);

                PdfPCell designationValueCell = new PdfPCell(new Phrase($"{salaryDetail.designation.Designation}", fontTableRow));
                designationValueCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                designationValueCell.Border = Rectangle.NO_BORDER;
                table.AddCell(designationValueCell);
                #endregion

                #region Department
                PdfPCell departmentTitleCell = new PdfPCell(new Phrase("Department", fontTableRow));
                departmentTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                departmentTitleCell.Border = Rectangle.NO_BORDER;
                table.AddCell(departmentTitleCell);

                PdfPCell departmentValueCell = new PdfPCell(new Phrase($"{salaryDetail.dept.DepartmentName}", fontTableRow));
                departmentValueCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                departmentValueCell.Border = Rectangle.NO_BORDER;
                table.AddCell(departmentValueCell);

                PdfPCell accountNoTitleCell = new PdfPCell(new Phrase("A/C No", fontTableRow));
                accountNoTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                accountNoTitleCell.Border = Rectangle.NO_BORDER;
                accountNoTitleCell.PaddingLeft = 15f;
                table.AddCell(accountNoTitleCell);

                PdfPCell accountNoValueCell = new PdfPCell(new Phrase($"{salaryDetail.userDetail.AccountNumber}", fontTableRow));
                accountNoValueCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                accountNoValueCell.Border = Rectangle.NO_BORDER;
                table.AddCell(accountNoValueCell);
                #endregion
                document.Add(table);

                PdfPTable tblSalary = new PdfPTable(4);
                tblSalary.WidthPercentage = 95;
                tblSalary.SpacingBefore = 10f;
                tblSalary.SpacingAfter = 10f;


                #region BlankCell
                table.AddCell(blankCell);
                #endregion

                #region header
                PdfPCell earningTitleCell = new PdfPCell(new Phrase("Earnings", fontTableHeader));
                earningTitleCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                earningTitleCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                earningTitleCell.Border = Rectangle.BOX;
                tblSalary.AddCell(earningTitleCell);

                PdfPCell earningValueCell = new PdfPCell(new Phrase("Amount", fontTableHeader));
                earningValueCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                earningValueCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                earningValueCell.Border = Rectangle.BOX;
                tblSalary.AddCell(earningValueCell);

                PdfPCell deductionTitleCell = new PdfPCell(new Phrase("Deduction", fontTableHeader));
                deductionTitleCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                deductionTitleCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                deductionTitleCell.Border = Rectangle.BOX;
                tblSalary.AddCell(deductionTitleCell);

                PdfPCell deductionValueCell = new PdfPCell(new Phrase("Amount", fontTableHeader));
                deductionValueCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                deductionValueCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                deductionValueCell.Border = Rectangle.BOX;
                tblSalary.AddCell(deductionValueCell);
                #endregion

                #region Basic
                PdfPCell basicTitleCell = new PdfPCell(new Phrase("Basic", fontTableRow));
                basicTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                basicTitleCell.Border = Rectangle.BOX;
                basicTitleCell.PaddingLeft = 15f;
                basicTitleCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                basicTitleCell.DisableBorderSide(Rectangle.TOP_BORDER);

                tblSalary.AddCell(basicTitleCell);

                PdfPCell basicValueCell = new PdfPCell(new Phrase($"{salaryDetail.salaryList.Basic}", fontTableRow));
                basicValueCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                basicValueCell.PaddingRight = 20f;
                basicValueCell.Border = Rectangle.BOX;
                basicValueCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                basicValueCell.DisableBorderSide(Rectangle.TOP_BORDER);
                basicValueCell.DisableBorderSide(Rectangle.LEFT_BORDER);

                tblSalary.AddCell(basicValueCell);

                PdfPCell pfTitleCell = new PdfPCell(new Phrase("PF", fontTableRow));
                pfTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                pfTitleCell.PaddingLeft = 15f;
                pfTitleCell.Border = Rectangle.NO_BORDER;
                tblSalary.AddCell(pfTitleCell);

                PdfPCell pfValueCell = new PdfPCell(new Phrase($"{salaryDetail.salaryList.Pfemployer}", fontTableRow));
                pfValueCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                pfValueCell.PaddingRight = 20f;
                pfValueCell.Border = Rectangle.BOX;
                pfValueCell.DisableBorderSide(Rectangle.TOP_BORDER);
                pfValueCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tblSalary.AddCell(pfValueCell);
                #endregion

                #region HRA
                PdfPCell hraTitleCell = new PdfPCell(new Phrase("HRA", fontTableRow));
                hraTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                hraTitleCell.Border = Rectangle.BOX;
                hraTitleCell.PaddingLeft = 15f;
                hraTitleCell.DisableBorderSide(Rectangle.TOP_BORDER);
                hraTitleCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tblSalary.AddCell(hraTitleCell);

                PdfPCell hraValueCell = new PdfPCell(new Phrase($"{salaryDetail.salaryList.Hra}", fontTableRow));
                hraValueCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                hraValueCell.PaddingRight = 20f;
                hraValueCell.Border = Rectangle.BOX;
                hraValueCell.DisableBorderSide(Rectangle.LEFT_BORDER);
                hraValueCell.DisableBorderSide(Rectangle.TOP_BORDER);
                hraValueCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tblSalary.AddCell(hraValueCell);

                PdfPCell ptTitleCell = new PdfPCell(new Phrase("PF", fontTableRow));
                ptTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                ptTitleCell.PaddingLeft = 15f;
                ptTitleCell.Border = Rectangle.NO_BORDER;
                tblSalary.AddCell(ptTitleCell);

                PdfPCell ptValueCell = new PdfPCell(new Phrase($"{salaryDetail.salaryList.ProfessionalTax}", fontTableRow));
                ptValueCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                ptValueCell.PaddingRight = 20f;
                ptValueCell.Border = Rectangle.BOX;
                ptValueCell.DisableBorderSide(Rectangle.TOP_BORDER);
                ptValueCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tblSalary.AddCell(ptValueCell);
                #endregion

                #region Conveyance
                PdfPCell conveyanceTitleCell = new PdfPCell(new Phrase("Conveyance", fontTableRow));
                conveyanceTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                conveyanceTitleCell.Border = Rectangle.BOX;
                conveyanceTitleCell.PaddingLeft = 15f;
                conveyanceTitleCell.DisableBorderSide(Rectangle.TOP_BORDER);
                conveyanceTitleCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tblSalary.AddCell(conveyanceTitleCell);

                PdfPCell conveyanceValueCell = new PdfPCell(new Phrase($"{salaryDetail.salaryList.ConveyanceAllowance}", fontTableRow));
                conveyanceValueCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                conveyanceValueCell.Border = Rectangle.BOX;
                conveyanceValueCell.PaddingRight = 20f;
                conveyanceValueCell.DisableBorderSide(Rectangle.TOP_BORDER);
                conveyanceValueCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                conveyanceValueCell.DisableBorderSide(Rectangle.LEFT_BORDER);
                tblSalary.AddCell(conveyanceValueCell);

                PdfPCell pfEmployerTitleCell = new PdfPCell(new Phrase("PF_Employer", fontTableRow));
                pfEmployerTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                pfEmployerTitleCell.PaddingLeft = 15f;
                pfEmployerTitleCell.Border = Rectangle.NO_BORDER;
                tblSalary.AddCell(pfEmployerTitleCell);

                PdfPCell pfEmployerValueCell = new PdfPCell(new Phrase($"{salaryDetail.salaryList.Pfemployer}", fontTableRow));
                pfEmployerValueCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                pfEmployerValueCell.Border = Rectangle.BOX;
                pfEmployerValueCell.PaddingRight = 20f;
                pfEmployerValueCell.DisableBorderSide(Rectangle.TOP_BORDER);
                pfEmployerValueCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tblSalary.AddCell(pfEmployerValueCell);
                #endregion

                #region Medical
                PdfPCell medicalTitleCell = new PdfPCell(new Phrase("Medical", fontTableRow));
                medicalTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                medicalTitleCell.Border = Rectangle.BOX;
                medicalTitleCell.PaddingLeft = 15f;
                medicalTitleCell.DisableBorderSide(Rectangle.TOP_BORDER);
                medicalTitleCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tblSalary.AddCell(medicalTitleCell);

                PdfPCell medicalValueCell = new PdfPCell(new Phrase($"{salaryDetail.salaryList.MedicalAllowance}", fontTableRow));
                medicalValueCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                medicalValueCell.Border = Rectangle.BOX;
                medicalValueCell.PaddingRight = 20f;
                medicalValueCell.DisableBorderSide(Rectangle.TOP_BORDER);
                medicalValueCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                medicalValueCell.DisableBorderSide(Rectangle.LEFT_BORDER);
                tblSalary.AddCell(medicalValueCell);

                PdfPCell medicalblankTitleCell = new PdfPCell(new Phrase(""));
                medicalblankTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                medicalblankTitleCell.Border = Rectangle.NO_BORDER;
                medicalblankTitleCell.PaddingRight = 15f;
                tblSalary.AddCell(medicalblankTitleCell);

                PdfPCell medicalblankValueCell = new PdfPCell(new Phrase(""));
                medicalblankValueCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                medicalblankValueCell.Border = Rectangle.BOX;
                medicalblankValueCell.DisableBorderSide(Rectangle.TOP_BORDER);
                medicalblankValueCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tblSalary.AddCell(medicalblankValueCell);
                #endregion

                #region FlexibleB
                PdfPCell flexibleTitleCell = new PdfPCell(new Phrase("Flexible B", fontTableRow));
                flexibleTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                flexibleTitleCell.Border = Rectangle.BOX;
                flexibleTitleCell.PaddingLeft = 15f;
                flexibleTitleCell.DisableBorderSide(Rectangle.TOP_BORDER);
                flexibleTitleCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tblSalary.AddCell(flexibleTitleCell);

                PdfPCell flexibleValueCell = new PdfPCell(new Phrase($"{salaryDetail.salaryList.FlexibleAllowance}", fontTableRow));
                flexibleValueCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                flexibleValueCell.Border = Rectangle.BOX;
                flexibleValueCell.PaddingRight = 20f;
                flexibleValueCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                flexibleValueCell.DisableBorderSide(Rectangle.TOP_BORDER);
                flexibleValueCell.DisableBorderSide(Rectangle.LEFT_BORDER);
                tblSalary.AddCell(flexibleValueCell);

                PdfPCell flexibleblankTitleCell = new PdfPCell(new Phrase(""));
                flexibleblankTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                flexibleblankTitleCell.Border = Rectangle.NO_BORDER;
                tblSalary.AddCell(flexibleblankTitleCell);

                PdfPCell flexibleblankValueCell = new PdfPCell(new Phrase(""));
                flexibleblankValueCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                flexibleblankValueCell.Border = Rectangle.BOX;
                flexibleblankValueCell.DisableBorderSide(Rectangle.TOP_BORDER);
                flexibleblankValueCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tblSalary.AddCell(flexibleblankValueCell);
                #endregion

                #region Pf_Employer
                PdfPCell pfemployerTitleCell = new PdfPCell(new Phrase("PF_Employer", fontTableRow));
                pfemployerTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                pfemployerTitleCell.Border = Rectangle.BOX;
                pfemployerTitleCell.PaddingLeft = 15f;
                pfemployerTitleCell.DisableBorderSide(Rectangle.TOP_BORDER);
                pfemployerTitleCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tblSalary.AddCell(pfemployerTitleCell);

                PdfPCell pfemployerValueCell = new PdfPCell(new Phrase($"{salaryDetail.salaryList.Pfemployer}", fontTableRow));
                pfemployerValueCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                pfemployerValueCell.Border = Rectangle.BOX;
                pfemployerValueCell.PaddingRight = 20f;
                pfemployerValueCell.DisableBorderSide(Rectangle.TOP_BORDER);
                pfemployerValueCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                pfemployerValueCell.DisableBorderSide(Rectangle.LEFT_BORDER);
                tblSalary.AddCell(pfemployerValueCell);

                PdfPCell pfemployerblankTitleCell = new PdfPCell(new Phrase(""));
                pfemployerblankTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                pfemployerblankTitleCell.Border = Rectangle.NO_BORDER;
                tblSalary.AddCell(pfemployerblankTitleCell);

                PdfPCell pfemployerblankValueCell = new PdfPCell(new Phrase(""));
                pfemployerblankValueCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                pfemployerblankValueCell.Border = Rectangle.BOX;
                pfemployerblankValueCell.DisableBorderSide(Rectangle.TOP_BORDER);
                pfemployerblankValueCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tblSalary.AddCell(pfemployerblankValueCell);
                #endregion

                #region blankLIne
                PdfPCell pfblanklineCell = new PdfPCell(new Phrase(" "));
                pfblanklineCell.Border = Rectangle.BOX;
                pfblanklineCell.DisableBorderSide(Rectangle.TOP_BORDER);
                pfblanklineCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tblSalary.AddCell(pfblanklineCell);

                PdfPCell pfblanklineCell2 = new PdfPCell(new Phrase(" "));
                pfblanklineCell2.Border = Rectangle.BOX;
                pfblanklineCell2.DisableBorderSide(Rectangle.TOP_BORDER);
                pfblanklineCell2.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                pfblanklineCell2.DisableBorderSide(Rectangle.LEFT_BORDER);
                tblSalary.AddCell(pfblanklineCell2);

                PdfPCell pfblanklineCell3 = new PdfPCell(new Phrase(" "));
                pfblanklineCell3.Border = Rectangle.NO_BORDER;
                tblSalary.AddCell(pfblanklineCell3);

                PdfPCell pfblanklineCell4 = new PdfPCell(new Phrase(" "));
                pfblanklineCell4.Border = Rectangle.BOX;
                pfblanklineCell4.DisableBorderSide(Rectangle.TOP_BORDER);
                pfblanklineCell4.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tblSalary.AddCell(pfblanklineCell4);

                tblSalary.AddCell(pfblanklineCell);
                tblSalary.AddCell(pfblanklineCell2);
                tblSalary.AddCell(pfblanklineCell3);
                tblSalary.AddCell(pfblanklineCell4);

                tblSalary.AddCell(pfblanklineCell);
                tblSalary.AddCell(pfblanklineCell2);
                tblSalary.AddCell(pfblanklineCell3);
                tblSalary.AddCell(pfblanklineCell4);

                tblSalary.AddCell(pfblanklineCell);
                tblSalary.AddCell(pfblanklineCell2);
                tblSalary.AddCell(pfblanklineCell3);
                tblSalary.AddCell(pfblanklineCell4);


                tblSalary.AddCell(pfblanklineCell);
                tblSalary.AddCell(pfblanklineCell2);
                tblSalary.AddCell(pfblanklineCell3);
                tblSalary.AddCell(pfblanklineCell4);


                tblSalary.AddCell(pfblanklineCell);
                tblSalary.AddCell(pfblanklineCell2);
                tblSalary.AddCell(pfblanklineCell3);
                tblSalary.AddCell(pfblanklineCell4);

                tblSalary.AddCell(pfblanklineCell);
                tblSalary.AddCell(pfblanklineCell2);
                tblSalary.AddCell(pfblanklineCell3);
                tblSalary.AddCell(pfblanklineCell4);

                #endregion

                #region Total
                PdfPCell totalEarningTitleCell = new PdfPCell(new Phrase("Total", fontTableHeader));
                totalEarningTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                totalEarningTitleCell.Border = Rectangle.BOX;
                totalEarningTitleCell.PaddingLeft = 15f;
                tblSalary.AddCell(totalEarningTitleCell);

                PdfPCell totalEarningValueCell = new PdfPCell(new Phrase($"{salaryDetail.salaryList.TotalEarning}", fontTableHeader));
                totalEarningValueCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                totalEarningValueCell.PaddingRight = 20f;
                totalEarningValueCell.Border = Rectangle.BOX;
                tblSalary.AddCell(totalEarningValueCell);

                PdfPCell totalDeductionTitleCell = new PdfPCell(new Phrase("Total", fontTableHeader));
                totalDeductionTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                totalDeductionTitleCell.Border = Rectangle.BOX;
                totalDeductionTitleCell.PaddingLeft = 15f;
                tblSalary.AddCell(totalDeductionTitleCell);

                PdfPCell totalDeductionValueCell = new PdfPCell(new Phrase($"{salaryDetail.salaryList.TotalDeduction}", fontTableHeader));
                totalDeductionValueCell.HorizontalAlignment = Rectangle.ALIGN_RIGHT;
                totalDeductionValueCell.PaddingRight = 20f;
                totalDeductionValueCell.Border = Rectangle.BOX;
                tblSalary.AddCell(totalDeductionValueCell);
                #endregion

                #region NetPay
                PdfPCell netPayTitleCell = new PdfPCell(new Phrase("Net", fontTableHeader));
                netPayTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                netPayTitleCell.Border = Rectangle.BOX;
                netPayTitleCell.PaddingLeft = 15f;
                netPayTitleCell.DisableBorderSide(Rectangle.TOP_BORDER);
                netPayTitleCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                netPayTitleCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                tblSalary.AddCell(netPayTitleCell);

                PdfPCell netPayValueCell = new PdfPCell(new Phrase($"{salaryDetail.salaryList.NetPayable}", fontTableRow));
                netPayValueCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                netPayValueCell.Colspan = 3;
                netPayValueCell.Border = Rectangle.BOX;
                netPayValueCell.DisableBorderSide(Rectangle.TOP_BORDER);
                netPayValueCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                netPayValueCell.DisableBorderSide(Rectangle.LEFT_BORDER);
                tblSalary.AddCell(netPayValueCell);
                #endregion

                #region InWords
                PdfPCell inWordsTitleCell = new PdfPCell(new Phrase("In Words", fontTableHeader));
                inWordsTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                inWordsTitleCell.Border = Rectangle.BOX;
                inWordsTitleCell.PaddingLeft = 15f;
                inWordsTitleCell.DisableBorderSide(Rectangle.TOP_BORDER);
                inWordsTitleCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                inWordsTitleCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                tblSalary.AddCell(inWordsTitleCell);

                PdfPCell InWordsValueCell = new PdfPCell(new Phrase($"{_commonHelper.ConvertToWords(Convert.ToString(salaryDetail.salaryList.NetPayable))}", fontTableRow));
                InWordsValueCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                InWordsValueCell.Colspan = 3;
                InWordsValueCell.Border = Rectangle.BOX;
                InWordsValueCell.DisableBorderSide(Rectangle.TOP_BORDER);
                InWordsValueCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                InWordsValueCell.DisableBorderSide(Rectangle.LEFT_BORDER);
                tblSalary.AddCell(InWordsValueCell);
                #endregion

                #region blankline
                PdfPCell blanksignatureTitleCell = new PdfPCell(new Phrase(" "));
                blanksignatureTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                blanksignatureTitleCell.Colspan = 4;
                blanksignatureTitleCell.Border = Rectangle.BOX;
                blanksignatureTitleCell.DisableBorderSide(Rectangle.TOP_BORDER);
                blanksignatureTitleCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tblSalary.AddCell(blanksignatureTitleCell);
                #endregion

                #region Signature
                PdfPCell signatureTitleCell = new PdfPCell(new Phrase(" "));
                signatureTitleCell.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                signatureTitleCell.Colspan = 3;
                signatureTitleCell.Border = Rectangle.BOX;
                signatureTitleCell.DisableBorderSide(Rectangle.TOP_BORDER);
                signatureTitleCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
                tblSalary.AddCell(signatureTitleCell);

                PdfPCell signatureValueCell = new PdfPCell(new Phrase($"Signature", fontTableHeader));
                signatureValueCell.HorizontalAlignment = Rectangle.ALIGN_MIDDLE;
                signatureValueCell.Border = Rectangle.BOX;
                signatureValueCell.DisableBorderSide(Rectangle.TOP_BORDER);
                signatureValueCell.DisableBorderSide(Rectangle.LEFT_BORDER);
                tblSalary.AddCell(signatureValueCell);
                #endregion

                document.Add(tblSalary);

                Paragraph paragraph = new Paragraph();
                paragraph.IndentationLeft = 15;

                PdfPTable tblLeave = new PdfPTable(7);
                tblLeave.WidthPercentage = 55;
                tblLeave.SpacingAfter = 10f;
                tblLeave.SpacingBefore = 10f;
                tblLeave.HorizontalAlignment = Rectangle.ALIGN_LEFT;

                #region Header
                PdfPCell leaveTypeCellTitle = new PdfPCell(new Phrase("Lev.Type", fontTableRow));
                leaveTypeCellTitle.Border = Rectangle.BOX;
                tblLeave.AddCell(leaveTypeCellTitle);

                PdfPCell opbalCellTitle = new PdfPCell(new Phrase("Op.Bal", fontTableRow));
                opbalCellTitle.Border = Rectangle.BOX;
                tblLeave.AddCell(opbalCellTitle);

                PdfPCell allotCellTitle = new PdfPCell(new Phrase("Allot.", fontTableRow));
                allotCellTitle.Border = Rectangle.BOX;
                tblLeave.AddCell(allotCellTitle);

                PdfPCell availCellTitle = new PdfPCell(new Phrase("Avail.", fontTableRow));
                availCellTitle.Border = Rectangle.BOX;
                tblLeave.AddCell(availCellTitle);

                PdfPCell encashCellTitle = new PdfPCell(new Phrase("Encash.", fontTableRow));
                encashCellTitle.Border = Rectangle.BOX;
                tblLeave.AddCell(encashCellTitle);

                PdfPCell adjCellTitle = new PdfPCell(new Phrase("Adj.", fontTableRow));
                adjCellTitle.Border = Rectangle.BOX;
                tblLeave.AddCell(adjCellTitle);

                PdfPCell clBalCellTitle = new PdfPCell(new Phrase("Cl.Bal", fontTableRow));
                clBalCellTitle.Border = Rectangle.BOX;
                tblLeave.AddCell(clBalCellTitle);
                #endregion

                #region Value
                PdfPCell leaveTypeCellvalue = new PdfPCell(new Phrase("Leave", fontTableRow));
                leaveTypeCellvalue.Border = Rectangle.BOX;
                tblLeave.AddCell(leaveTypeCellvalue);

                PdfPCell opbalCellvalue = new PdfPCell(new Phrase(_commonHelper.GetFormattedDecimal((decimal)leaveBalance.OpeningLeaveBalance), fontTableRow));
                opbalCellvalue.Border = Rectangle.BOX;
                tblLeave.AddCell(opbalCellvalue);

                PdfPCell allotCellvalue = new PdfPCell(new Phrase("1.5 ", fontTableRow));
                allotCellvalue.Border = Rectangle.BOX;
                tblLeave.AddCell(allotCellvalue);

                PdfPCell availCellvalue = new PdfPCell(new Phrase("0 ", fontTableRow));
                availCellvalue.Border = Rectangle.BOX;
                tblLeave.AddCell(availCellvalue);

                PdfPCell encashCellvalue = new PdfPCell(new Phrase("0", fontTableRow));
                encashCellvalue.Border = Rectangle.BOX;
                tblLeave.AddCell(encashCellvalue);

                PdfPCell adjCellvalue = new PdfPCell(new Phrase("0", fontTableRow));
                adjCellvalue.Border = Rectangle.BOX;
                tblLeave.AddCell(adjCellvalue);

                PdfPCell clBalCellvalue = new PdfPCell(new Phrase(_commonHelper.GetFormattedDecimal((decimal)leaveBalance.ClosingLeaveBalance), fontTableRow));
                clBalCellvalue.Border = Rectangle.BOX;
                tblLeave.AddCell(clBalCellvalue);
                #endregion

                paragraph.Add(tblLeave);
                document.Add(paragraph);


                Paragraph paragraphAdvance = new Paragraph();
                paragraphAdvance.IndentationRight = 15;

                PdfPTable tblAdvance = new PdfPTable(5);
                tblAdvance.WidthPercentage = 55;
                tblAdvance.HorizontalAlignment = Rectangle.ALIGN_RIGHT;

                #region AdvanceHeader
                PdfPCell advanceCellTitle = new PdfPCell(new Phrase("Advance", fontTableRow));
                advanceCellTitle.Border = Rectangle.BOX;
                tblAdvance.AddCell(advanceCellTitle);

                PdfPCell takenCellTitle = new PdfPCell(new Phrase("Taken", fontTableRow));
                takenCellTitle.Border = Rectangle.BOX;
                tblAdvance.AddCell(takenCellTitle);

                PdfPCell opbalaCellTitle = new PdfPCell(new Phrase("Op.Bal", fontTableRow));
                opbalaCellTitle.Border = Rectangle.BOX;
                tblAdvance.AddCell(opbalaCellTitle);

                PdfPCell emirctCellTitle = new PdfPCell(new Phrase("EMI/Rct.", fontTableRow));
                emirctCellTitle.Border = Rectangle.BOX;
                tblAdvance.AddCell(emirctCellTitle);

                PdfPCell clbalaCellTitle = new PdfPCell(new Phrase("Cl.Bal", fontTableRow));
                clbalaCellTitle.Border = Rectangle.BOX;
                tblAdvance.AddCell(clbalaCellTitle);


                #endregion






                paragraphAdvance.Add(tblAdvance);
                document.Add(paragraphAdvance);

                document.Close();
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                var filename = $"{salaryDetail.userMst.FirstName} {salaryDetail.userMst.MiddleName} {salaryDetail.userMst.LastName} ({salaryDetail.userDetail.EmployeeCode})";
                filePath = (filename + ".pdf");
                return File(bytes, "application/pdf", filePath);
            }
        }
    }
}
