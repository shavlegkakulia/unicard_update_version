using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models
{
	public class GetUserByPhoneResponse: UnicardApiBaseResponse
	{
		[JsonProperty ("Phone")]
		public string UserPhoneNumber{ get; set; }

		[JsonProperty ("UserId")]
		public string UserId{ get; set; }
	}
}

