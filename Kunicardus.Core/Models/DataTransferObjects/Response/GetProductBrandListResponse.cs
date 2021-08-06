using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System.Collections.Generic;
using Kunicardus.Core.Models.DataTransferObjects;

namespace Kunicardus.Core.Models
{
	public class GetProductBrandListResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("")]
		public  List<BrandDTO> Brands{ get; set; }
	}
}

