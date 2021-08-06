using System;
using SQLite;
using Newtonsoft.Json;

namespace Kuni.Core.Models.DB
{
	public class DeliveryMethod:DBModel
	{
		[PrimaryKey]
		[JsonProperty ("id")]
		public int DeliveryMethodId { get; set; }

		[JsonProperty ("text")]
		public string Name { get; set; }

		[JsonProperty ("note")]
		public string Note { get; set; }
	}
}

