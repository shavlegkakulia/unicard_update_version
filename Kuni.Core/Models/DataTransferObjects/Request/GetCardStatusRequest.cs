using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kuni.Core.Models.DataTransferObjects
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

