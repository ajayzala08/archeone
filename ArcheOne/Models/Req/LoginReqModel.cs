using System.ComponentModel.DataAnnotations;

namespace ArcheOne.Models.Req
{
    public class LoginReqModel
    {
        [Required(ErrorMessage = "Please select UserName")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please select Password")]
        public string Password { get; set; }
    }
}
