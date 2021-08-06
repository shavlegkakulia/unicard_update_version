using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Kunicardus.Billboards.Core.Services.UnicardApiProvider
{
	public class BillboardsApiBaseRequest
	{
		[JsonProperty ("lang")]
		public string Language { get; set; }

		[JsonProperty ("app_source")]
		public string Channel { get; set; }

		public BillboardsApiBaseRequest ()
		{
			Language = "ka";
			Channel = "MOBAPP";
		}
	}
}