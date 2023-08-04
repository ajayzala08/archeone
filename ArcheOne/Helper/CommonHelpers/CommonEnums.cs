namespace ArcheOne.Helper.CommonHelpers
{
    public class CommonEnums
    {
        public enum RoleMst
        {
            Super_Admin = 1,
            Admin,
            Manager,
            Team_Lead,
            Professional,
        }

        public enum DepartmentMst
        {
            System_Administration = 1,
            Software_Development,
            Quality_Analyst,
            Human_Resource,
            Marketing,
            Designer,
            Content,
            Finance,
            Sales,
            Recruitment
        }

        public enum PermissionMst
        {
            Dashboard_View = 1,
            Users_View,
            User_Document_View,
            User_Detail_View,
            User_Edit,
            User_Delete,
            Teams_View,
            Holidays_View,
            Holidays_Add_View,
            Holidays_Edit_View,
            Holidays_Delete_View,
            Default_Permissions_View,
            Default_Permissions_Edit,
            User_Permissions_View,
            User_Permissions_Edit,
            Requirements_View,
            Requirements_Add,
            Requirements_Edit,
            Requirements_Delete,
            Policy_View,
            Policy_Add_View,
            Policy_Edit_View,
            Policy_Delete_View,
            Leaves_View,
            Appraisal_View,
            Leads_View,
            Daily_Tasks_View,
            Projects_View,
            Task_Report_View,
            Event_View,
            Salary_View,
            Salary_Add_View,
            Salary_Delete_View,
            Uploaded_Resume_Status_Update
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

        public enum ResumeStatus
        {
            Pending = 1,
            Approved,
            Rejected
        }
    }
}
