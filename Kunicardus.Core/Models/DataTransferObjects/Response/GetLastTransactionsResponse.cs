using System;
using Kunicardus.Core.UnicardApiProvider;
using System.Collections.Generic;
using Newtonsoft.Json;
using Kunicardus.Core.Models.DataTransferObjects;

namespace Kunicardus.Core.Models
{
	public class GetLastTransactionsResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("lasttransactions")]
		public List<MerchantModel> Merchants {
			get;
			set;
		}
	}
}

