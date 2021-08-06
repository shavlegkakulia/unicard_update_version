using System;
using Kunicardus.Core.UnicardApiProvider;
using System.Collections.Generic;
using Kunicardus.Core.Models.DataTransferObjects;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models
{
	public class GetUserTypesResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("customer_types")]
		public List<UserTypeDTO> UserTypes{ get; set; }
	}
}

