using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kuni.Core.Models.DataTransferObjects.Request
{
	public class GetOrganizationsRequest : UnicardApiBaseRequest
	{
        [JsonProperty("page_index")]
        public int PageIndex { get; set; }

        [JsonProperty("row_count")]
		public int RowCount{ get; set; }

        [JsonProperty("sector_id")]
		public int? SectorId { get; set; }

        [JsonProperty("sub_sector_id")]
		public int? SubSectorId { get; set; }

        [JsonProperty("latest_org")]
        public bool LatestOrg { get; set; }

        [JsonProperty("by_score")]
        public bool ByScore { get; set; }
	}
}

