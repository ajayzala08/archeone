namespace ArcheOne.Helper.CommonHelpers
{
    public class CommonConstant
    {
        #region LogType
        public const string ActivityLog = "ActivityLog";
        public const string ExceptionLog = "ExceptionLog";
        #endregion

        #region File Extensions
        public const string json = "json";
        public const string jpeg = "jpeg";
        public const string jpg = "jpg";
        public const string xlsx = ".xlsx";
        public const string png = ".png";
        #endregion

        #region Roles
        public const int Super_Admin = 1;
        public const int Admin = 2;
        public const int Project_Manager = 3;
        public const int Project_Team_Lead = 4;
        public const int Developer = 5;
        public const int Sales_Team_Lead = 6;

        #endregion

        #region Permissions
        public const int Dashboard_View = 1;

        public const int Default_Permission_View = 2;
        public const int Default_Permission_Update = 3;

        public const int User_Permission_View = 4;
        public const int User_Permission_Update = 5;

        public const int User_View = 6;
        public const int User_Add = 7;
        public const int User_Update = 8;
        public const int User_Delete = 9;
        #endregion
    }
}
