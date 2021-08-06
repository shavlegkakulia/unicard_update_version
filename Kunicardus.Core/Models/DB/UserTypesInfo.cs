using System;
using SQLite;

namespace Kunicardus.Core.Models.DB
{
	public class UserTypesInfo : DBModel
	{
		[PrimaryKey, AutoIncrement]
		public int LocalId { get; set; }

		public int UserTypeId { get; set; }

		public string UserTypeName { get; set; }
	}
}

