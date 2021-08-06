using System;
using Newtonsoft.Json;

namespace Kunicardus.Core
{
	public class ServiceCenterDTO
	{
		[JsonProperty ("id")]
		public int ID { get; set; }

		[JsonProperty ("name")]
		public string Name { get; set; }
	}
}

