using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kuni.Core
{
	public class ResetPasswordRequest : UnicardApiBaseRequest
	{
		[JsonProperty ("user_name")]
		public string UserName { get; set; }

		[JsonProperty ("sms_code")]
		public string SmsCode{ get; set; }

		[JsonProperty ("new_password")]
		public string NewPassword{ get; set; }
	}
}

