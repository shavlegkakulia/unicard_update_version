using System;
using System.Collections.Generic;

namespace Kunicardus.Core.Models
{
	
	public class ProductModel
	{

		public int ProductID { get; set; }

		public string ProductName { get; set; }

		public string ProductDescription { get; set; }

		public int ProductPrice { get; set; }

		public float DiscountPrice { get; set; }

		public string DiscountDescription { get; set; }

		public int DiscountPercent { get; set; }

		public int CategoryID { get; set; }

		public int? CustomerTypeID { get; set; }

		public string BrandID { get; set; }

		public string DiscountPercentString { get { return this.DiscountPercent.ToString () + "%"; } set { } }

		private string _imageURL;

		public string ImageURL { get ; set; }

		public List<string> ImageUrls { get; set; }
	}
}

