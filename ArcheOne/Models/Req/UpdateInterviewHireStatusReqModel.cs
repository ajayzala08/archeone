namespace ArcheOne.Models.Req
{
    public class UpdateInterviewHireStatusReqModel
    {
        public int HireStatusId { get; set; }
        public int OfferStatusId { get; set; }
        public int UploadedResumeId { get; set; }
        public DateTime? JoinInDate { get; set; } = null;
        public Decimal? OfferedPackage { get; set; } = null;
        public string? Note { get; set; } = null;
    }
}
