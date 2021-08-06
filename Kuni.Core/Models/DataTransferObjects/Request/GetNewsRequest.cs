using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuni.Core.Models.DataTransferObjects.Request
{
    public class GetNewsRequest : UnicardApiBaseRequest
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("news_id")]
        public int NewsId { get; set; }
    }
}