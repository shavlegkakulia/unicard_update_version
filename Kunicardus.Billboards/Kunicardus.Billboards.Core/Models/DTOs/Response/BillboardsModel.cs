using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;








namespace Kunicardus.Billboards.Core.Models.DTOs.Response
{
	public class BillboardsModel
	{
		[JsonProperty ("BillboardId")]
		public int BillboardId { get; set; }

		[JsonProperty ("ad_id")]
		public int AdvertismentId { get; set; }

		[JsonProperty ("AlertDistance")]
		public decimal AlertDistance { get; set; }

		[JsonProperty ("BrandLogo")]
		public string MerchantLogo { get; set; }

		[JsonProperty ("BrandName")]
		public string MerchantName { get; set; }

		[JsonProperty ("")]
		public decimal Points { get; set; }

		[JsonProperty ("StartBearing")]
		public decimal StartBearing { get; set; }

		[JsonProperty ("EndBearing")]
		public decimal EndBearing { get; set; }

		[JsonProperty ("Latitude")]
		public decimal Latitude { get; set; }

		[JsonProperty ("Longitude")]
		public decimal Longitude { get; set; }

		[JsonProperty ("IsActive")]
		public bool IsActive { get; set; }
	}
}