using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using Newtonsoft.Json;
using Kunicardus.Billboards.Core.Enums;








namespace Kunicardus.Billboards.Core.Models.DTOs.Response
{
	public class AccumulatePointsResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("AccumulatedBonus")]
		public int AccumulatedBonus { get; set; }

		[JsonIgnore]
		public AdvertismentStatus Status { get; set; }
	}
}