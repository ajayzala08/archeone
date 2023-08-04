using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Transactions;

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
                bool isUserHR = false;

                CommonResponse departmentDetailsResponse = await new CommonController(_dbRepo, _dbContext, _commonHelper).GetDepartmentByUserId(userId);

                if (departmentDetailsResponse.Status)
                {
                    isUserHR = departmentDetailsResponse.Data.DepartmentCode == CommonEnums.DepartmentMst.Human_Resource.ToString();
                }

                getTeamListResModel.IsEditable = !isUserHR ? _commonHelper.CheckHasPermission(CommonEnums.PermissionMst.Teams_Edit_View) : isUserHR;
                getTeamListResModel.IsDeletable = !isUserHR ? _commonHelper.CheckHasPermission(CommonEnums.PermissionMst.Teams_Delete_View) : isUserHR;

                var teamList = await _dbRepo.TeamList().ToListAsync();

                getTeamListResModel.TeamDetails = await (from team in _dbRepo.TeamList()
                                                         join userMstLead in _dbRepo.UserMstList() on team.TeamLeadId equals userMstLead.Id into userMstLeadGroup
                                                         from userMstLeadItem in userMstLeadGroup.DefaultIfEmpty()
                                                         select new GetTeamListResModel.TeamDetail
                                                         {
                                                             Id = team.Id,
                                                             TeamLeadId = team.TeamLeadId,
                                                             TeamMemeberIds = team.TeamMemberId,
                                                             TeamLeadName = $"{userMstLeadItem.FirstName} {userMstLeadItem.LastName}",
                                                             TeamMemebersNames = string.Empty
                                                         }).ToListAsync();

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

                var teamLeadRoleId = await _dbRepo.RoleMstList().Where(x => x.RoleCode == CommonEnums.RoleMst.Team_Lead.ToString()).Select(x => x.Id).FirstOrDefaultAsync();
                var userList = await _dbRepo.UserMstList().Select(x => new AddEditTeamReqViewModel.UserDetail { Id = x.Id, FullName = $"{x.FirstName} {x.LastName}", RoleId = x.RoleId }).ToListAsync();

                addEditTeamReqViewModel.TeamLeadList = userList.Where(x => x.RoleId == teamLeadRoleId).ToList();
                addEditTeamReqViewModel.TeamMemberList = userList.Where(x => x.RoleId != teamLeadRoleId).ToList();

                addEditTeamReqViewModel.TeamLeadId = 0;

                if (TeamId > 0)
                {
                    var teamDetails = await _dbRepo.TeamList().Where(x => x.Id == TeamId).Select(x => new { x.TeamLeadId, x.TeamMemberId }).FirstOrDefaultAsync();
                    if (teamDetails != null)
                    {
                        addEditTeamReqViewModel.TeamLeadId = teamDetails.TeamLeadId;

                        List<string> teamMemberIds = teamDetails.TeamMemberId.Split(",").ToList();

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
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    #region sonal

                    ////Edit Mode
                    //var teamDetails = await _dbRepo.TeamList().FirstOrDefaultAsync(x => x.TeamLeadId == saveUpdateTeamReqModel.TeamLeadId);
                    //if (teamDetails != null)
                    //{
                    //    TeamMst teamMst = new TeamMst();
                    //    foreach (var teamMember in saveUpdateTeamReqModel.TeamMemberId)
                    //    {
                    //        //TeamMst teamMst = new TeamMst();
                    //        teamMst.TeamLeadId = saveUpdateTeamReqModel.TeamLeadId;
                    //        teamMst.TeamMemberId = teamMember;
                    //        teamMst.IsActive = true;
                    //        teamMst.IsDelete = false;
                    //        teamMst.CreatedDate = DateTime.Now;
                    //        teamMst.UpdatedDate = DateTime.Now;
                    //        teamMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                    //        teamMst.UpdatedBy = _commonHelper.GetLoggedInUserId();


                    //        //_dbContext.Entry(teamDetails).State = EntityState.Added;
                    //        //_dbContext.SaveChanges();
                    //    }

                    //    await _dbContext.TeamMsts.AddRangeAsync(teamMst);
                    //    _dbContext.SaveChangesAsync();


                    //    response.Status = true;
                    //    response.StatusCode = HttpStatusCode.OK;
                    //    response.Message = "Team Edited Successfully";
                    //}
                    //else
                    //{
                    //    //Add Mode
                    //    foreach (var teamMember in saveUpdateTeamReqModel.TeamMemberId)
                    //    {
                    //        var teamList = _dbRepo.TeamList().Where(x => x.TeamMemberId == teamMember).ToList();

                    //        if (teamList.Count > 0)
                    //        {
                    //            response.Status = false;
                    //            //response.StatusCode = HttpStatusCode.NotFound;
                    //            response.Message = "TeamMember Is Already Exist";
                    //        }
                    //        else
                    //        {
                    //            TeamMst teamMst = new TeamMst();
                    //            teamMst.TeamLeadId = saveUpdateTeamReqModel.TeamLeadId;
                    //            teamMst.TeamMemberId = teamMember;
                    //            teamMst.IsActive = true;
                    //            teamMst.IsDelete = false;
                    //            teamMst.CreatedDate = DateTime.Now;
                    //            teamMst.UpdatedDate = DateTime.Now;
                    //            teamMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                    //            teamMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
                    //            addTeamReqModelList.Add(teamMst);
                    //        }
                    //    }

                    //    await _dbContext.TeamMsts.AddRangeAsync(addTeamReqModelList);
                    //    _dbContext.SaveChangesAsync();

                    //    response.Status = true;
                    //    response.StatusCode = HttpStatusCode.OK;
                    //    response.Message = "Team Added Successfully";
                    //}


                    #endregion

                    #region Preyansi
                    //Edit Mode
                    /*var isExistteamLead = _dbRepo.TeamList().Where(x => x.TeamLeadId == saveUpdateTeamReqModel.TeamId).ToList();
                    if (isExistteamLead.Count > 0)
                    {
                        if (saveUpdateTeamReqModel.TeamId == saveUpdateTeamReqModel.TeamLeadId)
                        {


                            var teamDetails = await _dbRepo.TeamList().FirstOrDefaultAsync(x => x.TeamLeadId == saveUpdateTeamReqModel.TeamLeadId);
                            if (teamDetails != null)
                            {
                                var TeamList = await _dbRepo.TeamList().Where(x => x.TeamLeadId == saveUpdateTeamReqModel.TeamLeadId).ToListAsync();
                                foreach (var item in TeamList)
                                {
                                    _dbContext.TeamMsts.RemoveRange(item);
                                    await _dbContext.SaveChangesAsync();
                                }


                                List<TeamMst> teamMstList = new List<TeamMst>();


                                foreach (var teamMember in saveUpdateTeamReqModel.TeamMemberId)
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
                                    teamMstList.Add(teamMst);
                                }
                                await _dbContext.TeamMsts.AddRangeAsync(teamMstList);
                                await _dbContext.SaveChangesAsync();
                                transactionScope.Complete();

                                response.Status = true;
                                response.StatusCode = HttpStatusCode.OK;
                                response.Message = "Team Edited Successfully";
                            }
                            else
                            {
                                foreach (var item in isExistteamLead)
                                {
                                    var teamDetails1 = _dbRepo.TeamList().FirstOrDefault(x => x.TeamLeadId == item.TeamLeadId);
                                    if (teamDetails1 != null)
                                    {
                                        teamDetails1.TeamLeadId = saveUpdateTeamReqModel.TeamLeadId;
                                        teamDetails1.UpdatedDate = DateTime.Now;
                                        teamDetails1.UpdatedBy = _commonHelper.GetLoggedInUserId();

                                        _dbContext.Entry(teamDetails1).State = EntityState.Modified;
                                        await _dbContext.SaveChangesAsync();


                                    }

                                }
                                transactionScope.Complete();
                                response.Status = true;
                                response.StatusCode = HttpStatusCode.OK;
                                response.Message = "Team Edited Successfully";
                            }
                        }
                        else
                        {
                            foreach (var item in isExistteamLead)
                            {
                                var teamDetails1 = _dbRepo.TeamList().FirstOrDefault(x => x.TeamLeadId == item.TeamLeadId);
                                if (teamDetails1 != null)
                                {
                                    teamDetails1.TeamLeadId = saveUpdateTeamReqModel.TeamLeadId;
                                    teamDetails1.UpdatedDate = DateTime.Now;
                                    teamDetails1.UpdatedBy = _commonHelper.GetLoggedInUserId();

                                    _dbContext.Entry(teamDetails1).State = EntityState.Modified;
                                    await _dbContext.SaveChangesAsync();
                                }

                            }
                            transactionScope.Complete();
                            response.Status = true;
                            response.StatusCode = HttpStatusCode.OK;
                            response.Message = "Team Edited Successfully";
                        }

                    }
                    else
                    {
                        var teamDetails = await _dbRepo.TeamList().FirstOrDefaultAsync(x => x.TeamLeadId == saveUpdateTeamReqModel.TeamLeadId);
                        if (teamDetails != null)
                        {
                            TeamMst teamMst = new TeamMst();
                            foreach (var teamMember in saveUpdateTeamReqModel.TeamMemberId)
                            {
                                //TeamMst teamMst = new TeamMst();
                                teamMst.TeamLeadId = saveUpdateTeamReqModel.TeamLeadId;
                                teamMst.TeamMemberId = teamMember;
                                teamMst.IsActive = true;
                                teamMst.IsDelete = false;
                                teamMst.CreatedDate = DateTime.Now;
                                teamMst.UpdatedDate = DateTime.Now;
                                teamMst.CreatedBy = _commonHelper.GetLoggedInUserId();
                                teamMst.UpdatedBy = _commonHelper.GetLoggedInUserId();


                                //_dbContext.Entry(teamDetails).State = EntityState.Added;
                                //_dbContext.SaveChanges();
                            }

                            await _dbContext.TeamMsts.AddRangeAsync(teamMst);
                            await _dbContext.SaveChangesAsync();

                            transactionScope.Complete();
                            response.Status = true;
                            response.StatusCode = HttpStatusCode.OK;
                            response.Message = "Team Edited Successfully";
                        }
                        else
                        {
                            //Add Mode
                            foreach (var teamMember in saveUpdateTeamReqModel.TeamMemberId)
                            {
                                var teamList = _dbRepo.TeamList().Where(x => x.TeamMemberId == teamMember).ToList();

                                if (teamList.Count > 0)
                                {
                                    response.Status = false;
                                    response.Message = "TeamMember Is Already Exist";
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
                            await _dbContext.SaveChangesAsync();

                            transactionScope.Complete();
                            response.Status = true;
                            response.StatusCode = HttpStatusCode.OK;
                            response.Message = "Team Added Successfully";
                        }
                    }*/

                    //response.Data = addTeamReqModelList;

                    #endregion

                    int userId = _commonHelper.GetLoggedInUserId();
                    if (request.TeamId == 0)
                    {
                        if (await _dbRepo.TeamList().AnyAsync(x => x.TeamLeadId == request.TeamLeadId))
                        {
                            response.Message = "This Team Lead is already having another team!";
                        }
                        else if (await _dbRepo.TeamList().AnyAsync(row => request.TeamMemberId.Any(id => row.TeamMemberId.Contains(id.ToString()))))
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
                            else if (await _dbRepo.TeamList().AnyAsync(x => request.TeamMemberId.Any(id => x.TeamMemberId.Contains(id.ToString())) && x.Id != request.TeamId))
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
