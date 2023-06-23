namespace ArcheOne.Models.Req
{
    public class AddUpdateProjectReqModel
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectStatus { get; set; }
        public string Resources { get; set; }
    }
}
