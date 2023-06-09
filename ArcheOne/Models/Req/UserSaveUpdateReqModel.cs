﻿namespace ArcheOne.Models.Req
{
	public class UserSaveUpdateReqModel
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; } 
		public string Password { get; set; } 
		public string Address { get; set; } 
		public string Pincode { get; set; }
		public string Mobile1 { get; set; }
		public string Mobile2 { get; set; } 
		public string Email { get; set; }
		public IFormFile PhotoUrl { get; set; }
		public bool IsActive { get; set; }
		public int RoleId { get; set; }
	}
}