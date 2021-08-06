using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kuni.Core.Models.DataTransferObjects
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

