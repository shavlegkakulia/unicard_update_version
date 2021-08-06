using System;
using Newtonsoft.Json;

namespace Kunicardus.Billboards.Core
{
	public class UnicardApiBaseRequestForMethods
	{

		[JsonProperty ("lang")]
		public string Language { get; set; }

		[JsonProperty ("app_source")]
		public string Channel { get; set; }

		public UnicardApiBaseRequestForMethods ()
		{
			Language = "ka";
			Channel = "UniBoard";
		}
	}
}

