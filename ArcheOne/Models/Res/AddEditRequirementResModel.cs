using ArcheOne.Database.Entities;
using System.Reflection.Metadata.Ecma335;

namespace ArcheOne.Models.Res
{
    public class AddEditRequirementResModel
    {
        public RequirementMst RequirementDetail { get; set; }
        public List<KeyValueModel> RequirementForList { get; set; }
        public List<KeyValueModel> ClientList { get; set; }
        public List<KeyValueModel> PositionTypeList { get; set; }
        public List<KeyValueModel> RequirementTypeList { get; set; }
        public List<KeyValueModel> EmploymentTypeList { get; set; }
        public List<KeyValueModel> RequirementStatusList { get; set; }
    }
}
