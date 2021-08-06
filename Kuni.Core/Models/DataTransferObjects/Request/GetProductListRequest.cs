using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;
using Kuni.Core.Models.DB;
using System.Collections.Generic;

namespace Kuni.Core
{
	public class GetProductListRequest : UnicardApiBaseRequest
	{
		[JsonProperty ("page_index")]
		public int? PageIndex { get; set; }

		[JsonProperty ("row_count")]
		public int? RowCount { get; set; }

		[JsonProperty ("latest_prod")]
		public int LatestProduct { get; set; }

		[JsonProperty ("category_id")]
		public int? CategoryID { get; set; }

		[JsonProperty ("sub_category_id")]
		public int? SubCategoryID { get; set; }

		[JsonProperty ("brand_id")]
		public int? BrandID { get; set; }

		[JsonProperty ("customer_type_id")]
		public int? CustomerTypeID { get; set; }

		[JsonProperty ("delivery_method_id")]
		public int? DeliveryMethodID { get; set; }

		[JsonProperty ("product_type_id")]
		public int? ProductTypeID { get; set; }

		[JsonProperty ("special_offers")]
		public bool? SpecialOffers { get; set; }

		[JsonProperty ("discounted")]
		public int? Discounted { get; set; }

		[JsonProperty ("price_range_id")]
		public int? PriceRangeId  { get; set; }

		[JsonProperty ("name")]
		public string Name  { get; set; }

		[JsonProperty ("user_id")]
		public int UserId  { get; set; }
	}
}

