using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

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
            //List<SelectListItem> list = new List<SelectListItem>().ToList();
            //var teamLeadList = _dbRepo.UserMstList().Select(x => new SelectListItem { Text = x.UserName, Value = x.Id.ToString() }).ToList();
            //ViewBag.TeamLead = teamLeadList;



            //List<SelectListItem> list1 = new List<SelectListItem>().ToList();
            //var teamMemberList = _dbRepo.UserMstList().Select(x => new SelectListItem { Text = x.UserName, Value = x.Id.ToString() }).ToList();
            //ViewBag.teamMember = teamMemberList;

            //List<SelectListItem> list2 = new List<SelectListItem>().ToList();
            //var clientList = _dbRepo.ClientList().Select(x => new SelectListItem { Text = x.ClientName, Value = x.Id.ToString() }).ToList();
            //ViewBag.Client = clientList;


            return View();
        }
        public IActionResult TeamList()
        {
            CommonResponse commonResponse = new CommonResponse();
            List<GetTeamListResModel> teamlists = new List<GetTeamListResModel>();
            GetTeamListResModel getTeamListResModel = new GetTeamListResModel();
            try
            {
                teamlists = (from z in _dbRepo.TeamList()
                             join f in _dbRepo.AllUserMstList() on z.TeamLeadId equals f.Id
                             join t in _dbRepo.AllUserMstList() on z.TeamMemberId equals t.Id
                             select new
                             {
                                 TeamId = z.TeamLeadId,
                                 TeamLeadName = f.FirstName + " " + f.LastName,
                                 TeamMemerName = t.FirstName + " " + t.LastName,
                                 TeamLeadId = z.TeamLeadId,
                                 TeamMemberId = z.TeamMemberId
                             }).ToList().GroupBy(g => g.TeamLeadName).Select((x, Index) => new GetTeamListResModel
                             {
                                 Id = Index + 1,
                                 TeamLead = x.Key,
                                 TeamName = string.Join(",", x.Select(x => x.TeamMemerName))
                             }).ToList();

                getTeamListResModel.TeamLeadId = teamlists[0].TeamLeadId;
                getTeamListResModel.TeamMemberId = teamlists[0].TeamMemberId;

            }
            catch { throw; }

            return View(teamlists);
        }

        [HttpGet]
        public IActionResult AddEditTeam(int Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            AddEditTeamReqViewModel addEditTeamReqViewModel = new AddEditTeamReqViewModel();
            List<TeamDetails> teamlists = new List<TeamDetails>();
            addEditTeamReqViewModel.TeamDetails = new TeamDetails();
            addEditTeamReqViewModel.TeamLeadList = _dbRepo.UserMstList().ToList();
            addEditTeamReqViewModel.TeamMemberList = _dbRepo.UserMstList().ToList();

            try
            {
                if (Id > 0)
                {
                    var UserDetails = _dbRepo.AllUserMstList().FirstOrDefault(x => x.Id == Id);
                    if (UserDetails != null)
                    {
                        //addEditTeamReqViewModel.TeamDetails.TeamId = Id;
                        //addEditTeamReqViewModel.TeamDetails.TeamLeadId = UserDetails.Id;
                        //addEditTeamReqViewModel.TeamDetails.TeamMemberId = UserDetails.Id;

                        teamlists = (from z in _dbRepo.TeamList()
                                     join f in _dbRepo.AllUserMstList() on z.TeamLeadId equals f.Id
                                     join t in _dbRepo.AllUserMstList() on z.TeamMemberId equals t.Id
                                     select new
                                     {
                                         TeamId = z.Id,
                                         TeamLeadName = f.FirstName + " " + f.LastName,
                                         TeamMemerName = t.FirstName + " " + t.LastName
                                     }).ToList().GroupBy(g => g.TeamLeadName).Select((x, Index) => new TeamDetails
                                     {
                                         TeamId = Index + 1,
                                         TeamLeadId = x.Key,
                                         TeamMemberId = string.Join(",", x.Select(x => x.TeamMemerName))
                                     }).ToList();
                    }
                }
                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Data = addEditTeamReqViewModel;
                commonResponse.Message = "Success";

            }
            catch { throw; }

            return View(commonResponse.Data);

        }

        [HttpPost]
        public async Task<CommonResponse> SaveUpdateTeam([FromBody] SaveUpdateTeam team)
        {
            CommonResponse commonResponse = new CommonResponse();
            TeamMst teamMst = new TeamMst();
            try
            {
                if (team != null)
                {
                    var teamDetails = _dbRepo.TeamList().FirstOrDefault(x => x.Id == team.TeamId);
                    if (teamDetails != null)
                    {
                        //foreach (var teamMember in team.TeamMemberId)
                        //{
                        teamDetails.TeamLeadId = team.TeamLeadId;
                        teamDetails.TeamMemberId = team.TeamMemberId;
                        teamDetails.IsActive = true;
                        teamDetails.IsDelete = false;
                        teamDetails.CreatedDate = DateTime.Now;
                        teamDetails.UpdatedDate = DateTime.Now;
                        teamDetails.CreatedBy = _commonHelper.GetLoggedInUserId();
                        teamDetails.UpdatedBy = _commonHelper.GetLoggedInUserId();

                        _dbContext.Entry(teamDetails).State = EntityState.Modified;
                        _dbContext.SaveChanges();

                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Message = "Success";
                        //}
                    }
                    else
                    {
                        //foreach (var teamMember in team.TeamMemberId)
                        //{
                        teamMst.TeamLeadId = team.TeamLeadId;
                        teamMst.TeamMemberId = team.TeamMemberId;
                        teamMst.IsActive = true;
                        teamMst.IsDelete = false;
                        teamMst.CreatedDate = DateTime.Now;
                        teamMst.UpdatedDate = DateTime.Now;
                        teamMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                        teamMst.UpdatedBy = _commonHelper.GetLoggedInUserId();

                        _dbContext.TeamMsts.Add(teamMst);
                        _dbContext.SaveChanges();
                        //}

                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Message = "Success";
                    }
                    commonResponse.Data = teamMst;
                }
                else
                {
                    commonResponse.Message = "Fail";
                    commonResponse.Status = false;
                }

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
