using ArcheOne.Database.Entities;

namespace ArcheOne.Models.Req
{
    public class AddEditUserDocumentReqModel
    {
        public List<UserMst> UserList { get; set; }
        public List<DocumentTypeMst> DocumentTypeList { get; set; }
        public AddEditUserDocument AddEditUserDocuments { get; set; }
    }

    public class AddEditUserDocument
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int DocumentTypeId { get; set; }

        public string Document { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
