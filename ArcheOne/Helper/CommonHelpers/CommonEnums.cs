namespace ArcheOne.Helper.CommonHelpers
{
    public class CommonEnums
    {
        public enum RoleMst
        {
            Super_Admin = 1,
            Admin,
            Project_Manager
        }
        public enum InterviewRoundStatusMst
        {
            Scheduled = 1,
            Cleared,
            Rejected,
            No_Show,
        }

        public enum OfferStatusMst
        {
            Cleared = 1,
            Offer,
            Hire
        }

        public enum HireStatusMst
        {
            To_Be_Join = 1,
            Join,
            No_Show,
            Bad_Delivery
        }

        public enum UploadedResumeTableFlowStatus
        {
            Interview_Info = 1,
            Cleared,
            Offer,
            To_Be_Join,
            Join,
            No_Show
        }

        public enum ValidationTypes
        {
            NotNullOrEmpty,
            OnlyDecimal,
            OnlyDateTime
        }
    }
}
