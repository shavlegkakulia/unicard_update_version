using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Core.Models.Helpers
{
	public class Transfer
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		public string  Name { get; set; }

		public string Address { get; set; }

		public decimal Amount { get; set; }

		public DateTime Date { get; set; }

		public double Points { get; set; }
	}
}
