using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;
using Kunicardus.Core.Models.DataTransferObjects;
using System.Collections.Generic;

namespace Kunicardus.Core.Models
{
	public class GetProductListResponse: UnicardApiBaseResponse
	{
		[JsonProperty ("total_count")]
		public int TotalCount { get; set; }

		[JsonProperty ("products")]
		public List<ProductDTO> Products{ get; set; }
	}
}

