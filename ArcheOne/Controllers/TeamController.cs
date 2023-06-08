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

            try
            {
                teamlists = (from z in _dbRepo.TeamList()
                             join f in _dbRepo.AllUserMstList() on z.TeamLeadId equals f.Id
                             join t in _dbRepo.AllUserMstList() on z.TeamMemberId equals t.Id
                             select new
                             {
                                 TeamId = z.Id,
                                 TeamLeadName = f.FirstName + " " + f.LastName,
                                 TeamMemerName = t.FirstName + " " + t.LastName
                             }).ToList().GroupBy(g => g.TeamLeadName).Select((x,Index) => new GetTeamListResModel
                             {
                                 Id = Index +1,
                                 TeamLead = x.Key,
                                 TeamName = string.Join(",", x.Select(x => x.TeamMemerName))
                             }).ToList();
            }
            catch { throw; }

            return View(teamlists);
        }

        [HttpGet]
        public async Task<IActionResult> AddEditTeam(int Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            AddEditTeamReqViewModel addEditTeamReqViewModel = new AddEditTeamReqViewModel();
            addEditTeamReqViewModel.TeamId = Id;
            TeamDetails teamDetails = new TeamDetails();
            addEditTeamReqViewModel.TeamLeadList = _dbRepo.UserMstList().ToList();
            addEditTeamReqViewModel.TeamMemberList = _dbRepo.UserMstList().ToList();

            try
            {
                if (Id > 0)
                {
                    var UserDetails = _dbRepo.AllUserMstList().FirstOrDefault(x => x.Id == Id);
                    if (UserDetails != null)
                    {
                        addEditTeamReqViewModel.TeamId = Id;
                        addEditTeamReqViewModel.TeamDetails.TeamLeadId = UserDetails.Id;
                        addEditTeamReqViewModel.TeamDetails.TeamMemberId = UserDetails.Id;
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

        public CommonResponse SaveUpdateTeam(TeamMst team)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                TeamMst teamMst = new TeamMst();
                bool IsDuplicate = false;
                //var duplicateCheck = _dbRepo.TeamList().FirstOrDefault(x => x.TeamMemberId != team.TeamMemberId);
                //IsDuplicate = duplicateCheck != null;
                //if (!IsDuplicate)
                //{
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
                        commonResponse.Message = "success";
                    }
                //}
                //else
                //{
                //    commonResponse.Message = "Team Member Is Already Exist ";
                //}
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
