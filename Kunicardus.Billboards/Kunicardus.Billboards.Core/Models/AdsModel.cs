using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunicardus.Billboards.Core.Models
{
	public class AdsModel
	{
        public int HistoryId { get; set; }

		public int BillboardId { get; set; }

		public int AdvertismentId { get; set; }

		public string MerchantLogo { get; set; }

		public string MerchantName { get; set; }

		public decimal Points { get; set; }

		public DateTime PassDate { get; set; }

		public string UserId { get; set; }
	}
}