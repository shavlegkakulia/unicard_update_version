using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DataTransferObjects
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

