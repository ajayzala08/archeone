namespace ArcheOne.Helper.CommonHelpers
{
    public class CommonEnums
    {
        public enum InterviewRoundStatusMst
        {
            Scheduled = 1,
            Cleared,
            Rejected,
            No_Show,
        }

        public enum OfferStatusMst
        {
            Offer = 1,
            Hire
        }

        public enum UploadedResumeTableFlowStatus
        {
            Interview_Info = 1,
            Offer,
            To_Be_Join,
            Join,
            No_Show
        }
    }
}
