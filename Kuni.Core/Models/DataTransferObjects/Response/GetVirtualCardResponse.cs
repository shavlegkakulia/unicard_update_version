using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kuni.Core.Models.DataTransferObjects
{
	public class GetVirtualCardResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("vcard")]
		public string CardNumber { get; set; }
	}
}

