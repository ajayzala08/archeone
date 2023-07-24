namespace ArcheOne.Models.Res
{
    public class GetBirthdayWorkAnniversaryHolidayResModel
    {
        public List<WorkAnniversary> WorkAnniversaries { get; set; }
        public List<Birthday> Birthdays { get; set; }
        public List<Holiday> Holidays { get; set; }
    }
    public class WorkAnniversary
    {
        public string EmployeeImagePath { get; set; }
        public string EmployeeName { get; set; }
        public string JoinDate { get; set; }
    }
    public class Birthday
    {
        public string EmployeeImagePath { get; set; }
        public string EmployeeName { get; set; }
        public string Birthdate { get; set; }
    }
    public class Holiday
    {
        public string HolidayName { get; set; }
        public string HolidayDate { get; set; }
    }
}
