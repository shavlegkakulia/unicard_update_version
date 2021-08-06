using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DataTransferObjects
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

