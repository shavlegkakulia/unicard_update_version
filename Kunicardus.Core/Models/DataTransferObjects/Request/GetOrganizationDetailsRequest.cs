using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Core.Models.DataTransferObjects.Request
{
    public class GetOrganizationDetailsRequest : UnicardApiBaseRequest
    {
        [JsonProperty("org_id")]
        public int OrganizationId { get; set; }
    }
}
