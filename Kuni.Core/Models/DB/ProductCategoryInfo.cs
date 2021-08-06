using System;
using SQLite;

namespace Kuni.Core.Models.DB
{
	public class ProductCategoryInfo:DBModel
	{
		[PrimaryKey, AutoIncrement]
		public int LocalId { get; set; }

		public string CategoryID{ get; set; }

		public bool IsHidden { get; set; }

		public string CategoryName { get; set; }
	}
}

