using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kuni.Core.Models
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

