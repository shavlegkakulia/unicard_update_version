using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class GetLastTransactionsRequest : UnicardApiBaseRequest
	{
		[JsonProperty ("card")]
		public string CardNumber {
			get;
			set;
		}
	}
}

