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
                             //select new { z,f,t}).ToList().Select new
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
                                 TeamName = string.Join(",", x.Select(x => x.TeamMemerName)),
                                 TeamLeadId = x.Select(x => x.TeamLeadId).FirstOrDefault(),
                                 TeamMemberId = x.Select(x => x.TeamMemberId).FirstOrDefault()

                             }).ToList();

                if (teamlists.Count > 0)
                {
                    getTeamListResModel.TeamLeadId = teamlists[0].TeamLeadId;
                    getTeamListResModel.TeamMemberId = teamlists[0].TeamMemberId;
                }

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
            var roleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("Team_Lead")).ToList();
            var roleIdList = roleList.Select(x => x.Id).ToList();
            var userList = _dbRepo.UserMstList().Where(x => x.RoleId != null);
            addEditTeamReqViewModel.TeamLeadList = userList.Where(x => roleIdList.Contains(x.RoleId.Value)).ToList();
            addEditTeamReqViewModel.TeamMemberList = userList.Where(x => !roleIdList.Contains(x.RoleId.Value)).ToList();

            try
            {
                if (Id > 0)
                {
                    var UserDetails = _dbRepo.AllUserMstList().FirstOrDefault(x => x.Id == Id);
                    if (UserDetails != null)
                    {
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
                                         TeamMemberId = string.Join(" , ", x.Select(x => x.TeamMemerName))
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
                        foreach (var teamMember in team.TeamMemberId)
                        {
                            teamDetails.TeamLeadId = team.TeamLeadId;
                            teamDetails.TeamMemberId = teamMember;
                            //teamDetails.TeamMemberId = team.TeamMemberId;
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
                        }
                    }
                    else
                    {
                        foreach (var teamMember in team.TeamMemberId)
                        {
                            teamMst.TeamLeadId = team.TeamLeadId;
                            teamMst.TeamMemberId = teamMember;
                            //teamMst.TeamMemberId = team.TeamMemberId;
                            teamMst.IsActive = true;
                            teamMst.IsDelete = false;
                            teamMst.CreatedDate = DateTime.Now;
                            teamMst.UpdatedDate = DateTime.Now;
                            teamMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                            teamMst.UpdatedBy = _commonHelper.GetLoggedInUserId();

                            _dbContext.TeamMsts.Add(teamMst);
                            _dbContext.SaveChanges();
                        }


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

        public async Task<IActionResult> DeleteTeam(int id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                if (id > 0)
                {
                    var teamList = await _dbRepo.TeamList().Where(x => x.TeamLeadId == id).ToListAsync();
                    if (teamList != null)
                    {
                        _dbContext.Entry(teamList).State = EntityState.Deleted;
                        _dbContext.SaveChanges();

                        commonResponse.Status = true;
                        commonResponse.StatusCode = HttpStatusCode.OK;
                        commonResponse.Message = "Team Deleted Successfully";

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
            //return Json(commonResponse);
        }
    }
}
