using System;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class UserTypeDTO
	{
		[JsonProperty ("id")]
		public int UserTypeID { get; set; }

		[JsonProperty ("name")]
		public string UserTypeName { get; set; }
	}
}

