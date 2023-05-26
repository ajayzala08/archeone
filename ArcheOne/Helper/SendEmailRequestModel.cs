namespace ArcheOne.Helper
{
	public class SendEmailRequestModel
	{
		public string ToEmail { get; set; }
		public string Body { get; set; }
		public string Subject { get; set; }
		public dynamic Attachment { get; set; } = null;
	}
}
