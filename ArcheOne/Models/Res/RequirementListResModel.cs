using System.Reflection.Metadata.Ecma335;

namespace ArcheOne.Models.Res
{
    public class RequirementListResModel
    {
        public List<RequirementListModel> RequirementList { get; set; }
    }

    public class RequirementListModel
    {
        public int Id { get; set; }
        public string JobCode { get; set; }
        public int RequirementForId { get; set; }
        public string RequirementForName { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int PositionTypeId { get; set; }
        public string PositionTypeName { get; set; }
        public int RequirementTypeId { get; set; }
        public string RequirementTypeName { get; set; }
        public int EmploymentTypeId { get; set; }
        public string EmploymentTypeName { get; set; }
        public int RequirementStatusId { get; set; }
        public string RequirementStatusName { get; set; }
        public string MainSkill { get; set; }
        public string MandatorySkills { get; set; }
        public decimal TotalMinExperience { get; set; }
        public decimal TotalMaxExperience { get; set; }
        public decimal RelevantMinExperience { get; set; }
        public decimal RelevantMaxExperience { get; set; }
        public int NoOfPosition { get; set; }
        public string Location { get; set; }
        public string JobDescription { get; set; }
        public string EndClient { get; set; }
        public string Pocname { get; set; }
        public decimal ClientBillRate { get; set; }
        public decimal CandidatePayRate { get; set; }
        public bool IsActive { get; set; }
        public string AssignedUserIds { get; set; }
        public string AssignedUserNames { get; set; }
    }
}
