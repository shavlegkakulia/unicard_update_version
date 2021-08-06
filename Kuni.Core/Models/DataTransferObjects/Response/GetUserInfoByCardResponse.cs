using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kuni.Core.Models.DataTransferObjects
{
	public class GetUserInfoByCardResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("name")]
		public string Name {
			get;
			set;
		}

		[JsonProperty ("surname")]
		public string Surname {
			get;
			set;
		}

		[JsonProperty ("person_code")]
		public string UserIdNumber {
			get;
			set;
		}

		[JsonProperty ("phone")]
		public string PhoneNumber {
			get;
			set;
		}

		[JsonProperty ("result")]
		public string Result {
			get;
			set;
		}

	}
}

