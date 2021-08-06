using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kuni.Core.Models.DataTransferObjects
{
	public class ProductDTO
	{
		[JsonProperty ("id")]
		public int ProductID { get; set; }

		[JsonProperty ("name")]
		public string ProductName { get; set; }

		[JsonProperty ("price")]
		public int? ProductPrice { get; set; }

		[JsonProperty ("discounted_price")]
		public float? DiscountPrice { get; set; }

		[JsonProperty ("discounted_percent")]
		public int? DiscountPercent { get; set; }

		[JsonProperty ("category_id")]
		public int CategoryID { get; set; }

		[JsonProperty ("customer_type_id")]
		public int? CustomerTypeID { get; set; }

		[JsonProperty ("brand_id")]
		public string BrandID { get; set; }

		[JsonProperty ("images")]
		public List<string> ImageURLs { get; set; }
	}
}

