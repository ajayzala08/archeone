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

        public IQueryable<UserMst> UserMstList(bool IsDeleted = false, bool IsActive = true)
        {
            return _db.UserMsts.Where(x => x.IsDelete == IsDeleted && x.IsActive == IsActive).AsQueryable();
        }
        public IQueryable<LinkMst> LinkMstList()
        {
            return _db.LinkMsts.AsQueryable();
        }
    }
}
