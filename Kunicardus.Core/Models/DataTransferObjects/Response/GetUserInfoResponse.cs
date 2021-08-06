using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class GetUserInfoResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("add_email")]
		public string AdditionalEmail {
			get;
			set;
		}

		[JsonProperty ("name")]
		public string FirstName {
			get;
			set;
		}

		[JsonProperty ("surname")]
		public string LastName {
			get;
			set;
		}

		[JsonProperty ("address")]
		public string Address {
			get;
			set;
		}

		[JsonProperty ("full_address")]
		public string FullAddress {
			get;
			set;
		}

		[JsonProperty ("phone")]
		public string Phone {
			get;
			set;
		}

		[JsonProperty ("person_code")]
		public string PersonalId {
			get;
			set;
		}
	}
}

