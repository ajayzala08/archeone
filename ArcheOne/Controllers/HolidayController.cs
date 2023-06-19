﻿using ArcheOne.Database.Entities;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ArcheOneDbContext _dbContext;
        public HolidayController(DbRepo dbRepo, CommonHelper commonHelper, IWebHostEnvironment webHostEnvironment, ArcheOneDbContext dbContext)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Holiday()
        {
            return View();
        }

        public IActionResult HolidayList()
        {
            CommonResponse commonResponse = new CommonResponse();

            GetHolidayListResModel getHolidayListResModelList = new GetHolidayListResModel();
            HolidayMst holidayMst = new HolidayMst();
            var holidayList = _dbRepo.HolidayDayList().ToList();
            try
            {
                if (holidayList.Count > 0)
                {
                    List<GetHolidayListResModel> getHolidayListResModel = new List<GetHolidayListResModel>();
                    getHolidayListResModel = _dbRepo.HolidayDayList().Where(x => x.IsActive == true && x.IsDelete == false).Select(x => new GetHolidayListResModel
                    {
                        Id = x.Id,
                        HolidayName = x.HolidayName,
                        Date = x.HolidayDate.Date.ToString("dd-MM-yyyy"),
                        Day  =x.HolidayDate.DayOfWeek.ToString()
                       
                    }).ToList();
                    commonResponse.Data = getHolidayListResModel;

                    commonResponse.Status = true;
                    commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
                    commonResponse.Data = getHolidayListResModel;
                    commonResponse.Message = "GetAll HolidayList Successfully";
                }
                else
                {
                    commonResponse.Message = "No Data Found";
                    commonResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                }


            }
            catch (Exception ex)
            {
                commonResponse.Data = ex.Message;
                commonResponse.Status = false;
            }
            return View(commonResponse.Data);
        }

        public IActionResult AddEditHoliday(int Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            AddEditHolidayReqModel addEditHolidayReqModel = new AddEditHolidayReqModel();

            HolidayMst holidayMst = new HolidayMst();
            try
            {
                var holidayList = _dbRepo.HolidayDayList().FirstOrDefault(x => x.Id == Id);
                if (Id > 0)
                {
                    addEditHolidayReqModel.Id = holidayList.Id;
                    addEditHolidayReqModel.HolidayName = holidayList.HolidayName;
                    addEditHolidayReqModel.HolidayDate = holidayList.HolidayDate;
                }
                commonResponse.Status = true;
                commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
                commonResponse.Message = "Success";
                commonResponse.Data = addEditHolidayReqModel;

            }
            catch (Exception ex)
            {
                commonResponse.Data = ex.Data;
                commonResponse.Message = ex.Message;
            }
            return View(commonResponse.Data);
        }


        [HttpPost]
        public async Task<CommonResponse> SaveUpdateHoliday([FromBody] SaveUpdateHolidayReqModel saveUpdateHolidayReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            AddEditHolidayReqModel addEditHolidayReqModel = new AddEditHolidayReqModel();
            HolidayMst holidayMst = new HolidayMst();
            var holidayDetails = _dbRepo.HolidayDayList().FirstOrDefault(x => x.Id == saveUpdateHolidayReqModel.Id);
            try
            {
             
                if (holidayDetails != null)
                {

                    holidayDetails.HolidayName = saveUpdateHolidayReqModel.HolidayName;
                    holidayDetails.HolidayDate = saveUpdateHolidayReqModel.HolidayDate;
                    holidayDetails.IsActive = true;
                    holidayDetails.IsDelete = false;
                    holidayDetails.CreatedDate = DateTime.Now;
                    holidayDetails.UpdatedDate = DateTime.Now;
                    holidayDetails.CreatedBy = _commonHelper.GetLoggedInUserId();
                    holidayDetails.UpdatedBy = _commonHelper.GetLoggedInUserId();

                    _dbContext.Entry(holidayDetails).State = EntityState.Modified;
                    _dbContext.SaveChanges();


                    commonResponse.Status = true;
                    commonResponse.StatusCode = HttpStatusCode.OK;
                    commonResponse.Message = "Success";

                }
                else
                {
                    var checkHolidayName = _dbRepo.HolidayDayList().Where(x => x.HolidayName == saveUpdateHolidayReqModel.HolidayName);
                    if (checkHolidayName.Count() > 0)
                    {
                        commonResponse.Message = "Holiday Is Already Exist";
                        commonResponse.StatusCode = HttpStatusCode.BadRequest;
                    }
                    else
                    {
                        holidayMst.HolidayName = saveUpdateHolidayReqModel.HolidayName;
                        holidayMst.HolidayDate = saveUpdateHolidayReqModel.HolidayDate;
                        holidayMst.IsActive = true;
                        holidayMst.IsDelete = false;
                        holidayMst.CreatedDate = DateTime.Now;
                        holidayMst.UpdatedDate = DateTime.Now;
                        holidayMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                        holidayMst.UpdatedBy = _commonHelper.GetLoggedInUserId();

                        _dbContext.HolidayMsts.Add(holidayMst);
                        _dbContext.SaveChanges();

                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Message = "Success";
                    }
                   
                }
                commonResponse.Data = holidayMst;

            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Data = ex.StackTrace;
            }
            return commonResponse;

        }
        public async Task<IActionResult> DeleteHoliday(int id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                if (id > 0)
                {
                    var holidayList = await _dbRepo.HolidayDayList().FirstOrDefaultAsync(x => x.Id == id);
                    if (holidayList != null)
                    {
                        _dbContext.Remove(holidayList);
                        _dbContext.SaveChanges();

                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Message = "Holiday Deleted Successfully";

                    }
                    else
                    {
                        commonResponse.Message = "Data not found!";
                        commonResponse.StatusCode = HttpStatusCode.NotFound;
                    }
                }
                else
                {
                    commonResponse.Message = "Data not found!";
                    commonResponse.StatusCode = HttpStatusCode.NotFound;
                }
            }
            catch { throw; }
            return Json(commonResponse);
        }

    }
}