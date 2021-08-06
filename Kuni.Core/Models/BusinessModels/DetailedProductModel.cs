using System;
using System.Collections.Generic;
using Kuni.Core.Models.DB;

namespace Kuni.Core.Models
{
	public class DetailedProductModel
	{
		public string ProductName { get; set; }

		public string PoductDescription { get; set; }

		public int CategoryID { get; set; }

		public int SubCategoryID { get; set; }

		public int? BrandID { get; set; }

		public int CustomerTypeID { get; set; }

		public int DeliveryMethodID { get; set; }

		public int ProductTypeID { get; set; }

		public int ProductPrice { get; set; }

		public int DiscountedPrice { get; set; }

		public int DiscountedPercent { get; set; }

		public string CatalogID { get; set; }

		public List<string> ProductImages { get; set; }

		public List<DiscountModel> UserDiscounts{ get; set; }

		public List<DeliveryMethod> DeliveryMethods { get; set; }

	}
}

