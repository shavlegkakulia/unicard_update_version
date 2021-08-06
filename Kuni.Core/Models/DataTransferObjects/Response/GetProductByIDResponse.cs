using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System.Collections.Generic;
using Kuni.Core.Models.DB;

namespace Kuni.Core.Models
{
	public class GetProductByIDResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("name")]
		public string ProductName { get; set; }

		[JsonProperty ("description")]
		public string PoductDescription { get; set; }

		[JsonProperty ("category_id")]
		public int CategoryID { get; set; }

		[JsonProperty ("sub_category_id")]
		public int SubCategoryID { get; set; }

		[JsonProperty ("brand_id")]
		public int? BrandID { get; set; }

		[JsonProperty ("customer_type_id")]
		public int? CustomerTypeID { get; set; }

		[JsonProperty ("delivery_methods")]
		public List<DeliveryMethod> DeliveryMethods { get; set; }

		[JsonProperty ("type_id")]
		public int ProductTypeID { get; set; }

		[JsonProperty ("price")]
		public int? ProductPrice { get; set; }

		[JsonProperty ("discounted_price")]
		public int? DiscountedPrice { get; set; }

		[JsonProperty ("discounted_percent")]
		public int? DiscountedPercent { get; set; }

		[JsonProperty ("catalog_id")]
		public string CatalogID { get; set; }

		[JsonProperty ("images")]
		public List<string> ProductImages { get; set; }

		[JsonProperty ("user_discounts")]
		public List<DiscountDTO> UserDiscounts{ get; set; }
	}
}

