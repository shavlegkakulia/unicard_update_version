using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuni.Core.Models.DataTransferObjects.Request
{
    public class GetNewsListRequest : UnicardApiBaseRequest
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}
