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
    public class HolidayController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly CommonHelper _commonHelper;
        private readonly ArcheOneDbContext _dbContext;
        private readonly CommonController _commonController;

        public HolidayController(DbRepo dbRepo, CommonHelper commonHelper, ArcheOneDbContext dbContext, CommonController commonController)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
            _dbContext = dbContext;
            _commonController = commonController;
        }

        public IActionResult Holiday()
        {
            bool showAddHolidayButton = _commonHelper.CheckHasPermission(CommonEnums.PermissionMst.Holidays_Add_View);

            return View(showAddHolidayButton);
        }

        public async Task<IActionResult> HolidayList()
        {
            CommonResponse response = new CommonResponse();
            GetHolidayListResModel getHolidayListResModel = new GetHolidayListResModel();
            try
            {
                getHolidayListResModel.HolidayDetails = new List<GetHolidayListResModel.HolidayDetail>();

                getHolidayListResModel.IsEditable = _commonHelper.CheckHasPermission(CommonEnums.PermissionMst.Holidays_Edit_View);
                getHolidayListResModel.IsDeletable = _commonHelper.CheckHasPermission(CommonEnums.PermissionMst.Holidays_Delete_View);

                getHolidayListResModel.HolidayDetails = await _dbRepo.HolidayDayList().Select(x => new GetHolidayListResModel.HolidayDetail
                {
                    Id = x.Id,
                    HolidayName = x.HolidayName,
                    Date = x.HolidayDate.Date.ToString("dd-MM-yyyy"),
                    Day = x.HolidayDate.DayOfWeek.ToString()

                }).ToListAsync();

                if (getHolidayListResModel.HolidayDetails.Count > 0)
                {
                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Data = getHolidayListResModel;
                    response.Message = "Data found successfully!";
                }
                else
                {
                    response.Data = getHolidayListResModel;
                    response.Message = "No Data Found!";
                    response.StatusCode = HttpStatusCode.NotFound;
                }

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return View(getHolidayListResModel);
        }

        public async Task<IActionResult> AddEditHoliday(int Id)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                AddEditHolidayReqModel addEditHolidayReqModel = new AddEditHolidayReqModel();
                if (Id > 0)
                {
                    addEditHolidayReqModel = await _dbRepo.HolidayDayList().Where(x => x.Id == Id).Select(x => new AddEditHolidayReqModel { Id = x.Id, HolidayName = x.HolidayName, HolidayDate = x.HolidayDate }).FirstOrDefaultAsync();
                    if (addEditHolidayReqModel != null)
                    {
                        response.Status = true;
                        response.StatusCode = HttpStatusCode.OK;
                        response.Message = "Data found successfully!";
                        response.Data = addEditHolidayReqModel;
                    }
                    else
                    {
                        response.Data = addEditHolidayReqModel;
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = "Data not found!";
                    }
                }
                else
                {
                    addEditHolidayReqModel.HolidayDate = _commonHelper.GetCurrentDateTime().Date;

                    response.Status = true;
                    response.Data = addEditHolidayReqModel;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Data found successfully!";
                }

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return View(response.Data);
        }


        [HttpPost]
        public async Task<IActionResult> SaveUpdateHoliday([FromBody] SaveUpdateHolidayReqModel saveUpdateHolidayReqModel)
        {
            CommonResponse response = new CommonResponse();
            HolidayMst holidayMst = new HolidayMst();

            try
            {
                if (saveUpdateHolidayReqModel != null)
                {

                    if (_commonHelper.GetCurrentDateTime().Year == saveUpdateHolidayReqModel.HolidayDate.Year)
                    {
                        var holidayDetails = await _dbRepo.HolidayDayList().FirstOrDefaultAsync(x => x.Id == saveUpdateHolidayReqModel.Id);
                        if (holidayDetails != null)
                        {
                            //Edit Mode
                            var checkHolidayName = await _dbRepo.HolidayDayList().Where(x => x.HolidayName == saveUpdateHolidayReqModel.HolidayName && x.Id != saveUpdateHolidayReqModel.Id).ToListAsync();
                            if (checkHolidayName.Count() > 0)
                            {
                                response.Message = "Holiday Is Already Exist";
                                response.StatusCode = HttpStatusCode.BadRequest;
                            }
                            else
                            {

                                holidayDetails.HolidayName = saveUpdateHolidayReqModel.HolidayName;
                                holidayDetails.HolidayDate = saveUpdateHolidayReqModel.HolidayDate;
                                holidayDetails.IsActive = true;
                                holidayDetails.IsDelete = false;
                                holidayDetails.CreatedDate = _commonHelper.GetCurrentDateTime();
                                holidayDetails.UpdatedDate = _commonHelper.GetCurrentDateTime();
                                holidayDetails.CreatedBy = _commonHelper.GetLoggedInUserId();
                                holidayDetails.UpdatedBy = _commonHelper.GetLoggedInUserId();

                                _dbContext.Entry(holidayDetails).State = EntityState.Modified;
                                await _dbContext.SaveChangesAsync();

                                response.Status = true;
                                response.StatusCode = HttpStatusCode.OK;
                                response.Message = "Holiday Edited Successfully";
                            }

                        }
                        else
                        {
                            //Add Mode
                            var checkHolidayName = await _dbRepo.HolidayDayList().Where(x => x.HolidayName == saveUpdateHolidayReqModel.HolidayName).ToListAsync();
                            if (checkHolidayName.Count() > 0)
                            {
                                response.Message = "Holiday Is Already Exist";
                                response.StatusCode = HttpStatusCode.BadRequest;
                            }
                            else
                            {
                                holidayMst.HolidayName = saveUpdateHolidayReqModel.HolidayName;
                                holidayMst.HolidayDate = saveUpdateHolidayReqModel.HolidayDate;
                                holidayMst.IsActive = true;
                                holidayMst.IsDelete = false;
                                holidayMst.CreatedDate = _commonHelper.GetCurrentDateTime();
                                holidayMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
                                holidayMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                                holidayMst.UpdatedBy = _commonHelper.GetLoggedInUserId();

                                await _dbContext.HolidayMsts.AddAsync(holidayMst);
                                await _dbContext.SaveChangesAsync();
                                /*
                                 *Add Holiday into events
                                AddEventReqModel addEventReqModel = new AddEventReqModel()
                                {
                                    Subject = saveUpdateHolidayReqModel.HolidayName,
                                    Start = saveUpdateHolidayReqModel.HolidayDate,
                                    End = saveUpdateHolidayReqModel.HolidayDate,
                                    Description = saveUpdateHolidayReqModel.HolidayName,
                                    IsFullDay = true,
                                    EventType = "Holiday",
                                    TheamColor = "Red"
                                };
                                var addEventResponse = _commonController.AddEvent(addEventReqModel);*/


                                response.Status = true;
                                response.StatusCode = HttpStatusCode.OK;
                                response.Message = "Holiday Added Successfully";
                            }

                        }
                        response.Data = holidayMst;
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Not allows to past and future date";
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = "Please enter the valid Date formatted!";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Json(response);

        }

        public async Task<IActionResult> DeleteHoliday(int id)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                if (id > 0)
                {
                    var holidayList = await _dbRepo.HolidayDayList().FirstOrDefaultAsync(x => x.Id == id);
                    if (holidayList != null)
                    {
                        _dbContext.Remove(holidayList);
                        await _dbContext.SaveChangesAsync();

                        response.Status = true;
                        response.StatusCode = HttpStatusCode.OK;
                        response.Message = "Holiday deleted successfully!";

                    }
                    else
                    {
                        response.Message = "Data not found!";
                        response.StatusCode = HttpStatusCode.NotFound;
                    }
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

    }
}
