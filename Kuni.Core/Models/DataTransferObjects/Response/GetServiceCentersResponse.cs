using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kuni.Core.Models.DataTransferObjects
{
	public class GetServiceCentersResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("service_centers")]
		public List<ServiceCenterDTO> ServiceCenters  { get; set; }
	}
}

