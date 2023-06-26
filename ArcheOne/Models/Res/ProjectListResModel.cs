namespace ArcheOne.Models.Res
{
    public class ProjectListResModel
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Resources { get; set; }
        public string ResourcesNames { get; set; }
        public bool IsEditable { get; set; }
        public bool IsDeletable { get; set; }
    }
}
