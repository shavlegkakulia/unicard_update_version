using System;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System.Collections.Generic;
using Kunicardus.Billboards.Core.Models.DTOs.Response;

namespace Kunicardus.Billboards.Core
{
	public class GetAdvertisementResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("Advertisement")]
		public AdvertisementModel Advertisement{ get; set; }
	}
}

