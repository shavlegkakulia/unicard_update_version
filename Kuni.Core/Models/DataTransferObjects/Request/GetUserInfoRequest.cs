using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kuni.Core.Models.DataTransferObjects
{
	public class GetUserInfoRequest : UnicardApiBaseRequest
	{
		[JsonProperty ("user_id")]
		public string UserId {
			get;
			set;
		}
	}
}

