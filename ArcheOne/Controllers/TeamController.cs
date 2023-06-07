using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ArcheOne.Controllers
{
    public class TeamController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly CommonHelper _commonHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ArcheOneDbContext _dbContext;
        public TeamController(DbRepo dbRepo, CommonHelper commonHelper, IWebHostEnvironment webHostEnvironment, ArcheOneDbContext dbContext)
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
        public IActionResult Team()
        {
            List<SelectListItem> list = new List<SelectListItem>().ToList();
            var teamLeadList = _dbRepo.UserMstList().Select(x => new SelectListItem { Text = x.UserName, Value = x.Id.ToString() }).ToList();
            ViewBag.TeamLead = teamLeadList;



            List<SelectListItem> list1 = new List<SelectListItem>().ToList();
            var teamMemberList = _dbRepo.UserMstList().Select(x => new SelectListItem { Text = x.UserName, Value = x.Id.ToString() }).ToList();
            ViewBag.teamMember = teamMemberList;

            List<SelectListItem> list2 = new List<SelectListItem>().ToList();
            var clientList = _dbRepo.ClientList().Select(x => new SelectListItem { Text = x.ClientName, Value = x.Id.ToString() }).ToList();
            ViewBag.Client = clientList;


            return View();
        }
        public IActionResult TeamList()
        {
            return View(_dbRepo.TeamList().ToList());
        }

        public IActionResult AddEditTeam(int? Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            TeamMst teamMst = new TeamMst();
            var team = _dbRepo.TeamList().ToList();
            AddEditTeamReqViewModel addEditTeamReqViewModel = new AddEditTeamReqViewModel();

            List<SelectListItem> list = new List<SelectListItem>().ToList();
            var teamLeadList = _dbRepo.UserMstList().Select(x => new SelectListItem { Text = x.UserName, Value = x.Id.ToString() }).ToList();
            ViewBag.TeamLead = teamLeadList;



            List<SelectListItem> list1 = new List<SelectListItem>().ToList();
            var teamMemberList = _dbRepo.UserMstList().Select(x => new SelectListItem { Text = x.UserName, Value = x.Id.ToString() }).ToList();
            ViewBag.teamMember = teamMemberList;

            List<SelectListItem> list2 = new List<SelectListItem>().ToList();
            var clientList = _dbRepo.ClientList().Select(x => new SelectListItem { Text = x.ClientName, Value = x.Id.ToString() }).ToList();
            ViewBag.Client = clientList;

            try
            {
                //addEditTeamReqViewModel.TeamLeadId = 1;
                //addEditTeamReqViewModel.TeamMemberId = 1;

                //addEditTeamReqViewModel.TeamLeadId = 
                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Data = addEditTeamReqViewModel;
                commonResponse.Message = "Success";



            }
            catch { throw; }
            return View();




            //var UserList = _dbRepo.AllUserMstList();
            //TeamMst teamMst = new TeamMst();
            //try
            //{
            //    TeamMst team = new TeamMst();
            //    AddEditTeamReqViewModel addEditTeamReqViewModel = new AddEditTeamReqViewModel();

            //    foreach (var item in addEditTeamReqViewModel.TeamMemberId)
            //    {
            //        teamMst.TeamLeadId = addEditTeamReqViewModel.TeamLeadId;
            //        teamMst.TeamMemberId = item;
            //    }
            //    //_dbContext.TeamMsts.Add(teamMst);
            //    //_dbContext.SaveChanges();
            //    if (Id > 0)
            //    {
            //        var TeamDetails = _dbRepo.TeamList().FirstOrDefault(x => x.Id == Id);
            //        if (TeamDetails != null)
            //        {
            //            foreach (var item in addEditTeamReqViewModel.TeamMemberId)
            //            {
            //                TeamDetails.TeamLeadId = addEditTeamReqViewModel.TeamLeadId;
            //                TeamDetails.TeamMemberId = item;
            //            }
            //            //_dbContext.TeamMsts.Add(TeamDetails);
            //            //_dbContext.SaveChanges();
            //        }
            //    }
            //    commonResponse.Status = true;
            //    commonResponse.StatusCode = HttpStatusCode.OK;
            //    commonResponse.Message = "Success!";
            //    commonResponse.Data = addEditTeamReqViewModel;

            //}
            //catch
            //{
            //    throw;
            //}
            //return View(commonResponse.Data);
            return View();
        }

        public CommonResponse SaveUpdateTeam(TeamMst team)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                TeamMst teamMst = new TeamMst();
                bool IsDuplicate = false;
                var duplicateCheck = _dbRepo.TeamList().FirstOrDefault(x => x.Id != team.Id && x.TeamMemberId != team.TeamMemberId);
                IsDuplicate = duplicateCheck != null;
                if (!IsDuplicate)
                {
                    var teamDetails = _dbRepo.TeamList().FirstOrDefault(x => x.Id == team.Id);
                    if (teamDetails != null && teamDetails.Id > 0)
                    {
                        //Edit Mode
                        teamMst = team;
                        teamMst.CreatedDate = teamDetails.CreatedDate;
                        teamMst.CreatedBy = teamDetails.CreatedBy;
                        teamMst.IsActive = teamDetails.IsActive;
                        teamMst.IsDelete = teamDetails.IsDelete;

                        teamMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
                        teamMst.UpdatedBy = 1;

                        _dbContext.Entry(teamMst).State = EntityState.Modified;
                        _dbContext.SaveChanges();

                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Message = "User Updated Successfully!";
                    }
                    else
                    {
                        //Add Mode
                        teamMst = team;
                        teamMst.CreatedDate = _commonHelper.GetCurrentDateTime();
                        teamMst.UpdatedDate = _commonHelper.GetCurrentDateTime();
                        teamMst.CreatedBy = 1;
                        teamMst.UpdatedBy = 1;
                        teamMst.IsActive = true;
                        teamMst.IsDelete = false;
                        _dbContext.Add(teamMst);
                        _dbContext.SaveChanges();

                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Message = "Team Added Successfully!";
                    }
                }
                else
                {
                    commonResponse.Message = "Team Member Is Already Exist ";
                }
                commonResponse.Data = teamMst;
            }
            catch { throw; }
            return commonResponse;
        }

        public IActionResult DeleteTeam(int id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                var res = _dbRepo.TeamList().FirstOrDefault(x => x.Id == id);
                if (res != null)
                {
                    res.IsDelete = true;
                    res.UpdatedBy = _commonHelper.GetLoggedInUserId();
                    res.CreatedDate = _commonHelper.GetCurrentDateTime();
                    res.UpdatedDate = _commonHelper.GetCurrentDateTime();

                    _dbContext.Entry(res).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                }
                else
                {
                    commonResponse.Message = "Data not found!";
                    commonResponse.StatusCode = HttpStatusCode.NotFound;
                }
            }
            catch { throw; }
            return RedirectToAction("TeamList");
        }
    }
}
