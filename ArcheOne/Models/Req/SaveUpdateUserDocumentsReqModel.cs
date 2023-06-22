namespace ArcheOne.Models.Req
{
    public class SaveUpdateUserDocumentsReqModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int DocumentTypeId { get; set; }

        public IFormFile Document { get; set; } = null!;
    }
}
