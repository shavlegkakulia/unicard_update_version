using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Core.Models.DataTransferObjects.Response
{
    public class GetNewsDetailsResponse : UnicardApiBaseResponse
    {

        [JsonProperty("news")]
        public GetNewsResponse News { get; set; }
    }
}
