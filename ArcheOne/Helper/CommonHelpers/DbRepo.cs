using ArcheOne.Database.Entities;

namespace ArcheOne.Helper.CommonHelpers
{
    public class DbRepo
    {
        private readonly ArcheOneDbContext _db;
        private readonly CommonHelper _commonHelper;
        public DbRepo(ArcheOneDbContext db, CommonHelper commonHelper)
        {
            _db = db;
            _commonHelper = commonHelper;
        }

        public IQueryable<UserMst> AllUserMstList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.UserMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<UserMst> UserMstList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.UserMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive && x.RoleId != CommonConstant.Super_Admin).AsQueryable();
        }

        public IQueryable<LinkMst> LinkMstList()
        {
            return _db.LinkMsts.AsQueryable();
        }

        public IQueryable<CompanyMst> CompanyMstList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.CompanyMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<RoleMst> RoleMstList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.RoleMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive && x.Id != CommonConstant.Super_Admin).AsQueryable();
        }

        public IQueryable<DefaultPermission> DefaultPermissionList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.DefaultPermissions.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<PermissionMst> PermissionList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.PermissionMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<UserPermission> UserPermissionList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.UserPermissions.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        //public IQueryable<ClientMst> ClientList(bool IsDeleted = false, bool IsActive = true)
        //{
        //    return _db.ClientMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();

        //}

        //public IQueryable<PositionTypeMst> positionTypeList(bool IsDeleted = false, bool IsActive = true)
        //{
        //    return _db.PositionTypeMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();

        //}

        //public IQueryable<RequirementTypeMst> RequirementTypeList(bool IsDeleted = false, bool IsActive = true)
        //{
        //    return _db.RequirementTypeMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();

        //}

        //public IQueryable<RequirementForMst> RequirementForList(bool IsDeleted = false, bool IsActive = true)
        //{
        //    return _db.RequirementForMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();

        //}

        //public IQueryable<EmploymentTypeMst> EmploymentTypeList(bool IsDeleted = false, bool IsActive = true)
        //{
        //    return _db.EmploymentTypeMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();

        //}

        public IQueryable<TeamMst> TeamList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.TeamMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();

        }

        public IQueryable<SalesLeadMst> SalesLeadList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.SalesLeadMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public UserMst? GetLoggedInUserDetails()
        {
            int UserId = _commonHelper.GetLoggedInUserId();
            var UserDetail = _db.UserMsts.FirstOrDefault(x => x.Id == UserId && x.IsDelete == false && x.IsActive == true);
            return UserDetail;
        }

        public bool HasPermission(int PermissionId)
        {
            bool hasPermission = false;
            var UserDetail = GetLoggedInUserDetails();
            if (UserDetail != null)
            {
                int RoleId = UserDetail.RoleId.Value;
                int UserId = UserDetail.Id;
                if (RoleId != CommonConstant.Super_Admin)
                {
                    var DefaultPermissionIdList = DefaultPermissionList().Where(x => x.RoleId == RoleId).Select(x => x.PermissionId).ToList();
                    var UserPermissionIdList = UserPermissionList().Where(x => x.UserId == UserId).Select(x => x.PermissionId).ToList();

                    hasPermission = DefaultPermissionIdList.Contains(PermissionId) ? true : false;
                    if (!hasPermission)
                        hasPermission = UserPermissionIdList.Contains(PermissionId) ? true : false;
                }
                else
                {
                    hasPermission = true;
                }
            }
            return hasPermission;
        }
        public IQueryable<SalesContactPersonMst> SalesContactPersonList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.SalesContactPersonMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<ResumeFileUploadDetailMst> ResumeFileUploadDetailList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.ResumeFileUploadDetailMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<RequirementMst> RequirementList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.RequirementMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<RequirementForMst> RequirementForList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.RequirementForMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<ClientMst> ClientList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.ClientMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<PositionTypeMst> PositionTypeList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.PositionTypeMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<RequirementTypeMst> RequirementTypeList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.RequirementTypeMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<EmploymentTypeMst> EmploymentTypeList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.EmploymentTypeMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<RequirementStatusMst> RequirementStatusList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.RequirementStatusMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<InterviewRoundTypeMst> InterviewRoundTypeList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.InterviewRoundTypeMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<InterviewMst> InterviewList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.InterviewMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<InterviewRoundMst> InterviewRoundList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.InterviewRoundMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<InterviewRoundStatusMst> InterviewRoundStatusList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.InterviewRoundStatusMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<HireStatusMst> HireStatusList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.HireStatusMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }
    }
}
