namespace ArcheOne.Models.Req
{
    public class AddEditHolidayReqModel
    {
        public int Id { get; set; }

        public string HolidayName { get; set; } = null!;

        public DateTime HolidayDate { get; set; }
    }
}
