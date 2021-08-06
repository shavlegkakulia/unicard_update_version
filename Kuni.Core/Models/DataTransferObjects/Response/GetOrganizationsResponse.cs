using System;
using Kuni.Core.UnicardApiProvider;
using System.Collections.Generic;
using Newtonsoft.Json;
using Kuni.Core.Models.DataTransferObjects.Response;

namespace Kuni.Core.Models.DataTransferObjects.Response
{
	public class GetOrganizationsResponse : UnicardApiBaseResponse
	{
        [JsonProperty("total_count")]
		public int TotalCount{ get; set; }

        [JsonProperty("organizations")]
        public List<OrganizationItemResponse> Organizations { get; set; }
	}
}

