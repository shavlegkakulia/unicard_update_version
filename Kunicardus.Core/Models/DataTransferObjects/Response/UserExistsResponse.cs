using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class UserExistsResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("exists")]
		public bool Exists {
			get;
			set;
		}
	}
}

