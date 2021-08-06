using System;

namespace Kuni.Core.Models
{
	public class CheckTransactionModel
	{
		public string CardNumber { get; set; }

		public string OrgId { get; set; }

		public string Amount { get; set; }

		public DateTime TranDate{ get; set; }
	}
}

