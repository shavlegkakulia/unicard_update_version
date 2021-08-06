using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kuni.Core.Models
{
	public class GetUserByPhoneRequest : UnicardApiBaseRequest
	{
		[JsonProperty ("user_name")]
		public string UserName{ get; set; }
	}
}

