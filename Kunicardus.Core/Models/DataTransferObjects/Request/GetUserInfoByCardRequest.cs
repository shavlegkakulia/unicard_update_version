using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class GetUserInfoByCardRequest : UnicardApiBaseRequest
	{
		[JsonProperty ("card")]
		public string Card {
			get;
			set;
		}
	}
}

