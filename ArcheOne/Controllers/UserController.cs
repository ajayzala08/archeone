using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;
using System.Transactions;

namespace ArcheOne.Controllers
{
    public class UserController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly CommonHelper _commonHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ArcheOneDbContext _dbContext;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostEnvironment;
        public UserController(DbRepo dbRepo, CommonHelper commonHelper, IWebHostEnvironment webHostEnvironment, ArcheOneDbContext dbContext, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> User()
        {
            return View();
        }

        public async Task<IActionResult> AddEditUser(int Id)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                UserAddEditReqViewModel userAddEditReqViewModel = new UserAddEditReqViewModel();
                userAddEditReqViewModel.UserDetails = new UserDetail();
                userAddEditReqViewModel.RoleList = _dbRepo.RoleMstList().ToList();
                userAddEditReqViewModel.DepartmentList = _dbRepo.DepartmentList().ToList();
                if (Id > 0)
                {
                    var userDetails = await _dbRepo.AllUserMstList().FirstOrDefaultAsync(x => x.Id == Id);
                    if (userDetails != null)
                    {
                        var designationJsonResponse = await GetDesignationListByRoleAndDepartment(userDetails.RoleId, userDetails.DepartmentId);

                        userAddEditReqViewModel.DesignationList = designationJsonResponse.Data;

                        userAddEditReqViewModel.UserDetails.Id = userDetails.Id;
                        userAddEditReqViewModel.UserDetails.CompanyId = userDetails.CompanyId;
                        userAddEditReqViewModel.UserDetails.FirstName = userDetails.FirstName;
                        userAddEditReqViewModel.UserDetails.MiddleName = userDetails.MiddleName;
                        userAddEditReqViewModel.UserDetails.LastName = userDetails.LastName;
                        userAddEditReqViewModel.UserDetails.UserName = userDetails.UserName;
                        userAddEditReqViewModel.UserDetails.Address = userDetails.Address;
                        userAddEditReqViewModel.UserDetails.Pincode = userDetails.Pincode;
                        userAddEditReqViewModel.UserDetails.Mobile1 = userDetails.Mobile1;
                        userAddEditReqViewModel.UserDetails.Mobile2 = userDetails.Mobile2;
                        userAddEditReqViewModel.UserDetails.Email = userDetails.Email;
                        userAddEditReqViewModel.UserDetails.PhotoUrl = userDetails.PhotoUrl;
                        userAddEditReqViewModel.UserDetails.RoleId = userDetails.RoleId;
                        userAddEditReqViewModel.UserDetails.DepartmentId = userDetails.DepartmentId;
                        userAddEditReqViewModel.UserDetails.DesignationId = userDetails.DesignationId;
                        userAddEditReqViewModel.UserDetails.IsActive = userDetails.IsActive;
                    }


                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Data found successfully!";
                    response.Data = userAddEditReqViewModel;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "Data not found!";
                }

                response.Data = userAddEditReqViewModel;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return View(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveUpdateUser(UserSaveUpdateReqModel request)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                int userId = _commonHelper.GetLoggedInUserId();
                DateTime currentDateTime = _commonHelper.GetCurrentDateTime();
                IFormFile file;
                string fileName = string.Empty;
                bool validateFileExtension = false;
                bool validateFileSize = false;
                string filePath = string.Empty;
                UserMst userMst = new UserMst();
                if (request.PhotoUrl != null)
                {
                    file = request.PhotoUrl;
                    fileName = file.FileName;
                    FileInfo fileInfo = new FileInfo(fileName);
                    string fileExtension = fileInfo.Extension;
                    long fileSize = file.Length;

                    string[] allowedFileExtensions = { CommonConstant.jpeg, CommonConstant.png, CommonConstant.jpg };
                    long allowedFileSize = 1 * 1024 * 1024 * 10; // 10MB
                    validateFileExtension = allowedFileExtensions.Contains(fileExtension) ? true : false;
                    validateFileSize = fileSize <= allowedFileSize ? true : false;
                    fileName = request.UserName + request.Id + fileExtension;
                    if (validateFileExtension && validateFileSize)
                    {
                        var imageFile = _commonHelper.UploadFile(request.PhotoUrl, @"UserProfile", fileName, false, true, false);
                        filePath = imageFile.Data.RelativePath;
                    }
                    else
                    {
                        response.Message = "Only jpg and png files are Allowed !";
                    }
                }
                //var userDetail = await _dbRepo.AllUserMstList().FirstOrDefaultAsync(x => x.Id == request.Id && x.Email != request.Email && x.Mobile1 != request.Mobile1);
                var userDetail = await _dbRepo.AllUserMstList().FirstOrDefaultAsync(x => x.Id == request.Id);
                if (userDetail != null && userDetail.Id > 0)
                {
                    bool duplicateCheck = await _dbRepo.AllUserMstList().Where(x => (x.Id != request.Id) && (x.UserName == request.UserName || x.Email == request.Email || x.Mobile1 == request.Mobile1)).AnyAsync();
                    if (!duplicateCheck)
                    {
                        //Edit Mode
                        userDetail.RoleId = request.RoleId;
                        userDetail.DepartmentId = request.DepartmentId;
                        userDetail.DesignationId = request.DesignationId;
                        userDetail.CompanyId = _dbRepo.GetLoggedInUserDetails().CompanyId;
                        userDetail.FirstName = request.FirstName;
                        userDetail.MiddleName = request.MiddleName;
                        userDetail.LastName = request.LastName;
                        userDetail.Address = (Convert.ToString(request.Address) != "" && request.Address != null) ? request.Address : "NA";
                        userDetail.Pincode = request.Pincode;
                        userDetail.Mobile1 = request.Mobile1;
                        userDetail.Mobile2 = (Convert.ToString(request.Mobile2) != "" && request.Mobile2 != null) ? request.Mobile2 : "NA";
                        userDetail.Email = request.Email;
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            userDetail.PhotoUrl = filePath;
                        }
                        userDetail.UpdatedDate = currentDateTime;
                        userDetail.UpdatedBy = userId;

                        _dbContext.Entry(userDetail).State = EntityState.Modified;
                        await _dbContext.SaveChangesAsync();

                        response.Status = true;
                        response.StatusCode = HttpStatusCode.OK;
                        response.Message = "User Updated Successfully!";
                    }
                    else
                    {
                        response.Message = "UserName, Email OR Contact Already Exist";
                    }
                }
                else
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        //Add Mode
                        var duplicateCheck = await _dbRepo.AllUserMstList().Where(x => x.UserName == request.UserName || x.Email == request.Email || x.Mobile1 == request.Mobile1).ToListAsync();
                        if (duplicateCheck.Count == 0)
                        {
                            var encryptedPassword = _commonHelper.EncryptString(request.Password);

                            userMst.RoleId = request.RoleId;
                            userMst.DepartmentId = request.DepartmentId;
                            userMst.DesignationId = request.DesignationId;
                            userMst.CompanyId = _dbRepo.GetLoggedInUserDetails().CompanyId;
                            userMst.FirstName = request.FirstName;
                            userMst.MiddleName = request.MiddleName;
                            userMst.LastName = request.LastName;
                            userMst.UserName = request.UserName;
                            userMst.Password = encryptedPassword;
                            userMst.Address = (Convert.ToString(request.Address) != "" && request.Address != null) ? request.Address : "NA";
                            userMst.Pincode = request.Pincode;
                            userMst.Mobile1 = request.Mobile1;
                            userMst.Mobile2 = (Convert.ToString(request.Mobile2) != "" && request.Mobile2 != null) ? request.Mobile2 : "NA";
                            userMst.Email = request.Email;
                            userMst.PhotoUrl = filePath;
                            userMst.CreatedDate = currentDateTime;
                            userMst.UpdatedDate = currentDateTime;
                            userMst.CreatedBy = userId;
                            userMst.UpdatedBy = userId;
                            userMst.IsActive = true;
                            userMst.IsDelete = false;

                            await _dbContext.AddAsync(userMst);
                            await _dbContext.SaveChangesAsync();

                            var defaultPermission = await _dbRepo.DefaultPermissionList().Where(x => x.RoleId == request.RoleId).ToListAsync();

                            var userPermission = (from a in defaultPermission
                                                  select new UserPermission
                                                  {
                                                      UserId = userMst.Id,
                                                      PermissionId = a.PermissionId,
                                                      IsActive = a.IsActive,
                                                      IsDelete = a.IsDelete,
                                                      CreatedBy = userId,
                                                      UpdatedBy = userId,
                                                      CreatedDate = currentDateTime,
                                                      UpdatedDate = currentDateTime
                                                  }).ToList() ?? new List<UserPermission>();

                            await _dbContext.UserPermissions.AddRangeAsync(userPermission);
                            await _dbContext.SaveChangesAsync();

                            scope.Complete();

                            response.Status = true;
                            response.StatusCode = HttpStatusCode.OK;
                            response.Message = "User Added Successfully!";
                        }
                        else
                        {
                            response.Message = "UserName, Email OR Contact Already Exist";
                        }
                    }
                }
                response.Data = userMst;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Json(response);
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var isUserExist = await _dbRepo.AllUserMstList().FirstOrDefaultAsync(x => x.Id == id);
                if (isUserExist != null)
                {
                    isUserExist.IsDelete = true;
                    isUserExist.UpdatedBy = _commonHelper.GetLoggedInUserId();
                    isUserExist.UpdatedDate = _commonHelper.GetCurrentDateTime();

                    _dbContext.Entry(isUserExist).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();

                    response.Status = true;
                    response.Message = "User Deleted Successfully!";
                    response.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    response.Message = "User not found!";
                    response.StatusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Json(response);
        }

        public async Task<IActionResult> UserList()
        {
            CommonResponse response = new CommonResponse();
            try
            {
                dynamic userList;

                userList = (from U in await _dbRepo.AllUserMstList().ToListAsync()
                            join C in _dbRepo.CompanyMstList()
                                               on U.CompanyId equals C.Id
                            join R in _dbRepo.RoleMstList()
                            on U.RoleId equals R.Id
                            select new { U, C, R })
                     .Select(x => new UserListModel
                     {
                         Id = x.U.Id,
                         CompanyId = x.C.CompanyName,
                         RoleId = x.R.RoleName,
                         FullName = x.U.FirstName + ' ' + x.U.MiddleName + ' ' + x.U.LastName,
                         UserName = x.U.UserName,
                         Password = x.U.Password,
                         Address = x.U.Address,
                         Pincode = x.U.Pincode,
                         Mobile1 = x.U.Mobile1,
                         Mobile2 = x.U.Mobile2,
                         Email = x.U.Email,
                         PhotoUrl = System.IO.File.Exists(Path.Combine(_commonHelper.GetPhysicalRootPath(false), x.U.PhotoUrl)) ? Path.Combine(@"\", x.U.PhotoUrl) :
                          @"\Theme\Logo\default_user_profile.png"
                     }).OrderByDescending(x => x.Id).ToList();
                if (userList != null && userList.Count > 0)
                {
                    response.Data = userList;
                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Data found successfully!";
                }
                else
                {
                    response.Message = "Data not found!";
                    response.StatusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Json(response);
        }

        public async Task<IActionResult> UserListByRoleId(int? RoleId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                dynamic userList;
                if (RoleId == null)
                {
                    userList = await _dbRepo.UserMstList().Select(x => new { x.Id, x.FirstName, x.MiddleName, x.LastName }).ToListAsync();
                }
                else
                {
                    userList = await _dbRepo.UserMstList().Where(x => x.RoleId == RoleId).Select(x => new { x.Id, x.FirstName, x.MiddleName, x.LastName }).ToListAsync();
                }


                if (userList != null && userList.Count > 0)
                {
                    response.Data = userList;
                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Data found successfully!";
                }
                else
                {
                    response.Message = "Data not found!";
                    response.StatusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Json(response);
        }

        public async Task<IActionResult> GetDesignationByRoleAndDepartment(int RoleId, int? DepartmentId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                response = await GetDesignationListByRoleAndDepartment(RoleId, DepartmentId);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Json(response);
        }

        private async Task<CommonResponse> GetDesignationListByRoleAndDepartment(int RoleId, int? DepartmentId)
        {
            CommonResponse response = new CommonResponse();
            List<DesignationDetails> designationListByRoleId = await _dbRepo.DesignationList().Where(x => x.RoleId == RoleId).Select(x => new DesignationDetails
            {
                Id = x.Id,
                Designation = x.Designation,
                RoleId = x.RoleId,
                DepartmentId = x.DepartmentId
            }).ToListAsync();
            var designationList = designationListByRoleId.Where(x => DepartmentId != null && DepartmentId != 0 ? x.DepartmentId == DepartmentId : true).ToList();
            if (designationList == null || (designationList != null && designationList.Count <= 0))
            {
                designationList = designationListByRoleId.Where(x => x.DepartmentId == 0).ToList();
            }

            if (designationList != null && designationList.Count > 0)
            {
                response.Data = designationList;
                response.Status = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Data found successfully!";
            }
            else
            {
                response.Message = "Data not found!";
                response.StatusCode = HttpStatusCode.NotFound;
            }
            return response;
        }

        [HttpPost]
        public async Task<IActionResult> UploadUserSheet(UploadUserSheetReqModel uploadUserSheetReqModel)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                IExcelDataReader reader;
                // string dataFileName = System.IO.Path.GetFileName(uploadUserSheetReqModel.UserSheet.FileName);

                string extension = System.IO.Path.GetExtension(uploadUserSheetReqModel.UserSheet.FileName);
                Stream stream = uploadUserSheetReqModel.UserSheet.OpenReadStream();
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
                        System.Data.DataTable userDataTable = ds.Tables[0];
                        //string companyName = !string.IsNullOrEmpty(userDataTable.Rows[0][1].ToString()) ? userDataTable.Rows[0][1].ToString() : "";
                        //if (companyName != null && companyName != "")
                        //{
                        //    var compneyDetailes = _dbRepo.CompanyMstList().FirstOrDefault(x => x.CompanyName.ToLower() == companyName.ToLower());

                        //    int comnpayId = companyName != "" && companyName != null ? compneyDetailes.Id : 0;
                        //    if (userDataTable.Rows[1][5] != null && Convert.ToString(userDataTable.Rows[1][5]) != "")
                        //    {
                        //        string[] yearMonth = Convert.ToString(userDataTable.Rows[1][5]).Split('-');
                        //        string userMonth = yearMonth[0];
                        //        int userYear = int.Parse(yearMonth[1]);
                        string userName = !string.IsNullOrEmpty(userDataTable.Rows[0][3].ToString()) ? userDataTable.Rows[0][3].ToString() : "";
                        string email = !string.IsNullOrEmpty(userDataTable.Rows[0][9].ToString()) ? userDataTable.Rows[0][9].ToString() : "";
                        string Mobile = !string.IsNullOrEmpty(userDataTable.Rows[0][7].ToString()) ? userDataTable.Rows[0][7].ToString() : "";

                        if (userName != null && userName != "" || email != null && email != "" || Mobile != null && Mobile != "")
                        {
                            var duplicateCheck = await _dbRepo.AllUserMstList().Where(x => x.UserName == userName || x.Email == email || x.Mobile1 == Mobile).ToListAsync();
                            if (duplicateCheck.Count == 0)
                            {

                                List<UserMst> users = new List<UserMst>();
                                for (int i = 1; i < userDataTable.Rows.Count; i++)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(userDataTable.Rows[i][3])) && Convert.ToString(userDataTable.Rows[i][3]).All(char.IsDigit) || !string.IsNullOrEmpty(Convert.ToString(userDataTable.Rows[i][9])) && Convert.ToString(userDataTable.Rows[i][9]).All(char.IsDigit) || !string.IsNullOrEmpty(Convert.ToString(userDataTable.Rows[i][7])) && Convert.ToString(userDataTable.Rows[i][7]).All(char.IsDigit))
                                    {
                                        users.Add(new UserMst
                                        {
                                            FirstName = !string.IsNullOrEmpty(Convert.ToString(userDataTable.Rows[i][0])) ? Convert.ToString(userDataTable.Rows[i][0]) : "",
                                            MiddleName = !string.IsNullOrEmpty(Convert.ToString(userDataTable.Rows[i][1])) ? Convert.ToString(userDataTable.Rows[i][1]) : "",
                                            LastName = !string.IsNullOrEmpty(Convert.ToString(userDataTable.Rows[i][2])) ? Convert.ToString(userDataTable.Rows[i][2]) : "",
                                            UserName = !string.IsNullOrEmpty(Convert.ToString(userDataTable.Rows[i][3])) ? Convert.ToString(userDataTable.Rows[i][3]) : "",
                                            Password = !string.IsNullOrEmpty(Convert.ToString(userDataTable.Rows[i][4])) ? Convert.ToString(userDataTable.Rows[i][4]) : "",
                                            Address = !string.IsNullOrEmpty(Convert.ToString(userDataTable.Rows[i][5])) ? Convert.ToString(userDataTable.Rows[i][5]) : "",
                                            Pincode = !string.IsNullOrEmpty(Convert.ToString(userDataTable.Rows[i][6])) ? Convert.ToString(userDataTable.Rows[i][6]) : "",
                                            Mobile1 = !string.IsNullOrEmpty(Convert.ToString(userDataTable.Rows[i][7])) ? Convert.ToString(userDataTable.Rows[i][7]) : "",
                                            Mobile2 = !string.IsNullOrEmpty(Convert.ToString(userDataTable.Rows[i][8])) ? Convert.ToString(userDataTable.Rows[i][8]) : "",
                                            Email = !string.IsNullOrEmpty(Convert.ToString(userDataTable.Rows[i][9])) ? Convert.ToString(userDataTable.Rows[i][9]) : "",
                                            PhotoUrl = !string.IsNullOrEmpty(Convert.ToString(userDataTable.Rows[i][10])) ? Convert.ToString(userDataTable.Rows[i][10]) : "",
                                            IsActive = true,
                                            IsDelete = false,
                                            CreatedBy = _commonHelper.GetLoggedInUserId(),
                                            CreatedDate = _commonHelper.GetCurrentDateTime(),
                                            UpdatedBy = _commonHelper.GetLoggedInUserId(),
                                            UpdatedDate = _commonHelper.GetCurrentDateTime(),
                                        });
                                    }
                                    else
                                    {
                                        response.Message = "UserName, Email OR Contact Already Exist";
                                    }
                                }
                                await _dbContext.UserMsts.AddRangeAsync(users);
                                await _dbContext.SaveChangesAsync();
                                response.Status = true;
                                response.StatusCode = HttpStatusCode.OK;
                                response.Data = users;
                                response.Message = "User sheet uploaded successfully";

                            }
                            else
                            {
                                response.Message = "UserName, Email OR Contact Already Exist";
                            }
                        }
                        else
                        {
                            response.Message = "No records found";
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

    }
}
