namespace ArcheOne.Models.Res
{
    public class GetHolidayListResModel
    {
        public bool IsEditable { get; set; }
        public bool IsDeletable { get; set; }
        public List<HolidayDetail> HolidayDetails { get; set; }

        public class HolidayDetail
        {
            public int Id { get; set; }

            public string HolidayName { get; set; }

            public string Date { get; set; }

            public string Day { get; set; }
        }
    }
}
