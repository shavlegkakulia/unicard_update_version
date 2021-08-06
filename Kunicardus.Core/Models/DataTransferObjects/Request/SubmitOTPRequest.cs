using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Core.Models.DataTransferObjects
{
    public class SubmitOTPRequest : UnicardApiBaseRequest
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("card")]
        public string Card { get; set; }

        [JsonProperty("phone")]
        public string PhoneNumber { get; set; }

        [JsonProperty("otp")]
        public string OTP { get; set; }

        public SubmitOTPRequest(): base()
        {

        }
    }
}
