using System;
using Kuni.Core.UnicardApiProvider;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kuni.Core
{
	public class GetOnlinePaymentInfoResponse: UnicardApiBaseResponse
	{
		[JsonProperty ("debit_score")]
		public string DebitScore  { get; set; }

		[JsonProperty ("payment_info_list")]
		public List<PaymentInfoDTO> PaymentInfos  { get; set; }
	}
}

