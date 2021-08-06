using System;
using Newtonsoft.Json;

namespace Kuni.Core.UnicardApiProvider
{
	public class UnicardApiBaseRequest
	{
		[JsonProperty ("lang")]
		public string Language { get; set; }

		[JsonProperty ("app_source")]
		public string Channel { get; set; }

		public UnicardApiBaseRequest ()
		{
			Language = "ka";
			Channel = "MOBAPP";
		}
	}
}

