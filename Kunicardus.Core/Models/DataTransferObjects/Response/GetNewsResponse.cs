using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunicardus.Core.Models.DataTransferObjects.Response
{

    public class GetNewsResponse
    {
        [JsonProperty("Id")]
        public int Id { get; set; }
        [JsonProperty("CreateDate")]
        public DateTime CreateDate { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("Image")]
        public string Image { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }
    }
}
