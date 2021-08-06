using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models
{
	public class ChangePasswordRequest: UnicardApiBaseRequest
	{
		[JsonProperty ("user_id")]
		public string UserId { get; set; }

		[JsonProperty ("password")]
		public string OldPassword { get; set; }

		[JsonProperty ("new_password")]
		public string NewPassword { get; set; }
	}
}

