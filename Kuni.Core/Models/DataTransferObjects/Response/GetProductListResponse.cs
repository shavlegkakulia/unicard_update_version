using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;
using Kuni.Core.Models.DataTransferObjects;
using System.Collections.Generic;

namespace Kuni.Core.Models
{
	public class GetProductListResponse: UnicardApiBaseResponse
	{
		[JsonProperty ("total_count")]
		public int TotalCount { get; set; }

		[JsonProperty ("products")]
		public List<ProductDTO> Products{ get; set; }
	}
}

