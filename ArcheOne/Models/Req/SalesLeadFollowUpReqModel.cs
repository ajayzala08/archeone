namespace ArcheOne.Models.Req
{
    public class SalesLeadFollowUpReqModel
    {
        public int SalesLeadId { get; set; }
        public int SalesContactPersonId { get; set; }
        public DateTime FollowUpDate { get; set; }
        public DateTime NextFollowUpDate { get; set; }
        public int SalesLeadStatusId { get; set; }
    }
}
