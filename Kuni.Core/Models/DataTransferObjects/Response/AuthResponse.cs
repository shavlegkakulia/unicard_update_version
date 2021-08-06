using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kuni.Core.Models.DataTransferObjects
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

