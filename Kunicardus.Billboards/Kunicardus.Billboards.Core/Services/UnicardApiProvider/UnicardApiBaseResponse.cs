using System;
using Newtonsoft.Json;

namespace Kunicardus.Billboards.Core.UnicardApiProvider
{
	public class UnicardApiBaseResponse
	{
		[JsonProperty ("ResultCode")]
		public string ResultCode { get; set; }

		[JsonProperty ("ErrorMessage")]
		public string ErrorMessage { get; set; }

		[JsonProperty ("DisplayText")]
		public string DisplayMessage { get; set; }

		[JsonIgnore]
		public bool Successful {
			get { return (ResultCode == "200" || string.IsNullOrEmpty(ResultCode)); }
		}
	}
}

