using System;
using Newtonsoft.Json;
using Kunicardus.Billboards.Core.UnicardApiProvider;

namespace Kunicardus.Billboards.Core.Models.DataTransferObjects
{
	public class AuthRequest : UnicardApiBaseRequest
	{
		[JsonProperty ("user_name")]
		public string UserName {
			get;
			set;
		}

		[JsonProperty ("pass")]
		public string Password {
			get;
			set;
		}

		[JsonProperty ("fb_token")]
		public string FacebookId {
			get;
			set;
		}

		public AuthRequest () : base ()
		{
		}
	}
}

