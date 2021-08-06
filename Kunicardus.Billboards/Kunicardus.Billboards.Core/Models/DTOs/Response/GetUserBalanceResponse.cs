using System;
using Newtonsoft.Json;
using Kunicardus.Billboards.Core.UnicardApiProvider;

namespace Kunicardus.Billboards.Core.Models.DTOs
{
	public class GetUserBalanceResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("scores_blocked")]
		public decimal BlockedPoints {
			get;
			set;
		}

		[JsonProperty ("scores_left")]
		public decimal AvailablePoints {
			get;
			set;
		}

		[JsonProperty ("scores_saved")]
		public decimal AccumulatedPoint {
			get;
			set;
		}

		[JsonProperty ("scores_spent")]
		public decimal SpentPoints {
			get;
			set;
		}
	}
}

