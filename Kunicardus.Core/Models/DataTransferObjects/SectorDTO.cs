using System;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class SectorDTO
	{
		[JsonProperty ("")]
		public int SectorID { get; set; }

		[JsonProperty ("")]
		public string SectorName { get; set; }
	}
}

