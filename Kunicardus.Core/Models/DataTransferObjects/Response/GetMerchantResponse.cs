using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Core.Models.DataTransferObjects.Response
{
    public class GetMerchantResponse : UnicardApiBaseResponse
    {
        [JsonProperty("merch_id")]
        public string MerchantId { get; set; }

        [JsonProperty("merch_name")]
        public string MerchantName { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("city")]
        public int CityId { get; set; }

        [JsonProperty("district")]
        public int DistrictId { get; set; }

        [JsonProperty("lon")]
        public double? Longitude { get; set; }

        [JsonProperty("lat")]
        public double? Latitude { get; set; }

        [JsonProperty("new_id")]
        public int OrganizationId { get; set; }

        [JsonProperty("org_point_desc")]
        public string OrgnizationPointsDesc { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("unit_score")]
        public string UnitScore { get; set; }

        [JsonProperty("logo_url")]
        public string Image { get; set; }

        [JsonProperty("unit_desc")]
        public string UnitDescription { get; set; }
    }
}
