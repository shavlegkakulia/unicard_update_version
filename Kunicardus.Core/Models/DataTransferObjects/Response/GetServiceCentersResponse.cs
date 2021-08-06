using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class GetServiceCentersResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("service_centers")]
		public List<ServiceCenterDTO> ServiceCenters  { get; set; }
	}
}

