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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ArcheOneDbContext _dbContext;
        public TeamController(DbRepo dbRepo, CommonHelper commonHelper, IWebHostEnvironment webHostEnvironment, ArcheOneDbContext dbContext)
        {
            _dbRepo = dbRepo;
            _commonHelper = commonHelper;
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
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
            }

            return View(teamlists);

        }

        public IActionResult AddEditTeam(int Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            AddEditTeamReqViewModel addEditTeamReqViewModel = new AddEditTeamReqViewModel();
            TeamLeadDetails teamLeadDetails = new TeamLeadDetails();
            List<TeamMemberDetails> teamMemberDetails = new List<TeamMemberDetails>();
            addEditTeamReqViewModel.TeamLeadDetails = new TeamLeadDetails();

            var roleList = _dbRepo.RoleMstList().Where(x => x.RoleCode.Contains("Team_Lead")).ToList();
            var roleIdList = roleList.Select(x => x.Id).ToList();
            var userList = _dbRepo.UserMstList().Where(x => x.RoleId != null);

            var teamLeadList = userList.Where(x => roleIdList.Contains(x.RoleId)).ToList();
            addEditTeamReqViewModel.TeamLeadList = teamLeadList;
            addEditTeamReqViewModel.TeamMemberList = userList.Where(x => !roleIdList.Contains(x.RoleId)).ToList();

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

                        addEditTeamReqViewModel.TeamMemberIdList = _dbRepo.TeamList().Select(x => new TeamMemberIdList
                        {
                            TeamMemberId = x.TeamMemberId
                        }).ToList();

                    }
                }
                else
                {
                    teamMemberDetails = (from z in _dbRepo.TeamList()
                                         join t in _dbRepo.AllUserMstList() on z.TeamMemberId equals t.Id
                                         select new { z, t }).Select(x => new TeamMemberDetails
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
            }
            return View(commonResponse);


        }

        [HttpPost]
        public async Task<CommonResponse> SaveUpdateTeam([FromBody] SaveUpdateTeamReqModel saveUpdateTeamReqModel)
        {
            CommonResponse commonResponse = new CommonResponse();
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


                    //    commonResponse.Status = true;
                    //    commonResponse.StatusCode = HttpStatusCode.OK;
                    //    commonResponse.Message = "Team Edited Successfully";
                    //}
                    //else
                    //{
                    //    //Add Mode
                    //    foreach (var teamMember in saveUpdateTeamReqModel.TeamMemberId)
                    //    {
                    //        var teamList = _dbRepo.TeamList().Where(x => x.TeamMemberId == teamMember).ToList();

                    //        if (teamList.Count > 0)
                    //        {
                    //            commonResponse.Status = false;
                    //            //commonResponse.StatusCode = HttpStatusCode.NotFound;
                    //            commonResponse.Message = "TeamMember Is Already Exist";
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

                    //    commonResponse.Status = true;
                    //    commonResponse.StatusCode = HttpStatusCode.OK;
                    //    commonResponse.Message = "Team Added Successfully";
                    //}


                    #endregion

                    #region Preyansi
                    //Edit Mode
                    var isExistteamLead = _dbRepo.TeamList().Where(x => x.TeamLeadId == saveUpdateTeamReqModel.TeamId).ToList();
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
                                    _dbContext.SaveChangesAsync();
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
                                _dbContext.SaveChangesAsync();
                                transactionScope.Complete();

                                commonResponse.Status = true;
                                commonResponse.StatusCode = HttpStatusCode.OK;
                                commonResponse.Message = "Team Edited Successfully";
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
                                        _dbContext.SaveChanges();

                                    }

                                }
                                transactionScope.Complete();
                                commonResponse.Status = true;
                                commonResponse.StatusCode = HttpStatusCode.OK;
                                commonResponse.Message = "Team Edited Successfully";
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
                                    _dbContext.SaveChanges();
                                }

                            }
                            transactionScope.Complete();
                            commonResponse.Status = true;
                            commonResponse.StatusCode = HttpStatusCode.OK;
                            commonResponse.Message = "Team Edited Successfully";
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
                            _dbContext.SaveChangesAsync();

                            transactionScope.Complete();
                            commonResponse.Status = true;
                            commonResponse.StatusCode = HttpStatusCode.OK;
                            commonResponse.Message = "Team Edited Successfully";
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

                            transactionScope.Complete();
                            commonResponse.Status = true;
                            commonResponse.StatusCode = HttpStatusCode.OK;
                            commonResponse.Message = "Team Added Successfully";
                        }
                    }


                    #endregion

                    commonResponse.Data = addTeamReqModelList;

                }
                catch (Exception ex)
                {
                    commonResponse.Message = ex.Message;
                }
            }
            return commonResponse;

        }

        public async Task<IActionResult> DeleteTeam(int Id)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                if (Id > 0)
                {
                    var teamList = await _dbRepo.TeamList().Where(x => x.TeamLeadId == Id).ToListAsync();
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
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return Json(commonResponse);
        }
    }
}
