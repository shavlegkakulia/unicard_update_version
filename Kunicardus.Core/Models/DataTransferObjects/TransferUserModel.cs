using System;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class TransferUserModel
	{
		public string FBId { get; set; }

		public string Email { get; set; }

		public string Password { get; set; }

		public string Name { get; set; }

		public string Surname { get; set; }

		public string PersonalId { get; set; }

		public DateTime? DateOfBirth { get; set; }

		public string PhoneNumber { get; set; }

		public string CardNumber { get; set; }

		public bool NewCardRegistration { get; set; }

		public string SMSCode { get; set; }
	}
}

