using System;
using Newtonsoft.Json;

namespace Kunicardus.Billboards.Core
{
	public class HistoryModel
	{
		[JsonProperty ("address")]
		public string Address { get; set; }

		[JsonProperty ("amount_gel")]
		public float AmountGel { get; set; }

		[JsonProperty ("date")]
		public DateTime	Date { get; set; }

		[JsonProperty ("date24")]
		public DateTime	Date24 { get; set; }

		[JsonProperty ("organisation_id")]
		public int	OrgId { get; set; }

		[JsonProperty ("organisation_name")]
		public string OrgName { get; set; }

		[JsonProperty ("score")]
		public float Score { get; set; }

		[JsonProperty ("img_url")]
		public string ImageUrl { get; set; }

	}
}

