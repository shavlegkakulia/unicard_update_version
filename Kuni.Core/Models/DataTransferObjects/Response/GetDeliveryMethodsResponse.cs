using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;
using Kuni.Core.Models.DataTransferObjects;
using Kuni.Core.Models.DB;
using System.Collections.Generic;

namespace Kuni.Core
{
	public class GetDeliveryMethodsResponse:UnicardApiBaseResponse
	{
		[JsonProperty ("delivery_methods")]
		public List<DeliveryMethod> Methods{ get; set; }
	}
}

