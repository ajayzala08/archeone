using ArcheOne.Database.Entities;

namespace ArcheOne.Helper.CommonHelpers
{
    public class DbRepo
    {
        private readonly ArcheOneDbContext _db;
        public DbRepo(ArcheOneDbContext db)
        {
            _db = db;
        }

        public IQueryable<UserMst> AllUserMstList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.UserMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }

        public IQueryable<UserMst> UserMstList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.UserMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive && x.RoleId != CommonConstant.SuperAdmin).AsQueryable();
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
            return _db.RoleMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive && x.Id != CommonConstant.SuperAdmin).AsQueryable();
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

		public IQueryable<ClientMst> ClientList(bool IsDeleted = false, bool IsActive = true)
		{
			return _db.ClientMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();

		}
		public IQueryable<PositionTypeMst> positionTypeList(bool IsDeleted = false, bool IsActive = true)
		{
			return _db.PositionTypeMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();

		}
		public IQueryable<RequirementTypeMst> RequirementTypeList(bool IsDeleted = false, bool IsActive = true)
		{
			return _db.RequirementTypeMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();

		}
		public IQueryable<RequirementForMst> RequirementForList(bool IsDeleted = false, bool IsActive = true)
		{
			return _db.RequirementForMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();

		}
		public IQueryable<EmploymentTypeMst> EmploymentTypeList(bool IsDeleted = false, bool IsActive = true)
		{
			return _db.EmploymentTypeMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();

		}
        public IQueryable<TeamMst> TeamList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.TeamMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();

        }
    }
}
