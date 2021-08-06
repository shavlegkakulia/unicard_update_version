using System;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Billboards.Core
{
	public class GetAdvertisementRequest : UnicardApiBaseRequestForMethods
	{
		[JsonProperty ("BillBoardId")]
		public int BillboardId { get; set; }

		[JsonProperty ("UserId")]
		public string UserId { get; set; }

		[JsonProperty ("PassDate")]
		public string PassDate { get; set; }

	}
}

