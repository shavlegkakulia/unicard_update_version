using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Core.Models.DataTransferObjects.Response
{
    public class GetNewsListResponse : UnicardApiBaseResponse
    {
        [JsonProperty("news")]
        public List<GetNewsResponse> News { get; set; }
    }
}
