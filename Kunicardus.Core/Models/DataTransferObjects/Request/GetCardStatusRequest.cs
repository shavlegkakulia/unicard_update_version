using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class GetCardStatusRequest : UnicardApiBaseRequest
	{
		[JsonProperty ("card")]
		public string CardNumber {
			get;
			set;
		}

		public GetCardStatusRequest () : base ()
		{
		}
	}
}

