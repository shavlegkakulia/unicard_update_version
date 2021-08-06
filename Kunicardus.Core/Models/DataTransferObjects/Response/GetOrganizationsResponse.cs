using System;
using Kunicardus.Core.UnicardApiProvider;
using System.Collections.Generic;
using Newtonsoft.Json;
using Kunicardus.Core.Models.DataTransferObjects.Response;

namespace Kunicardus.Core.Models.DataTransferObjects.Response
{
	public class GetOrganizationsResponse : UnicardApiBaseResponse
	{
        [JsonProperty("total_count")]
		public int TotalCount{ get; set; }

        [JsonProperty("organizations")]
        public List<OrganizationItemResponse> Organizations { get; set; }
	}
}

