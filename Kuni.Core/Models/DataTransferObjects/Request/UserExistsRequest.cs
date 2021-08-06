using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kuni.Core.Models.DataTransferObjects
{
	public class UserExistsRequest : UnicardApiBaseRequest
	{
		[JsonProperty ("user_name")]
		public string UserName {
			get;
			set;
		}
	}
}

