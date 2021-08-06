using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using Kunicardus.Billboards.Core.Enums;

namespace Kunicardus.Billboards.Core.DbModels
{
	public class Billboard
	{
		[PrimaryKey]
		public int BillboardId { get; set; }

		public int AdvertismentId { get; set; }

		public string MerchantLogo { get; set; }

		public string MerchantName { get; set; }

		public decimal Points { get; set; }

		public decimal AlertDistance { get; set; }

		[Ignore]
		public decimal Distance { get; set; }

		public decimal StartBearing { get; set; }

		public decimal EndBearing { get; set; }

		public decimal Latitude { get; set; }

		public decimal Longitude { get; set; }
	}
}