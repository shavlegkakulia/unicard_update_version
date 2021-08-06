using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DataTransferObjects
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

