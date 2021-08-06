using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;







using Kunicardus.Billboards.Core.Services.UnicardApiProvider;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kunicardus.Billboards.Core.Models.DTOs.Response
{
	public class GetAdvertismentsResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("Advertisements")]
		public List<AdvertisementModel> Advertisements { get; set; }
	}
}