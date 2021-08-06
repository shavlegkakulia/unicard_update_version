using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using Kunicardus.Core.UnicardApiProvider;

namespace Kunicardus.Core
{
	public class PriceRangeDTO:UnicardApiBaseResponse
	{
		[JsonProperty ("price_ranges")]
		public List<PriceRange> PriceRanges  { get; set; }
	}

	public class PriceRange
	{
		[JsonProperty ("id")]
		public int ID  { get; set; }

		[JsonProperty ("range_description")]
		public string Name  { get; set; }
	}
}

