using System;
using Kuni.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kuni.Core.Models
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

