using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System.Collections.Generic;
using Kuni.Core.Models.DataTransferObjects;

namespace Kuni.Core.Models
{
	public class GetProductBrandListResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("")]
		public  List<BrandDTO> Brands{ get; set; }
	}
}

