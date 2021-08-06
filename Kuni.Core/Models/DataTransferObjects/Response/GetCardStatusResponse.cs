using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kuni.Core.Models.DataTransferObjects
{
	public class GetCardStatusResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("card_status")]
		public bool CardIsValid {
			get;
			set;
		}

		[JsonProperty ("has_transaction")]
		public bool HasTransactions {
			get;
			set;
		}

		[JsonProperty ("is_registered")]
		public bool ExistsUserData {
			get;
			set;
		}
	}
}

