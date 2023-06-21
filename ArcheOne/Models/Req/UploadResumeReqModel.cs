namespace ArcheOne.Models.Req
{
    public class UploadResumeReqModel
    {
        public int RequirementId { get; set; }
        public IFormFile ResumeFile { get; set; }
    }
}
