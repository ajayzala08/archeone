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

        public enum PermissionMst
        {
            Dashboard_View = 1,
            Users_View,
            User_Detail_View,
            User_Edit,
            User_Delete,
            Teams_View,
            Holidays_View,
            Default_Permissions_View,
            Default_Permissions_Edit,
            User_Permissions_View,
            User_Permissions_Edit,
            Requirements_View,
            Requirements_Add,
            Requirements_Edit,
            Requirements_Delete,
            Policy_View,
            Leaves_View,
            Appraisal_View,
            Leads_View,
            Daily_Tasks_View,
            Projects_View,
            Task_Report_View,
            Event_View,
            Salary_View
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

        public enum ProjectStatus
        {
            ToDo = 1,
            InProgress,
            OnHold,
            Completed
        }
    }
}
