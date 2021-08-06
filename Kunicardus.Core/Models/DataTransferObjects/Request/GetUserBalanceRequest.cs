using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DataTransferObjects
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

