using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuni.Core.Models.DataTransferObjects.Request
{
    public class GetAccountStatementRequest : UnicardApiBaseRequest
    {
        [JsonProperty("user_id")]
        public int UserId { get; set; }
        [JsonProperty("row_count")]
        public int? RowCount { get; set; }
        [JsonProperty("row_index")]
        public int? RowIndex { get; set; }
    }
}
