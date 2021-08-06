using System;
using Newtonsoft.Json;
using Kuni.Core.UnicardApiProvider;

namespace Kuni.Core
{
	public class GetOnlinePaymentInfoRequest: UnicardApiBaseRequest
	{
		[JsonProperty ("user_id")]
		public string UserId {
			get;
			set;
		}

		[JsonProperty ("product_id")]
		public int ProductId {
			get;
			set;
		}

		[JsonProperty ("subscriber_number")]
		public string SubscriberNumber {
			get;
			set;
		}
	}
}

