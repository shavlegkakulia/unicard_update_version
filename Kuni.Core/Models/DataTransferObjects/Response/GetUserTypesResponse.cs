using System;
using Kuni.Core.UnicardApiProvider;
using System.Collections.Generic;
using Kuni.Core.Models.DataTransferObjects;
using Newtonsoft.Json;

namespace Kuni.Core.Models
{
	public class GetUserTypesResponse : UnicardApiBaseResponse
	{
		[JsonProperty ("customer_types")]
		public List<UserTypeDTO> UserTypes{ get; set; }
	}
}

