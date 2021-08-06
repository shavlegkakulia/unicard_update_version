using System;
using System.Collections.Generic;
using SQLite;

namespace Kunicardus.Core.Models.DB
{
	public class ProductsInfo : DBModel
	{
		[PrimaryKey, AutoIncrement]
		public int LocalId { get; set; }

		public int ProductID { get; set; }

		public string ProductName { get; set; }

		public string ProductDescription { get; set; }

		public int ProductPrice { get; set; }

		public float DiscountPrice { get; set; }

		public string DiscountDescription { get; set; }

		public int DiscountPercent { get; set; }

		public string DiscountPercentString { get { return this.DiscountPercent.ToString () + "%"; } set { } }

		public string ImageURL { get; set; }

		public int CategoryID { get; set; }

		public int? CustomerTypeID { get; set; }

		public string BrandID { get; set; }

		[Ignore]
		public List<string> ImageUrls { get; set; }
	}
}

