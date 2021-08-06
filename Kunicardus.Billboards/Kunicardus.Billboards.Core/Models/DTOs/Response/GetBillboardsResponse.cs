using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;







using Kunicardus.Billboards.Core.UnicardApiProvider;
using Kunicardus.Billboards.Core.Services.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Billboards.Core.Models.DTOs.Response
{
	public class GetBillboardsResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("billboards")]
		public List<BillboardsModel> Billboards { get; set; }
	}
}