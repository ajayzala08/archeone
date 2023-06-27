using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Color = System.Drawing.Color;

namespace ArcheOne.Controllers
{
    public class TaskController : Controller
    {
        readonly ArcheOneDbContext _dbContext;
        readonly CommonHelper _commonHelper;
        readonly DbRepo _dbRepo;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment { get; }

        public TaskController(ArcheOneDbContext dbContext, CommonHelper commonHelper, DbRepo dbRepo, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _dbRepo = dbRepo;
            _dbContext = dbContext;
            _commonHelper = commonHelper;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Task()
        {
            return View();
        }

        public async Task<CommonResponse> GetTaskList([FromBody] GetTaskListReqModel request)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                int userId = _commonHelper.GetLoggedInUserId();
                CommonResponse roleDetailsResponse = await new RoleController(_dbRepo).GetRoleByUserId(userId);

                if (roleDetailsResponse.Status)
                {
                    bool showUserName = true;
                    if (roleDetailsResponse.Data.RoleCode == CommonEnums.RoleMst.Super_Admin.ToString())
                    {
                        showUserName = false;
                    }

                    var dailyTaskList = await _dbRepo.DailyTaskList()
                        .Where(x
                        => (request.ResourceId != 0 ? x.CreatedBy == request.ResourceId : true)
                        && (request.ProjectId != 0 ? x.ProjectId == request.ProjectId : true)
                        && (request.FromDate != null && request.ToDate != null
                        ? request.FromDate.Value.Date <= x.TaskDate.Date && x.TaskDate.Date <= request.ToDate.Value.Date
                        : request.FromDate != null ? x.TaskDate.Date == request.FromDate.Value.Date : true)).ToListAsync();

                    var taskList = (from dailyTask in dailyTaskList
                                    join project in _dbRepo.ProjectList() on dailyTask.ProjectId equals project.Id into projectGroup
                                    from projectItem in projectGroup.DefaultIfEmpty()
                                    join c in _dbRepo.AllUserMstList() on dailyTask.CreatedBy equals c.Id into cGroup
                                    from cItem in cGroup.DefaultIfEmpty()
                                    select new
                                    {
                                        dailyTask.Id,
                                        projectItem.ProjectName,
                                        dailyTask.TaskDate,
                                        dailyTask.TaskStatus,
                                        dailyTask.TimeSpent,
                                        dailyTask.TaskModule,
                                        dailyTask.TaskDescription,
                                        dailyTask.CreatedBy,
                                        CreatedByName = $"{cItem.FirstName ?? ""} {cItem.LastName ?? ""}",
                                        dailyTask.CreatedDate,
                                        IsEditable = !(dailyTask.CreatedDate.Date != _commonHelper.GetCurrentDateTime().Date),
                                        ShowUserName = showUserName
                                    }).ToList();

                    if (taskList != null && taskList.Count > 0)
                    {
                        response.Data = taskList;
                        response.Message = "Data found successfully!";
                        response.Status = true;
                        response.StatusCode = System.Net.HttpStatusCode.OK;
                    }
                    else
                    {
                        response.Message = "Data not found!";
                        response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<CommonResponse> GetTaskById(int TaskId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var dailyTaskDetails = await _dbRepo.DailyTaskList().FirstOrDefaultAsync(x => x.Id == TaskId);
                if (dailyTaskDetails != null)
                {
                    response.Data = dailyTaskDetails;
                    response.Status = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = "Data found successfully!";
                }
                else
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = "Data not found!";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<CommonResponse> AddUpdateTask([FromBody] AddUpdateTaskReqModel request)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    int userId = _commonHelper.GetLoggedInUserId();
                    if (request.Id == 0) // Add Daily Task
                    {
                        DailyTaskMst dailyTaskMst = new DailyTaskMst()
                        {
                            ProjectId = request.ProjectId,
                            TaskDate = request.TaskDate,
                            TaskStatus = request.TaskStatus,
                            TaskModule = request.TaskModule,
                            TaskDescription = request.TaskDescription,
                            TimeSpent = $"{request.TimeSpentHH}:{request.TimeSpentMM}",
                            IsActive = true,
                            IsDelete = false,
                            CreatedBy = userId,
                            UpdatedBy = userId,
                            CreatedDate = _commonHelper.GetCurrentDateTime(),
                            UpdatedDate = _commonHelper.GetCurrentDateTime()
                        };

                        await _dbContext.AddAsync(dailyTaskMst);
                        await _dbContext.SaveChangesAsync();

                        response.Status = true;
                        response.StatusCode = System.Net.HttpStatusCode.OK;
                        response.Message = "Task added successfully!";
                    }
                    else // updated
                    {
                        var dailyTask = await _dbRepo.DailyTaskList().FirstOrDefaultAsync(x => x.Id == request.Id);
                        if (dailyTask != null)
                        {
                            dailyTask.ProjectId = request.ProjectId;
                            dailyTask.TaskDate = request.TaskDate;
                            dailyTask.TaskStatus = request.TaskStatus;
                            dailyTask.TaskModule = request.TaskModule;
                            dailyTask.TaskDescription = request.TaskDescription;
                            dailyTask.TimeSpent = $"{request.TimeSpentHH}:{request.TimeSpentMM}";
                            dailyTask.UpdatedBy = userId;
                            dailyTask.UpdatedDate = _commonHelper.GetCurrentDateTime();

                            _dbContext.Entry(dailyTask).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();

                            response.Status = true;
                            response.StatusCode = System.Net.HttpStatusCode.OK;
                            response.Message = "Task updated successfully!";
                        }
                        else
                        {
                            response.Message = "Task not found!";
                            response.StatusCode = System.Net.HttpStatusCode.NotFound;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<CommonResponse> DeleteTaskById(int TaskId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var dailyTaskMst = await _dbRepo.DailyTaskList().FirstOrDefaultAsync(x => x.Id == TaskId);
                if (dailyTaskMst != null)
                {
                    dailyTaskMst.IsDelete = true;
                    dailyTaskMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                    dailyTaskMst.UpdatedDate = _commonHelper.GetCurrentDateTime();

                    _dbContext.Entry(dailyTaskMst).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();


                    response.Status = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = "Task deleted successfully!";
                }
                else
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = "Data not found!";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public IActionResult TaskReport()
        {
            return View();
        }


        public async Task<CommonResponse> GenerateTaskReport()
        {
            CommonResponse response = new CommonResponse();
            try
            {
                response.Message = await ExportProjectReportExcel();
                response.Status = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public class DailyTaskExcelViewModel
        {
            public int Id { get; set; }

            public string ProjectName { get; set; } = null!;

            public string ProjectStatus { get; set; } = null!;

            public List<Resources> UserMst { get; set; }
            public List<DailyTask> DailyTaskMsts { get; set; }

            public class Resources
            {
                public int Id { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
                public int RoleId { get; set; }
                public string Role { get; set; }
                public decimal Salary { get; set; }
            }

            public class DailyTask
            {
                public int Id { get; set; }
                public int ProjectId { get; set; }
                public string TimeSpent { get; set; }
                public string TaskModule { get; set; }
                public int CreatedBy { get; set; }
            }
        }

        public async Task<string> ExportProjectReportExcel()
        {
            string FileName = "WeeklyTaskReport_" + _commonHelper.GetCurrentDateTime().ToString("dd-MM-yyyy_HHmmss") + ".xlsx";
            string CurrentDirectory = Directory.GetCurrentDirectory();
            var target = Path.Combine(CurrentDirectory, "Files");
            Directory.CreateDirectory(target);
            var target1 = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", "Files", "WeeklyReports");

            Directory.CreateDirectory(target1);
            var strPath = Path.Combine(target1, FileName);

            List<DailyTaskExcelViewModel> dailyExcelModelList = new List<DailyTaskExcelViewModel>();

            var projectList = await _dbRepo.ProjectList().Select(x => new { x.Id, x.ProjectName, x.ProjectStatus, x.Resources }).ToListAsync();
            var userIdsWithComma = string.Join(",", projectList.Select(x => x.Resources).ToList());
            List<string> userIds = userIdsWithComma.Split(',').Distinct().ToList();

            var allUserData = (from userId in userIds
                               join user in _dbRepo.UserMstList() on userId equals user.Id.ToString()
                               join role in _dbRepo.RoleMstList() on user.RoleId equals role.Id into roleGroup
                               from roleItem in roleGroup.DefaultIfEmpty()
                               join userDetails in _dbRepo.UserDetailList() on user.Id equals userDetails.UserId into userDetailsGroup
                               from userDetailsItem in userDetailsGroup.DefaultIfEmpty()
                               select new DailyTaskExcelViewModel.Resources
                               {
                                   Id = user.Id,
                                   FirstName = user.FirstName,
                                   LastName = user.LastName,
                                   RoleId = roleItem.Id,
                                   Role = roleItem.RoleName,
                                   Salary = userDetailsItem?.Salary ?? 0
                               }).ToList();

            var previousDateTime = GetLastStartDateTime();

            var allDailyTaskData = (from userId in userIds
                                    join dailyTask in _dbRepo.DailyTaskList() on userId equals dailyTask.CreatedBy.ToString()
                                    where (previousDateTime <= dailyTask.CreatedDate && dailyTask.CreatedDate <= _commonHelper.GetCurrentDateTime())
                                    select new DailyTaskExcelViewModel.DailyTask
                                    {
                                        Id = dailyTask.Id,
                                        ProjectId = dailyTask.ProjectId,
                                        TimeSpent = dailyTask.TimeSpent,
                                        TaskModule = dailyTask.TaskModule,
                                        CreatedBy = dailyTask.CreatedBy
                                    }).ToList();

            foreach (var item in projectList)
            {
                DailyTaskExcelViewModel dailyExcelModel = new DailyTaskExcelViewModel();

                dailyExcelModel.Id = item.Id;
                dailyExcelModel.ProjectName = item.ProjectName;
                dailyExcelModel.ProjectStatus = item.ProjectStatus;

                dailyExcelModel.UserMst = new List<DailyTaskExcelViewModel.Resources>();
                dailyExcelModel.DailyTaskMsts = new List<DailyTaskExcelViewModel.DailyTask>();

                foreach (var id in item.Resources.Split(','))
                {
                    var userData = allUserData.Where(x => x.Id.ToString() == id).FirstOrDefault();
                    dailyExcelModel.UserMst.Add(userData != null ? userData : new DailyTaskExcelViewModel.Resources());

                    var dailyTaskData = allDailyTaskData.Where(x => x.CreatedBy.ToString() == id && x.ProjectId == item.Id).ToList();
                    DailyTaskExcelViewModel.DailyTask dailyTask = new DailyTaskExcelViewModel.DailyTask();
                    foreach (var daily in dailyTaskData)
                    {
                        daily.TimeSpent = GetHourByMinutes(GetMinutesByHours(dailyTask.TimeSpent ?? "0:0") + GetMinutesByHours(daily.TimeSpent));
                        dailyTask = daily;
                    }
                    dailyExcelModel.DailyTaskMsts.Add(dailyTask != null ? dailyTask : new DailyTaskExcelViewModel.DailyTask());
                }
                dailyExcelModelList.Add(dailyExcelModel);
            }

            List<string> SheetNames = new List<string>();

            try
            {
                byte[] a = null;
                var memoryStream = new MemoryStream();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excelPackage = new ExcelPackage(memoryStream);

                for (int i = 0; i < dailyExcelModelList.Count; i++)
                {
                    ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add(dailyExcelModelList[i].ProjectName);
                    SheetNames.Add(dailyExcelModelList[i].ProjectName);

                    int r = 4; // Initialize Excel Row Start Position  = 4
                    excelWorksheet.Cells["A1:E1"].Merge = true;
                    excelWorksheet.Cells["A1:E1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    excelWorksheet.Cells["A1:E1"].Style.Fill.BackgroundColor.SetColor(Color.LightPink);
                    excelWorksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.DarkRed);
                    excelWorksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    excelWorksheet.Cells["A1:E1"].Value = "In-House Project";

                    excelWorksheet.Cells["A2:D2"].Merge = true;
                    excelWorksheet.Cells["A2:D2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    excelWorksheet.Cells["A2:D2"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    excelWorksheet.Cells["A2:D2"].Style.Font.Color.SetColor(Color.DarkOrange);
                    excelWorksheet.Cells["A2:D2"].Style.Font.Size = 22;
                    excelWorksheet.Cells["A2:D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    excelWorksheet.Cells["A2:D2"].Value = "Resources Involved";

                    excelWorksheet.Cells["E2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    excelWorksheet.Cells["E2"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    excelWorksheet.Cells["E2"].Style.Font.Color.SetColor(Color.DarkOrange);
                    excelWorksheet.Cells["E2"].Style.Font.Size = 16;
                    excelWorksheet.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    excelWorksheet.Cells["E2"].Value = _commonHelper.GetCurrentDateTime().ToString("dd-MM-yyyy");

                    excelWorksheet.Cells["A3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    excelWorksheet.Cells["A3"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    excelWorksheet.Cells["A3"].Style.Font.Color.SetColor(Color.White);
                    excelWorksheet.Cells["A3"].Value = "Name";
                    excelWorksheet.Cells["B3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    excelWorksheet.Cells["B3"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    excelWorksheet.Cells["B3"].Style.Font.Color.SetColor(Color.White);
                    excelWorksheet.Cells["B3"].Value = "Role in project";
                    excelWorksheet.Cells["C3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    excelWorksheet.Cells["C3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    excelWorksheet.Cells["C3"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    excelWorksheet.Cells["C3"].Style.Font.Color.SetColor(Color.White);
                    excelWorksheet.Cells["C3"].Value = "% of Involvement";
                    excelWorksheet.Cells["D3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    excelWorksheet.Cells["D3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    excelWorksheet.Cells["D3"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    excelWorksheet.Cells["D3"].Style.Font.Color.SetColor(Color.White);
                    excelWorksheet.Cells["D3"].Value = "Proportional Salary for the week";
                    excelWorksheet.Cells["E3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    excelWorksheet.Cells["E3"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    excelWorksheet.Cells["E3"].Style.Font.Color.SetColor(Color.White);
                    excelWorksheet.Cells["E3"].Value = "Comments";

                    // Excel row and column start positions for writing Row=1 and Col=1
                    for (int col = 0; col < dailyExcelModelList[i].DailyTaskMsts.Count; col++)
                    {
                        excelWorksheet.Cells[r, 1].Value = $"{dailyExcelModelList[i].UserMst[col].FirstName} {dailyExcelModelList[i].UserMst[col].LastName}";
                        excelWorksheet.Cells[r, 2].Value = dailyExcelModelList[i].UserMst[col].Role;
                        excelWorksheet.Cells[r, 3].Value = Convert.ToDouble(GetPercentageByHrsWorking(dailyExcelModelList[i].DailyTaskMsts[col].TimeSpent ?? "00:00"));
                        excelWorksheet.Cells[r, 4].Value = (GetPerMinuteSalary((decimal?)dailyExcelModelList[i].UserMst[col].Salary ?? 0.0m) * GetMinutesByHours(dailyExcelModelList[i].DailyTaskMsts[col].TimeSpent ?? "00:00"));

                        excelWorksheet.Cells[r, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        excelWorksheet.Cells[r, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        excelWorksheet.Cells[r, 5].Value = dailyExcelModelList[i].DailyTaskMsts[col].TaskModule ?? "NA";

                        r++;
                    }

                    if (dailyExcelModelList[i].ProjectStatus?.ToLower() == "completed")
                    {
                        int currentRow = r + 2;
                        excelWorksheet.Cells["A" + currentRow + ":C" + currentRow].Merge = true;
                        excelWorksheet.Cells["A" + currentRow + ":C" + currentRow].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        excelWorksheet.Cells["A" + currentRow + ":C" + currentRow].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
                        excelWorksheet.Cells["A" + currentRow + ":C" + currentRow].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        excelWorksheet.Cells["A" + currentRow + ":C" + currentRow].Value = "Project Closed";
                    }

                    excelWorksheet.Columns.AutoFit();
                    string modelRange = "A1:E" + (r - 1).ToString();
                    var modelTable = excelWorksheet.Cells[modelRange];

                    // Assign borders
                    modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }
                a = excelPackage.GetAsByteArray();
                System.IO.File.WriteAllBytes(strPath, a);
            }
            catch (Exception ex)
            {
                FileName = "Error :: " + ex.Message;
            }
            return FileName;
        }

        public decimal GetPerMinuteSalary(decimal annualPackage)
        {
            var monthSalary = annualPackage / 12; // Monthly Salary
            var monthDays = DateTime.DaysInMonth(
              DateTime.Now.Year, // current Year
              DateTime.Now.Month // current Month
              );
            var daySalary = monthSalary / monthDays; // Per Day Salary as per Month Days.
            var hourSalary = daySalary / 8; // Per hour salary.
            var minuteSalary = hourSalary / 60.0m; // Per Minute Salary.
            return Math.Round(minuteSalary, 3);
        }

        public int GetMinutesByHours(string input)
        {
            var inputarr = input != null ? input.Split(':') : ("00:00").Split(':');
            var finalAmout = (Convert.ToInt32(inputarr[0]) * 60) + Convert.ToInt32(inputarr[1]);

            // this line of code is to set max salary per day : 480
            finalAmout = finalAmout > 480 ? 480 : finalAmout;

            return finalAmout;
        }

        public string GetHourByMinutes(int input)
        {
            return Convert.ToString((input / 60) + ":" + (input % 60));
        }

        public string GetPercentageByHrsWorking(string inputTime)
        {
            double finalOutput = 0.0;
            if (inputTime != null || inputTime != string.Empty)
            {
                inputTime = inputTime.Trim();
                int hr = 0, min = 0;
                var arr = inputTime.Split(':');
                if (arr != null)
                {
                    hr = Convert.ToInt32(arr[0] != null ? arr[0] : "0");
                    min = Convert.ToInt32(arr[1] != null ? arr[1] : "00");

                    int finalMinutes = 0;

                    if (hr > 0)
                    {
                        finalMinutes = hr * 60;
                    }
                    if (min > 0)
                    {
                        finalMinutes = finalMinutes + min;
                    }
                    finalOutput = (finalMinutes * 100.0f) / 2400.0f;

                }
            }
            return finalOutput.ToString("n2");
        }

        public DateTime GetLastStartDateTime()
        {
            //var emailMst = db.EmailScheduleMst.SingleOrDefault();

            //string startDay = emailMst.WeekDay;
            string startDay = "Friday";

            DateTime lastStartDateTime = DateTime.Now.AddDays(-1);
            while (lastStartDateTime.DayOfWeek.ToString() != startDay)
                lastStartDateTime = lastStartDateTime.AddDays(-1);

            return lastStartDateTime.Date;
        }

    }
}
