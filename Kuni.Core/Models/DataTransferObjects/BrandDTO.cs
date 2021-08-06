using System;
using Newtonsoft.Json;

namespace Kuni.Core.Models.DataTransferObjects
{
	public class BrandDTO
	{
		[JsonProperty ("")]
		public int BrandID { get; set; }

		[JsonProperty ("")]
		public string BrandName{ get; set; }
	}
}

