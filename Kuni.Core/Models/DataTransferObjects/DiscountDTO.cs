using System;
using Newtonsoft.Json;

namespace Kuni.Core
{
	public class DiscountDTO
	{
		[JsonProperty ("id")]
		public int DiscountID { get; set; }

		[JsonProperty ("description")]
		public string DiscountDescription { get; set; }

		[JsonProperty ("discounted_percent")]
		public int DiscountedPercent { get; set; }
	}
}

