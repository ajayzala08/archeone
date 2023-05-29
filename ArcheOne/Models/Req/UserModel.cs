using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ArcheOne.Models.Req
{
	public class UserModel
	{
		[Required(ErrorMessage = "Please select CompanyId")]
		public int CompanyId { get; set; }
		[Required(ErrorMessage = "Please enter FirstName")]
		public string FirstName { get; set; } = null!;
		[Required(ErrorMessage = "Please enter MiddleName")]
		public string MiddleName { get; set; } = null!;
		[Required(ErrorMessage = "Please enter LastName")]
		public string LastName { get; set; } = null!;
		[Required(ErrorMessage = "Please enter UserName")]
		public string UserName { get; set; } = null!;
		[Required(ErrorMessage = "Please enter Password")]
		public string Password { get; set; } = null!;
		[Required(ErrorMessage = "Please enter Address")]
		public string Address { get; set; } = null!;
		[Required(ErrorMessage = "Please enter Pincode")]
		public string Pincode { get; set; } = null!;
		[Required(ErrorMessage = "Please enter Mobile1")]
		public string Mobile1 { get; set; } = null!;
		[Required(ErrorMessage = "Please enter Mobile2")]
		public string Mobile2 { get; set; } = null!;
		[Required(ErrorMessage = "Please enter Email")]
		public string Email { get; set; } = null!;
		[Required(ErrorMessage = "Please enter PhotoUrl")]
		public IFormFile PhotoUrl { get; set; } = null!;
	}
}
