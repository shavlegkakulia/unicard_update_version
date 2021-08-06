using System;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Billboards.Core
{
	public class GetHistoryRequest : UnicardApiBaseRequestForMethods
	{
		[JsonProperty ("user_id")]
		public string UserId {
			get;
			set;
		}
	}
}

