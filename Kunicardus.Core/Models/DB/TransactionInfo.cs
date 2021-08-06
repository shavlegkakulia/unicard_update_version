using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Kunicardus.Core.Models.DB
{
	public class TransactionInfo : DBModel
	{
		[PrimaryKey,AutoIncrement]
		public int TransactionId { get; set; }

		public string Address { get; set; }

		public double PaymentAmount { get; set; }

		public string CardNumber { get; set; }

		public DateTime Date { get; set; }

		public string OrganizationId { get; set; }

		public string OrganizationName { get; set; }

		public double Score { get; set; }

		public string Status { get; set; }

		public string Type { get; set; }

		public bool First { get; set; }

		public bool Last { get; set; }
	}
}
