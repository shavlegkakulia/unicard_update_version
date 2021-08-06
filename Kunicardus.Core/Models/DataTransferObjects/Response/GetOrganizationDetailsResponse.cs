using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Core.Models.DataTransferObjects.Response
{
    public class GetOrganizationDetailsResponse : UnicardApiBaseResponse
    {
        [JsonProperty("organization")]
        public OrganisationResponse Organisation
        {
            get;
            set;
        }
    }

    public class OrganisationResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("org_desc")]
        public string Description { get; set; }

        [JsonProperty("org_email")]
        public string Email { get; set; }

        [JsonProperty("org_fb")]
        public string FbAddress { get; set; }

        [JsonProperty("org_id")]
        public int OrganizationId { get; set; }

        [JsonProperty("org_phone")]
        public string PhoneNumber { get; set; }

        [JsonProperty("org_phone_s")]
        public List<string> PhoneNumbers { get; set; }

        [JsonProperty("org_service_desc")]
        public string ServiceDescription { get; set; }

        [JsonProperty("org_short_desc")]
        public string ShortDescription { get; set; }

        [JsonProperty("org_working_hours_s")]
        public string WorkingHours { get; set; }

        [JsonProperty("unit_score")]
        public string UnitScore { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }


        [JsonProperty("org_web_add")]
        public string Website { get; set; }

        [JsonProperty("url")]
        public string ImageUrl { get; set; }

        [JsonProperty("unit_desc")]
        public string UnitDescription { get; set; }
    }
}
