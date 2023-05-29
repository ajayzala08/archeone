﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ArcheOne.Models.Req
{
	public class UserModel
	{
		[Required(ErrorMessage = "Please select company")]
		public int CompanyId { get; set; }
		[Required(ErrorMessage = "Please enter FirstName")]
		[StringLength(100)]
		public string FirstName { get; set; } = null!;
		[Required]
		public string MiddleName { get; set; } = null!;
		[Required]
		public string LastName { get; set; } = null!;
		[Required]
		public string UserName { get; set; } = null!;
		[Required]
		public string Password { get; set; } = null!;
		[Required]
		public string Address { get; set; } = null!;
		[Required]
		public string Pincode { get; set; } = null!;
		[Required]
		public string Mobile1 { get; set; } = null!;
		[Required]
		public string Mobile2 { get; set; } = null!;
		[Required]
		public string Email { get; set; } = null!;
		[Required]
		public IFormFile PhotoUrl { get; set; } = null!;
	}
}
