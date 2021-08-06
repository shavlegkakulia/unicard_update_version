using System;
using Newtonsoft.Json;

namespace Kuni.Core.Models.DataTransferObjects
{
	public class SubSector
	{
		[JsonProperty ("")]
		public int SubSectorID { get; set; }

		[JsonProperty ("")]
		public string SubSectorName { get; set; }

		[JsonProperty ("")]
		public int SectorID { get; set; }

		[JsonProperty ("")]
		public string SectorName { get; set; }
	}
}

