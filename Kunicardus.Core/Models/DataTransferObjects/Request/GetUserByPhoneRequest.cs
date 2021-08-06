using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models
{
	public class GetUserByPhoneRequest : UnicardApiBaseRequest
	{
		[JsonProperty ("user_name")]
		public string UserName{ get; set; }
	}
}

