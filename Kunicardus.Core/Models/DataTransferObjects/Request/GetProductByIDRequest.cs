using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models
{
	public class GetProductByIDRequest : UnicardApiBaseRequest
	{
		[JsonProperty ("product_id")]
		public int ProductID {
			get;
			set;
		}

		[JsonProperty ("user_id")]
		public int UserId {
			get;
			set;
		}
	}
}

