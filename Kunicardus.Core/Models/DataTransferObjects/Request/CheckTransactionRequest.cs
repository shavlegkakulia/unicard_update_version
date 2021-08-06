using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core
{
	public class CheckTransactionRequest : UnicardApiBaseRequest
	{
		[JsonProperty ("card")]
		public string UnicardNumber { get; set; }

		[JsonProperty ("org_id")]
		public string MerchantId { get; set; }

		[JsonProperty ("amount")]
		public string Amount { get; set; }

		[JsonProperty ("tran_date")]
		public DateTime? TransactionDate { get; set; }
	}
}

