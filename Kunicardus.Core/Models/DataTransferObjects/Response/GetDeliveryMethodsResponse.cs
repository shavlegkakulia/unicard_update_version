using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;
using Kunicardus.Core.Models.DataTransferObjects;
using Kunicardus.Core.Models.DB;
using System.Collections.Generic;

namespace Kunicardus.Core
{
	public class GetDeliveryMethodsResponse:UnicardApiBaseResponse
	{
		[JsonProperty ("delivery_methods")]
		public List<DeliveryMethod> Methods{ get; set; }
	}
}

