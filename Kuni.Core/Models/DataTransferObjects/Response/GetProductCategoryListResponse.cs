using System;
using Kuni.Core.UnicardApiProvider;
using System.Collections.Generic;
using Kuni.Core.Models;
using Newtonsoft.Json;

namespace Kuni.Core.Models.DataTransferObjects
{
	public class GetProductCategoryListResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("categories")]
		public List<CategoryDTO> Categories{ get; set; }
	}
}

