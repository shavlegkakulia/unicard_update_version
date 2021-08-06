using System;
using Newtonsoft.Json;

namespace Kunicardus.Billboards.Core
{
	public class AccumulatePointsRequest : UnicardApiBaseRequestForMethods
	{
		[JsonProperty ("user_id")]
		public string UserId { get; set; }

		[JsonProperty ("billboard_id")]
		public int BillBoardId { get; set; }

		[JsonProperty ("advertisement_id")]
		public int AdvertisementId { get; set; }

		[JsonProperty ("pass_date")]
		public string PassDate { get; set; }

		[JsonProperty ("view_date")]
		public string ViewDate { get; set; }

		[JsonProperty ("stan")]
		public Guid GuID { get; set; }
	}
}

