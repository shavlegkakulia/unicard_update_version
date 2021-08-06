using System;
using SQLite;

namespace Kuni.Core.Models.DB
{
	public class UserInfo : DBModel
	{
		[PrimaryKey]
		public string UserId { get; set; }

		public string Username { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Address { get; set; }

		public string FullAddress  { get; set; }

		public string Phone { get; set; }

		public string PersonalId { get; set; }

		public string VirtualCardNumber { get; set; }

		public decimal Balance_BlockedPoints { get; set; }

		public decimal Balance_AvailablePoints { get; set; }

		public decimal Balance_AccumulatedPoint { get; set; }

		public decimal Balance_SpentPoints { get; set; }

		public bool IsFacebookUser { get; set; }
		//		public bool IsAuthed  { get; set; }
	}
}

