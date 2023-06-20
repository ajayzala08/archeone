﻿using ArcheOne.Helper.CommonModels;
using System.Data;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ArcheOne.Helper.CommonHelpers
{
    public class CommonHelper
    {
        public const string DATE_FORMAT = "dd/MMM/yyyy";
        public const string SQL_SYS_DATE_FORMAT = "yyyy-MM-dd hh:mm:ss.fff"; //2023-03-16 15:07:50.413
        public const string SQL_DATE_FORMAT = "dd-MMM-yyyy hh:mm:ss.fff";    //16-Mar-2023 15:07:50.413
        private IConfiguration _configuration { get; }
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment { get; }
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommonHelper(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public CommonResponse UploadFile(IFormFile file, string subDirectory, string fileName, bool? IsTempFile = false, bool? IsSubDirectoryDateWise = false, bool? IsFileNameAutoGenerated = false)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                UploadFileResModel uploadResponse = new UploadFileResModel();
                DateTime CurrentDateTime = GetCurrentDateTime();
                string savePath = string.Empty;
                string CurrentDirectory = Directory.GetCurrentDirectory();
                subDirectory = subDirectory ?? string.Empty;
                subDirectory = IsTempFile != null && IsTempFile == true ? Path.Combine("Temp", subDirectory) : subDirectory;
                subDirectory = IsSubDirectoryDateWise != null && IsSubDirectoryDateWise == true ? Path.Combine(subDirectory) : subDirectory;
                //subDirectory = IsSubDirectoryDateWise != null && IsSubDirectoryDateWise == true ? Path.Combine(subDirectory, CurrentDateTime.Year.ToString(), CurrentDateTime.Month.ToString(), CurrentDateTime.Day.ToString()) : subDirectory;
                var target = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", "Files", subDirectory);
                if (!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                }
                FileInfo fileInfo = new FileInfo(file.FileName);
                string fileExtension = fileInfo.Extension;
                fileName = IsFileNameAutoGenerated != null && IsFileNameAutoGenerated == true ? Guid.NewGuid().ToString().ToLower() + fileExtension : fileName;
                savePath = Path.Combine("Files", subDirectory, fileName);
                var filePath = Path.Combine(target, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                uploadResponse.FileName = fileName;
                uploadResponse.RelativePath = savePath; // "/File/...";
                uploadResponse.PhysicalPath = filePath; // "D:/Folder/Folder/Folder/File.extension"

                response.Status = true;
                response.Message = "File Uploaded";
                response.Data = uploadResponse;
            }
            catch { throw; }
            return response;
        }
        public CommonResponse UploadFile(string file, string subDirectory, string fileName)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                string savePath = string.Empty;
                string CurrentDirectory = Directory.GetCurrentDirectory();
                subDirectory = subDirectory ?? string.Empty;
                var target = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", "Files", subDirectory);

                Directory.CreateDirectory(target);
                savePath = Path.Combine("/", subDirectory, fileName);
                var filePath = Path.Combine(target, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    //file.CopyTo(stream);
                    byte[] byteArray = Convert.FromBase64String(file.Split(',')[1]);
                    stream.Write(byteArray, 0, byteArray.Length);
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Status = true;
                response.Message = "File Uploaded";
                response.Data = savePath;
            }
            catch (Exception ex)
            {
                response.Data = ex;
                response.Message = ex.Message;
            }
            return response;
        }
        public string GetPhysicalRootPath(bool? IsIncludingFiles = true)
        {
            string directoryPath = IsIncludingFiles != null && IsIncludingFiles == true ? "/Files" : "";
            var physicalRootPath = _hostingEnvironment.WebRootPath + directoryPath;
            return physicalRootPath;
        }
        public string GetRelativeRootPath()
        {
            string directoryPath = "/files";
            string relativeRootPath = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host + directoryPath;
            return relativeRootPath;
        }
        public FilePaths GetFilePaths(string ModuleName, string extension, bool isTempFolder, string? fileName = "")
        {
            FilePaths filePaths = new FilePaths();
            var rootPath = _hostingEnvironment.WebRootPath;
            string directoryPath = "/files";
            if (isTempFolder)
            {
                directoryPath += "/temp";
            }
            string fileNameOnly = "_" + Guid.NewGuid() + "." + extension.ToString();
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                fileNameOnly = fileName + "." + extension.ToString();
            }

            directoryPath += "/" + ModuleName + "/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/";
            if (!Directory.Exists(rootPath + directoryPath))
            {
                Directory.CreateDirectory(rootPath + directoryPath);
            }

            var fileNameFull = directoryPath + fileNameOnly;

            filePaths.FilePhysicalPath = rootPath + fileNameFull;
            filePaths.FileRelativePath = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host + fileNameFull;
            filePaths.DirectoryPhysicalPath = rootPath + directoryPath;
            filePaths.DirectoryRelativePath = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host + directoryPath;
            return filePaths;
        }
        public string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();

            mimeType = "data:" + mimeType + ";base64,";
            return mimeType;
        }
        public CommonResponse SendEmail(SendEmailRequestModel model)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                if (!string.IsNullOrWhiteSpace(model.ToEmail))
                {
                    MailMessage mail = new MailMessage();

                    //get configration from appsettings.json
                    string FromEmail = _configuration.GetSection("SiteEmailConfigration:FromEmail").Value;
                    string Host = _configuration.GetSection("SiteEmailConfigration:Host").Value;
                    int Port = Convert.ToInt32(_configuration.GetSection("SiteEmailConfigration:Port").Value);
                    bool EnableSSL = Convert.ToBoolean(_configuration.GetSection("SiteEmailConfigration:EnableSSL").Value);
                    string Password = _configuration.GetSection("SiteEmailConfigration:MailPassword").Value;
                    bool EmailEnable = Convert.ToBoolean(_configuration.GetSection("SiteEmailConfigration:EmailEnable").Value);
                    if (EmailEnable)
                    {
                        mail.From = new MailAddress(FromEmail, "Arche");
                        mail.To.Add(new MailAddress(model.ToEmail));
                        mail.Subject = model.Subject;
                        mail.Body = model.Body;
                        mail.IsBodyHtml = true;
                        //if (model.Attachment != null)
                        //{

                        //    string path = Convert.ToString(model.Attachment);
                        //    Attachment attachment = new Attachment(path);
                        //    mail.Attachments.Add(attachment);
                        //}

                        if (model.Attachment != null)
                        {
                            mail.Attachments.Add(new Attachment(model.Attachment));
                        }

                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = Host;
                        smtp.Port = Port;
                        smtp.EnableSsl = EnableSSL;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(FromEmail, Password);
                        try
                        {
                            smtp.Send(mail);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }

                    response.Status = true;
                    response.Message = "Success.";
                }
                else
                {
                    response.Message = "Receiver Email Id Not Provided.";
                }
            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }
        public async Task AddActivityLog(string apiUrl, string methodType, string request, string requestResult)
        {
            try
            {
                bool APILogSwitch = Convert.ToBoolean(_configuration["CommonSwitches:APILogSwitch"].ToString());
                if (APILogSwitch)
                {
                    string logText = apiUrl + " (" + methodType + ") - Request : ( " + requestResult + " ) - Response : ( " + request + " ).";
                    AddLog(logText, CommonConstant.ActivityLog);
                }
            }
            catch { throw; }
        }
        public void AddExceptionLog(string exceptionText)
        {
            try
            {
                bool ExceptionLogSwitch = Convert.ToBoolean(_configuration["CommonSwitches:ExceptionLogSwitch"].ToString());
                if (ExceptionLogSwitch)
                {
                    AddLog(exceptionText, CommonConstant.ExceptionLog);
                }
            }
            catch { throw; }
        }
        public void AddLog(string text, string? logType = null)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                logType = logType != null ? logType : "";
                string logFileName = GetCurrentDateTime().ToString("dd/MM/yyyy").Replace('/', '_').ToString() + ".log";
                var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", "Logs", logType);
                var exists = Directory.Exists(filePath);
                if (!exists)
                {
                    Directory.CreateDirectory(filePath);
                }
                filePath = Path.Combine(filePath, logFileName);
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    text = GetCurrentDateTime().ToString() + " : " + text + "\n";
                    //writer.WriteLine(string.Format(text, GetCurrentDateTime().ToString("dd/MM/yyyy hh:mm:ss tt")));
                    writer.WriteLine(text);
                    writer.Close();
                }
            }
        }
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
        public string GetUpperCaseFirstAll(string value)
        {
            return value != null ? Regex.Replace(value, @"(^\w)|(\s\w)", m => m.Value.ToUpper()) : string.Empty;
        }
        public int GetLoggedInUserId()
        {
            var SessionValue = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            int UserId = !string.IsNullOrEmpty(SessionValue) ? Convert.ToInt32(SessionValue) : 0;
            return UserId;
        }
        public string GetUpdateQuery(string TableName, dynamic model)
        {
            string query = "Update " + TableName + " set ";
            string whereClause = string.Empty;

            int count = 0;
            foreach (var item in model)
            {
                if (item.Key.ToLower() != "id")
                {
                    if (count > 0)
                    {
                        query += ",";
                    }
                    query += $"{item.Key} = '{item.Value}'";
                    count++;
                }
                else
                {
                    whereClause = $" where {item.Key} = {item.Value}";
                }
            }
            query += whereClause;
            return query;
        }
        public string GetInsertQuery(string TableName, dynamic model)
        {
            string query = "insert into " + TableName + " (";
            Type ApplicantInfo = model.GetType();
            PropertyInfo[] properties = ApplicantInfo.GetProperties();
            var TotalParameters = properties.Count();
            int count = 0;
            bool IsInt = false;
            bool IsBool = false;
            bool IsDateTime = false;
            DateTime dateTime;
            foreach (PropertyInfo property in properties)
            {
                if (property.Name.ToString().ToLower() != "id")
                {
                    if (count > 0)
                    {
                        query += ", ";
                    }
                    query += property.Name.ToString();
                    count++;
                }
            }
            query += ") values (";
            count = 0;
            foreach (PropertyInfo property in properties)
            {
                if (property.Name.ToString().ToLower() != "id")
                {
                    IsInt = property.GetMethod.ReturnType.Name.ToLower().Contains("int");
                    IsBool = property.GetMethod.ReturnType.Name.ToLower().Contains("bool");
                    IsDateTime = property.GetMethod.ReturnType.Name.ToLower().Contains("datetime");
                    if (count > 0)
                    {
                        query += ", ";
                    }
                    if (!IsInt && !IsBool)
                    {
                        query += "'";
                    }
                    if (IsBool)
                    {
                        query += (model.GetType().GetProperty(property.Name).GetValue(model, null)) == true ? "1" : "0";
                    }
                    else if (IsDateTime)
                    {
                        dateTime = model.GetType().GetProperty(property.Name).GetValue(model, null);
                        query += dateTime.ToString(SQL_SYS_DATE_FORMAT);
                    }
                    else
                    {
                        query += model.GetType().GetProperty(property.Name).GetValue(model, null);
                    }
                    if (!IsInt && !IsBool)
                    {
                        query += "'";
                    }
                    count++;
                }
            }
            query += ")";
            return query;
        }
        public bool IsValidDecimal(string input)
        {
            string pattern = @"^\d+(\.\d+)?$";
            return Regex.IsMatch(input, pattern);
        }
        public bool IsValidDateTime(string input, string format, out DateTime results)
        {
            return DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out results);
        }
    }
}

