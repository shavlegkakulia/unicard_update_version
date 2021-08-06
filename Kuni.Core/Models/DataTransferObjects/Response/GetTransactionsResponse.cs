using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuni.Core.Models.DataTransferObjects.Response
{
    public class GetTransactionsResponse 
    {
        
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("amount_gel")]
        public double PaymentAmount { get; set; }
        [JsonProperty("card")]
        public string CardNumber { get; set; }
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        [JsonProperty("organisation_id")]
        public string OrganizationId { get; set; }
        [JsonProperty("organisation_name")]
        public string OrganizationName { get; set; }
        [JsonProperty("score")]
        public double Score { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
