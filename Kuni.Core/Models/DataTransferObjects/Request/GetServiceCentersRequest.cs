using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kuni.Core.Models.DataTransferObjects
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

