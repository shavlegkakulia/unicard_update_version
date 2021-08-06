using System;
using Kunicardus.Core.UnicardApiProvider;
using System.Collections.Generic;
using Kunicardus.Core.Models;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class GetProductCategoryListResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("categories")]
		public List<CategoryDTO> Categories{ get; set; }
	}
}

