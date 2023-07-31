namespace ArcheOne.Models.Res
{
    public class UserDocumentResModel
    {
        public int Id { get; set; }

        public string EmployeeCode { get; set; }

        public string EmployeeName { get; set; }

        public string DocumentTypeId { get; set; }

        public string Document { get; set; } = null!;
        public bool IsDownload { get; set; }

        //public bool IsActive { get; set; }

        //public bool IsDelete { get; set; }

        //public int CreatedBy { get; set; }

        //public int UpdatedBy { get; set; }

        //public DateTime CreatedDate { get; set; }

        //public DateTime UpdatedDate { get; set; }
    }
}
