using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuni.Core.Models.DataTransferObjects.Request
{
    public class GetMerchantsRequest : UnicardApiBaseRequest
    {
        [JsonProperty("org_id")]
        public int? OrganizationId { get; set; }
        [JsonProperty("city_id")]
        public int? CityId { get; set; }
        [JsonProperty("district_id")]
        public int? DistrictId { get; set; }
    }
}
