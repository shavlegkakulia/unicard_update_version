using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kuni.Core.Models.DataTransferObjects
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

