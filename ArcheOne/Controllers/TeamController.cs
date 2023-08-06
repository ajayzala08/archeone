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
    public class TeamController : Controller
    {
        private readonly DbRepo _dbRepo;
        private readonly CommonHelper _commonHelper;
        private readonly ArcheOneDbContext _dbContext;
        public TeamController(DbRepo dbRepo, CommonHelper commonHelper, ArcheOneDbContext dbContext)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Team()
        {
            int userId = _commonHelper.GetLoggedInUserId();
            bool showAddTeamButton = false;

            CommonResponse departmentDetailsResponse = await new CommonController(_dbRepo, _dbContext, _commonHelper).GetDepartmentByUserId(userId);

            if (departmentDetailsResponse.Status)
            {
                showAddTeamButton = departmentDetailsResponse.Data.DepartmentCode == CommonEnums.DepartmentMst.Human_Resource.ToString();
            }
            showAddTeamButton = !showAddTeamButton ? _commonHelper.CheckHasPermission(CommonEnums.PermissionMst.Teams_Add_View) : showAddTeamButton;

            return View(showAddTeamButton);
        }

        public async Task<IActionResult> TeamList()
        {
            CommonResponse response = new CommonResponse();
            GetTeamListResModel getTeamListResModel = new GetTeamListResModel();
            getTeamListResModel.TeamDetails = new List<GetTeamListResModel.TeamDetail>();
            try
            {
                int userId = _commonHelper.GetLoggedInUserId();
                bool isUserHRORSuperAdmin = false;

                CommonResponse departmentDetailsResponse = await new CommonController(_dbRepo, _dbContext, _commonHelper).GetDepartmentByUserId(userId);

                if (departmentDetailsResponse.Status)
                {
                    isUserHRORSuperAdmin = departmentDetailsResponse.Data.DepartmentCode == CommonEnums.DepartmentMst.Human_Resource.ToString();
                }

                if (!isUserHRORSuperAdmin)
                {
                    CommonResponse roleDetailsResponse = await new RoleController(_dbRepo).GetRoleByUserId(userId);
                    if (roleDetailsResponse.Status)
                    {
                        isUserHRORSuperAdmin = roleDetailsResponse.Data.RoleCode == CommonEnums.RoleMst.Super_Admin.ToString();
                    }
                }

                getTeamListResModel.IsEditable = !isUserHRORSuperAdmin ? _commonHelper.CheckHasPermission(CommonEnums.PermissionMst.Teams_Edit_View) : isUserHRORSuperAdmin;
                getTeamListResModel.IsDeletable = !isUserHRORSuperAdmin ? _commonHelper.CheckHasPermission(CommonEnums.PermissionMst.Teams_Delete_View) : isUserHRORSuperAdmin;

                var teamList = await _dbRepo.TeamList().Select(x => new { x.Id, x.TeamLeadId, x.TeamMemberId }).ToListAsync();

                if (teamList.Count > 0)
                {
                    // Get All TeamMemberIds by seperating after commas.
                    var teamMemberList = teamList.Select(x => new { Members = x.TeamMemberId.Split(',').Select(id => id.Trim()), TeamId = x.Id }).ToList();

                    var teamMember = teamMemberList.Where(x => x.Members.Any(y => y == userId.ToString())).Select(x => x.TeamId);

                    teamList = teamList.Where(x => (!isUserHRORSuperAdmin ? x.TeamLeadId == userId || teamMember.Any(y => y == x.Id) : true)).ToList();
                }

                getTeamListResModel.TeamDetails = (from team in teamList
                                                   join userMstLead in _dbRepo.UserMstList() on team.TeamLeadId equals userMstLead.Id into userMstLeadGroup
                                                   from userMstLeadItem in userMstLeadGroup.DefaultIfEmpty()
                                                   select new GetTeamListResModel.TeamDetail
                                                   {
                                                       Id = team.Id,
                                                       TeamLeadId = team.TeamLeadId,
                                                       TeamMemeberIds = team.TeamMemberId,
                                                       TeamLeadName = $"{userMstLeadItem.FirstName} {userMstLeadItem.LastName}",
                                                       TeamMemebersNames = string.Empty
                                                   }).ToList();

                foreach (var item in getTeamListResModel.TeamDetails)
                {

                    string[] teamMemberIds = item.TeamMemeberIds.Split(',');

                    var teamMemberNames = (from p in teamMemberIds
                                           join u in _dbRepo.UserMstList() on p.Trim() equals u.Id.ToString()
                                           select new
                                           {
                                               TeamMemberName = $"{u.FirstName} {u.LastName}"
                                           }).ToList();

                    item.TeamMemebersNames = string.Join(", ", teamMemberNames.Select(r => r.TeamMemberName).ToList());

                }
                response.Data = getTeamListResModel;

                if (getTeamListResModel.TeamDetails.Count > 0)
                {
                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Data found successfully!";
                }
                else
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "Data not found!";
                }

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return View(getTeamListResModel);

        }

        public async Task<IActionResult> AddEditTeam(int TeamId)
        {
            CommonResponse response = new CommonResponse();
            AddEditTeamReqViewModel addEditTeamReqViewModel = new AddEditTeamReqViewModel();

            try
            {
                // Get TeamLead Role Id
                var teamLeadRoleId = await _dbRepo.RoleMstList().Where(x => x.RoleCode == CommonEnums.RoleMst.Team_Lead.ToString()).Select(x => x.Id).FirstOrDefaultAsync();

                // Get All User's (Id, FirstName + LastName, RoleId).
                var userList = await _dbRepo.UserMstList().Select(x => new AddEditTeamReqViewModel.UserDetail { Id = x.Id, FullName = $"{x.FirstName} {x.LastName}", RoleId = x.RoleId }).ToListAsync();

                // Get All Team's (Id, TeamLeadId, TeamMemberId).
                var teamList = await _dbRepo.TeamList().Select(x => new { x.Id, x.TeamLeadId, x.TeamMemberId }).ToListAsync();

                // Get All User's who is TeamLead AND not assigned in any team as TeamLead.
                addEditTeamReqViewModel.TeamLeadList = userList.Where(x => x.RoleId == teamLeadRoleId && !teamList.Any(y => y.TeamLeadId == x.Id)).ToList();

                // Get All TeamMemberIds by seperating after commas.
                var teamMemberList = teamList.SelectMany(s => s.TeamMemberId.Split(',').Select(id => id.Trim())).ToList();

                // Get All User's who is not TeamLead AND not assigned in any Team.
                addEditTeamReqViewModel.TeamMemberList = userList.Where(x => x.RoleId != teamLeadRoleId && !teamMemberList.Any(y => y == x.Id.ToString())).ToList();

                addEditTeamReqViewModel.TeamId = TeamId;
                addEditTeamReqViewModel.TeamLeadId = 0;

                if (TeamId > 0) // Edit Team
                {
                    // Get Team Details by Id.
                    var teamDetails = teamList.Where(x => x.Id == TeamId).Select(x => new { x.TeamLeadId, x.TeamMemberId }).FirstOrDefault();
                    if (teamDetails != null)
                    {
                        // Append current TeamLead in TeamLead List.
                        addEditTeamReqViewModel.TeamLeadId = teamDetails.TeamLeadId;
                        var teamLeadName = userList.FirstOrDefault(x => x.Id == teamDetails.TeamLeadId) ?? new AddEditTeamReqViewModel.UserDetail();

                        addEditTeamReqViewModel.TeamLeadList.Add(new AddEditTeamReqViewModel.UserDetail { Id = teamDetails.TeamLeadId, FullName = teamLeadName.FullName });

                        List<string> teamMemberIds = teamDetails.TeamMemberId.Split(",").ToList();

                        // Append current TeamMember in TeamMember List.
                        AddEditTeamReqViewModel.UserDetail teamMemberName;
                        foreach (var item in teamMemberIds)
                        {
                            teamMemberName = userList.FirstOrDefault(x => x.Id == Convert.ToInt32(item)) ?? new AddEditTeamReqViewModel.UserDetail();
                            addEditTeamReqViewModel.TeamMemberList.Add(new AddEditTeamReqViewModel.UserDetail { Id = Convert.ToInt32(item), FullName = teamMemberName.FullName });
                        }

                        // Get current TeamMemberIds.
                        addEditTeamReqViewModel.TeamMemberIds = teamMemberIds;
                    }
                }

                response.Data = addEditTeamReqViewModel;
                if (addEditTeamReqViewModel.TeamLeadList != null && addEditTeamReqViewModel.TeamMemberList != null)
                {
                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Data found successfully!";
                }
                else
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "Data not found!";
                }

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return View(response);
        }

        [HttpPost]
        public async Task<CommonResponse> SaveUpdateTeam([FromBody] SaveUpdateTeamReqModel request)
        {
            CommonResponse response = new CommonResponse();
            List<TeamMst> addTeamReqModelList = new List<TeamMst>();
            try
            {
                int userId = _commonHelper.GetLoggedInUserId();
                if (request.TeamId == 0)
                {
                    if (await _dbRepo.TeamList().AnyAsync(x => x.TeamLeadId == request.TeamLeadId))
                    {
                        response.Message = "This Team Lead is already having another team!";
                    }

                    // This code is simplified version of else if condition.

                    /*List<string> userIdStrings = await _dbRepo.TeamList()..Select(u => u.TeamMemberId).ToListAsync();
                    IEnumerable<string> userIdArray = userIdStrings.SelectMany(s => s.Split(',').Select(id => id.Trim()));
                    IEnumerable<int> matchingUserIds = request.TeamMemberId.Intersect(userIdArray.Select(int.Parse));*/

                    else if (request.TeamMemberId.Intersect((await _dbRepo.TeamList().Select(u => u.TeamMemberId).ToListAsync()).SelectMany(s => s.Split(',').Select(id => id.Trim())).Select(int.Parse)).ToArray().Length > 0)
                    {
                        response.Message = "A Team Member(s) is already assingned in another team(s)!";
                    }
                    else
                    {
                        TeamMst teamMst = new TeamMst()
                        {
                            TeamLeadId = request.TeamLeadId,
                            TeamMemberId = string.Join(",", request.TeamMemberId),
                            IsActive = true,
                            IsDelete = false,
                            CreatedBy = userId,
                            UpdatedBy = userId,
                            CreatedDate = _commonHelper.GetCurrentDateTime(),
                            UpdatedDate = _commonHelper.GetCurrentDateTime(),
                        };

                        await _dbContext.TeamMsts.AddAsync(teamMst);
                        await _dbContext.SaveChangesAsync();

                        response.Status = true;
                        response.StatusCode = HttpStatusCode.OK;
                        response.Message = "Team added successfully!";
                    }
                }
                else
                {
                    var teamData = await _dbRepo.TeamList().FirstOrDefaultAsync(x => x.Id == request.TeamId);
                    if (teamData != null)
                    {

                        if (await _dbRepo.TeamList().AnyAsync(x => x.TeamLeadId == request.TeamLeadId && x.Id != request.TeamId))
                        {
                            response.Message = "This Team Lead is already having another team!";
                        }

                        // This code is simplified version of else if condition.
                        /*List<string> userIdStrings = await _dbRepo.TeamList().Where(x => x.Id != request.TeamId).Select(u => u.TeamMemberId).ToListAsync();
                        IEnumerable<string> userIdArray = userIdStrings.SelectMany(s => s.Split(',').Select(id => id.Trim()));
                        IEnumerable<int> matchingUserIds = request.TeamMemberId.Intersect(userIdArray.Select(int.Parse));*/

                        else if (request.TeamMemberId.Intersect((await _dbRepo.TeamList().Where(x => x.Id != request.TeamId).Select(u => u.TeamMemberId).ToListAsync()).SelectMany(s => s.Split(',').Select(id => id.Trim())).Select(int.Parse)).ToArray().Length > 0)
                        {
                            response.Message = "A Team Member(s) is already assingned in another team(s)!";
                        }
                        else
                        {
                            teamData.TeamLeadId = request.TeamLeadId;
                            teamData.TeamMemberId = string.Join(",", request.TeamMemberId);
                            teamData.UpdatedBy = userId;
                            teamData.UpdatedDate = _commonHelper.GetCurrentDateTime();

                            _dbContext.Entry(teamData).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();

                            response.Status = true;
                            response.StatusCode = HttpStatusCode.OK;
                            response.Message = "Team updated successfully!";
                        }

                    }
                    else
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = "Data not found!";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<IActionResult> DeleteTeam(int Id)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                var teamData = await _dbRepo.TeamList().FirstOrDefaultAsync(x => x.Id == Id);
                if (teamData != null)
                {
                    teamData.IsDelete = true;

                    _dbContext.Entry(teamData).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();

                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Team deleted successfully!";

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
