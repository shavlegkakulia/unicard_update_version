using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class RegisterUserResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("result")]
		public string Result { get; set; }

		[JsonProperty ("session_id")]
		public string SessionId { get; set; }

		[JsonProperty ("user_id")]
		public string UserId { get; set; }
	}
}

