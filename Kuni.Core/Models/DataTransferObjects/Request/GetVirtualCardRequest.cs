using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kuni.Core.Models.DataTransferObjects
{
	public class GetVirtualCardRequest : UnicardApiBaseRequest
	{
		[JsonProperty ("user_id")]
		public string UserId {
			get;
			set;
		}

		public GetVirtualCardRequest () : base ()
		{
		}
	}
}

