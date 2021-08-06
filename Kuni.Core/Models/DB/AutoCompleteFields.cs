using System;
using SQLite;

namespace Kuni.Core.Models.DB
{
	public class AutoCompleteFields : DBModel
	{
		[PrimaryKey,AutoIncrement]
		public int Id { get; set; }

		public string UserEmail { get; set; }
	}
}

