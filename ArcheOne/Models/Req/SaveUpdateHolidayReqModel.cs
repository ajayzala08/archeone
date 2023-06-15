namespace ArcheOne.Models.Req
{
    public class SaveUpdateHolidayReqModel
    {
        public int Id { get; set; }

        public string HolidayName { get; set; } = null!;

        public DateTime HolidayDate { get; set; }

    }
}
