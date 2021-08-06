using System;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class MerchantModel
	{
		[JsonProperty ("merch_name")]
		public string MerchantName {
			get;
			set;
		}

		[JsonProperty ("org_id")]
		public string MerchantId {
			get;
			set;
		}
	}
}

