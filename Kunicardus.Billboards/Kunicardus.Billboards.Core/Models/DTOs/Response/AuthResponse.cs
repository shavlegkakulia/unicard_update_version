using System;
using Newtonsoft.Json;
using Kunicardus.Billboards.Core.UnicardApiProvider;

namespace Kunicardus.Billboards.Core.Models.DataTransferObjects
{
	public class AuthResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("user_id")]
		public string UserId {
			get;
			set;
		}

		[JsonProperty ("session_id")]
		public string SessionID{ get; set; }
	}
}

