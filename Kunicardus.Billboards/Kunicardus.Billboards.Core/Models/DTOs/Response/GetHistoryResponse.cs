using System;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kunicardus.Billboards.Core
{
	public class GetHistoryResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("total_count")]
		public int TotalCount {
			get;
			set;
		}

		[JsonProperty ("transactions")]
		public List<HistoryModel> Transactions { get; set; }
	}
}

