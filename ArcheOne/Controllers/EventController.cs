using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;

namespace ArcheOne.Controllers
{
    public class EventController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly CommonHelper _commonHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ArcheOneDbContext _dbContext;
        public EventController(DbRepo dbRepo, CommonHelper commonHelper, IWebHostEnvironment webHostEnvironment, ArcheOneDbContext dbContext)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult EventData()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                List<GetAllEventResModel> eventList = new List<GetAllEventResModel>();

               
               //var list = _dbRepo.EventList().ToList();
                var list = _dbRepo.EventList().Where(x => x.CreatedBy == _commonHelper.GetLoggedInUserId()).ToList();
                if (list.Count > 0)
                {
                    foreach (var x in list)
                    {
                        GetAllEventResModel getAllEventResModel = new GetAllEventResModel();

                        getAllEventResModel.title = x.Subject;
                        getAllEventResModel.start = x.StartDate.Value;
                        getAllEventResModel.end = x.EndDate.Value;
                        getAllEventResModel.color = x.ThemeColour;
                        getAllEventResModel.allDay = x.IsFullDay.Value;
                        eventList.Add(getAllEventResModel);
                    }
                   
                }
                commonResponse.Status = true;
                commonResponse.Message = "Success!";
                commonResponse.Data = eventList;

            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Data = ex.StackTrace;
            }
            return Json(commonResponse);
        }

        [HttpPost]
        public JsonResult AddEditEventData([FromBody] AddEventReqModel addEvent)
        {
            CommonResponse commonResponse = new CommonResponse();
            if (addEvent != null)
            {
                EventMst eventMst = new EventMst();
                eventMst.Subject = addEvent.Subject;
                eventMst.StartDate = addEvent.Start;
                eventMst.EndDate = addEvent.End;
                eventMst.Description = addEvent.Description;
                eventMst.IsFullDay = addEvent.IsFullDay;
                eventMst.ThemeColour = addEvent.TheamColor;
                eventMst.IsFullDay = addEvent.IsFullDay;
                eventMst.EventType = "Meeting";
                eventMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                eventMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                eventMst.CreatedDate = DateTime.Now;
                eventMst.UpdatedDate = DateTime.Now;
                eventMst.IsActive = true;
                eventMst.IsDelete = false;
                _dbContext.EventMsts.Add(eventMst);
                _dbContext.SaveChanges();

                commonResponse.Status = true;
                commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
                commonResponse.Message = "Add Event Successfully";

                return Json(commonResponse);
            }
            else
            {
                commonResponse.Status = false;
                commonResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                commonResponse.Message = "Please Enter Valid Data";
                return Json(commonResponse);
            }
        }

        public IActionResult Event()
        {
            return View();
        }

    }
}
