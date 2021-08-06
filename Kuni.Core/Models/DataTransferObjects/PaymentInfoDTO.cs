using System;
using Newtonsoft.Json;

namespace Kuni.Core
{
	public class PaymentInfoDTO
	{
		[JsonProperty ("description")]
		public string Name  { get; set; }

		[JsonProperty ("value")]
		public string Value  { get; set; }
	}
}

