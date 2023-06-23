using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
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
                commonResponse.Status = true;
                commonResponse.Message = "GetAll TeamList Successfully";
                commonResponse.Data = teamlists;
                commonResponse.StatusCode = HttpStatusCode.OK;

            }
            catch (Exception ex)
            { 
                commonResponse.Message = ex.Message;
                commonResponse.Data = ex;
            }

            return View(teamlists);

        }

        public IActionResult AddEditTeam(int Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            AddEditTeamReqViewModel addEditTeamReqViewModel = new AddEditTeamReqViewModel();
            List<TeamLeadDetails> teamLeadLists = new List<TeamLeadDetails>();
            TeamLeadDetails teamLeadDetails = new TeamLeadDetails();
            List<TeamMemberDetails> teamMemberDetails = new List<TeamMemberDetails>();
            addEditTeamReqViewModel.TeamLeadDetails = new TeamLeadDetails();

            var roleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("Team_Lead")).ToList();
            var roleIdList = roleList.Select(x => x.Id).ToList();
            var userList = _dbRepo.UserMstList().Where(x => x.RoleId != null);
            
            var teamList = _dbRepo.TeamList().ToList();
            var teamLeadList = userList.Where(x => roleIdList.Contains(x.RoleId.Value)).ToList();
            addEditTeamReqViewModel.TeamLeadList = teamLeadList;
            addEditTeamReqViewModel.TeamMemberList = userList.Where(x => !roleIdList.Contains(x.RoleId.Value)).ToList();

            addEditTeamReqViewModel.TeamLeadDetails.TeamMemberDetails = null;
            addEditTeamReqViewModel.TeamLeadDetails.TeamLeadId = 0;


            try
            {
                if (Id > 0)
                {
                    var UserDetails = _dbRepo.AllUserMstList().FirstOrDefault(x => x.Id == Id);
                    if (UserDetails != null)
                    {
                        teamLeadDetails.TeamLeadId = UserDetails.Id;
                        teamLeadDetails.TeamLeadName = UserDetails.FirstName + " " + UserDetails.LastName;

                        teamMemberDetails = (from z in _dbRepo.TeamList()
                                             join f in _dbRepo.AllUserMstList().Where(x => x.Id == Id) on z.TeamLeadId equals f.Id
                                             join t in _dbRepo.AllUserMstList() on z.TeamMemberId equals t.Id
                                             select new { z, f, t }).ToList().Select(x => new TeamMemberDetails
                                             {
                                                 TeamMemberId = x.t.Id,
                                                 TeamMemberName = x.t.FirstName + " " + x.t.LastName,
                                             }).ToList();


                        addEditTeamReqViewModel.TeamLeadDetails = teamLeadDetails;
                        addEditTeamReqViewModel.TeamLeadDetails.TeamMemberDetails = teamMemberDetails;
                    }
                }
                else
                {
                    teamMemberDetails = (from z in _dbRepo.TeamList()
                                         join f in _dbRepo.AllUserMstList() on z.TeamLeadId equals f.Id
                                         join t in _dbRepo.AllUserMstList() on z.TeamMemberId equals t.Id
                                         select new { z, f, t }).ToList().Select(x => new TeamMemberDetails
                                         {
                                             TeamMemberId = x.t.Id,
                                             TeamMemberName = x.t.FirstName + " " + x.t.LastName,
                                         }).ToList();
                    addEditTeamReqViewModel.TeamLeadDetails.TeamMemberDetails = teamMemberDetails;
                }
                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Data = addEditTeamReqViewModel;
                commonResponse.Message = "Success";

            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Data = ex.StackTrace;
            }
            return View(commonResponse);


        }

        [HttpPost]
        public async Task<CommonResponse> SaveUpdateTeam([FromBody] SaveUpdateTeamReqModel saveUpdateTeamReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
            List<TeamMst> addTeamReqModelList = new List<TeamMst>();
            AddTeamReqModel addTeamReqModel = new AddTeamReqModel();
            try
            {
                //Edit Mode
                var teamDetails = await _dbRepo.TeamList().FirstOrDefaultAsync(x => x.Id == saveUpdateTeamReqModel.TeamId);
                if (teamDetails != null)
                {
                    foreach (var teamMember in saveUpdateTeamReqModel.TeamMemberId)
                    {
                        teamDetails.TeamLeadId = saveUpdateTeamReqModel.TeamLeadId;
                        teamDetails.TeamMemberId = teamMember;
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
                        commonResponse.Message = "Team Edited Succesfully";
                    }
                }
                else
                {
                    //Add Mode
                    foreach (var teamMember in saveUpdateTeamReqModel.TeamMemberId)
                    {
                        var teamList = _dbRepo.TeamList().Where(x => x.TeamMemberId == teamMember).ToList();

                        if (teamList.Count > 0)
                        {
                            commonResponse.Status = false;
                            commonResponse.StatusCode = HttpStatusCode.NotFound;
                            commonResponse.Message = "TeamMember Is Already Exist";
                        }
                        else
                        {
                            TeamMst teamMst = new TeamMst();
                            teamMst.TeamLeadId = saveUpdateTeamReqModel.TeamLeadId;
                            teamMst.TeamMemberId = teamMember;
                            teamMst.IsActive = true;
                            teamMst.IsDelete = false;
                            teamMst.CreatedDate = DateTime.Now;
                            teamMst.UpdatedDate = DateTime.Now;
                            teamMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                            teamMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                            addTeamReqModelList.Add(teamMst);
                        }
                    }

                    await _dbContext.TeamMsts.AddRangeAsync(addTeamReqModelList);
                    _dbContext.SaveChangesAsync();

                    commonResponse.Status = true;
                    commonResponse.StatusCode = HttpStatusCode.OK;
                    commonResponse.Message = "Team Added Successfully";
                }
                commonResponse.Data = addTeamReqModelList;

            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                commonResponse.Data = ex.StackTrace;
            }
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
                    if (teamList.Count > 0)
                    {
                        _dbContext.RemoveRange(teamList);
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
