using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class SendOTPRequest : UnicardApiBaseRequest
	{
		[JsonProperty ("user_id")]
		public string UserId { get; set; }

		[JsonProperty ("user_name")]
		public string UserName { get; set; }

		[JsonProperty ("card")]
		public string Card { get; set; }

		[JsonProperty ("phone")]
		public string PhoneNumber { get; set; }

		public SendOTPRequest () : base ()
		{

		}
	}
}
