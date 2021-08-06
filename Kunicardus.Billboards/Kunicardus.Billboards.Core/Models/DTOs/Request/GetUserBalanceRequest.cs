using System;
using Newtonsoft.Json;
using Kunicardus.Billboards.Core.UnicardApiProvider;

namespace Kunicardus.Billboards.Core.Models.DTOs
{
	public class GetUserBalanceRequest : UnicardApiBaseRequest
	{
		[JsonProperty ("user_id")]
		public string UserId {
			get;
			set;
		}

		public GetUserBalanceRequest ()
		{
		}
	}
}

