using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuni.Core.Models.DataTransferObjects.Response
{
    public class GetAccountStatementResponse : UnicardApiBaseResponse
    {
        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonProperty("transactions")]
        public List<GetTransactionsResponse> Transactions { get; set; }
    }
}
