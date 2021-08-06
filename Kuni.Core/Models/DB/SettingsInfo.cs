using System;
using SQLite;

namespace Kuni.Core.Models.DB
{
	public class SettingsInfo : DBModel
	{
		[PrimaryKey]
		public int UserId { get; set; }

		public bool? LocationEnabled { get; set; }

		public bool? PushNotificationEnabled{ get; set; }

		public bool? PinIsSet { get; set; }

		public string Pin { get; set; }
	}
}

