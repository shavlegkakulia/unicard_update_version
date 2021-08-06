using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Core.Models.DataTransferObjects.Response
{
    public class OrganizationItemResponse
    {

        [JsonProperty("org_id")]
        public int OrganizationId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("point_desc")]
        public string PointsDescription { get; set; }

        [JsonProperty("sector")]
        public int? SectorId { get; set; }

        [JsonProperty("sub_sector")]
        public int? SubSectorId { get; set; }

        [JsonProperty("url")]
        public string ImageUrl { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("unit_score")]
        public string UnitScore { get; set; }

        [JsonProperty("unit_desc")]
        public string UnitDescription { get; set; }
    }
}
