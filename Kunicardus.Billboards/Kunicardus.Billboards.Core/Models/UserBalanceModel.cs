using System;

namespace Kunicardus.Billboards.Core.Models
{
	public class UserBalanceModel
	{
		public decimal BlockedPoints {
			get;
			set;
		}

		public decimal AvailablePoints {
			get;
			set;
		}

		public decimal AccumulatedPoint {
			get;
			set;
		}


		public decimal SpentPoints {
			get;
			set;
		}
	}
}

