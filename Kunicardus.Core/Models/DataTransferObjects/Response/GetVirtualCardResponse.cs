using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class GetVirtualCardResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("vcard")]
		public string CardNumber { get; set; }
	}
}

