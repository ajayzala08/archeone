namespace ArcheOne.Models.Res
{
    public class GetAllEventResModel
    {
        public string title { get; set; }
        public string description { get; set; }
        public DateTime? start { get; set; }
        public DateTime? end { get; set; }
        public string? color { get; set; }
        public bool allDay { get; set; }
    }
}
