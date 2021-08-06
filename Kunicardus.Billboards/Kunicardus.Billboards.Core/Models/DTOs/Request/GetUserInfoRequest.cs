using System;
using Newtonsoft.Json;
using Kunicardus.Billboards.Core.UnicardApiProvider;

namespace Kunicardus.Billboards.Core.Models.DataTransferObjects
{
	public class GetUserInfoRequest : UnicardApiBaseRequest
	{
		[JsonProperty ("user_id")]
		public string UserId {
			get;
			set;
		}
	}
}

