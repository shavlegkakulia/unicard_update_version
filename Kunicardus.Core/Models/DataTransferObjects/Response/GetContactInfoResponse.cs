using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Core.Models.DataTransferObjects.Response
{
    public class GetContactInfoResponse : UnicardApiBaseResponse
    {
        
		[JsonProperty ("contact_email")]
        public string Email { get; set; }
		
        [JsonProperty ("fb_link")]        
        public string Facebook { get; set; }

		[JsonProperty ("phone")]
        public string PhoneNumber { get; set; }

		[JsonProperty ("web_page_link")]
        public string WebPage { get; set; }

		[JsonProperty ("work_hours")]
        public string WorkHours { get; set; }
    }
}
