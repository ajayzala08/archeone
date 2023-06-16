namespace ArcheOne.Models.Req
{
    public class SaveUpdateRequirementReqModel
    {
        public int RequirementId { get; set; }
        public int RequirementForId { get; set; }
        public int ClientId { get; set; }
        public string JobCode { get; set; } 
        public string? MainSkill { get; set; }
        public int NoOfPosition { get; set; }
        public string Location { get; set; }
        public string EndClient { get; set; }
        public decimal TotalMinExperience { get; set; }
        public decimal TotalMaxExperience { get; set; }
        public decimal RelevantMinExperience { get; set; }
        public decimal RelevantMaxExperience { get; set; }
        public decimal ClientBillRate { get; set; }
        public decimal CandidatePayRate { get; set; }
        public int PositionTypeId { get; set; }
        public int RequirementTypeId { get; set; }
        public int EmploymentTypeId { get; set; }
        public string Pocname { get; set; }
        public string MandatorySkills { get; set; } 
        public string? JobDescription { get; set; }
        public List<string> AssignedUserIds { get; set; }
        public int RequirementStatusId { get; set; }
        public bool IsActive { get; set; }
       
    }
}
