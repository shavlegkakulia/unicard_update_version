using System;
using Kuni.Core.UnicardApiProvider;
using System.Collections.Generic;
using Newtonsoft.Json;
using Kuni.Core.Models.DataTransferObjects;

namespace Kuni.Core.Models
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

