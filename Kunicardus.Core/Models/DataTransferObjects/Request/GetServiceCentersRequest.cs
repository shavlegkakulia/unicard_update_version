using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class GetServiceCentersRequest : UnicardApiBaseRequest
	{
		[JsonProperty ("product_id")]
		public int ProductId {
			get;
			set;
		}

		[JsonProperty ("delivery_method_id")]
		public int DeliveryMethodId {
			get;
			set;
		}
	}
}

