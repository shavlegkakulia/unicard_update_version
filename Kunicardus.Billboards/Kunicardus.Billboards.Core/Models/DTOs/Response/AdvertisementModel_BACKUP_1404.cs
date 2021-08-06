using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;








namespace Kunicardus.Billboards.Core.Models.DTOs.Response
{
	public class AdvertisementModel
	{
		[JsonProperty ("Id")]
		public int AdvertismentId { get; set; }

		[JsonProperty ("Images")]
		public List<string> Image { get; set; }

		[JsonProperty ("Points")]
		public double Points { get; set; }

<<<<<<< HEAD
        [JsonProperty("ExternalLink")]
=======
		[JsonProperty ("ExternalLink")]
>>>>>>> e60532bac884be9b22ffd788103d03303a998ead
		public string ExternalLink { get; set; }

		[JsonProperty ("ValidTo")]
		public string IsValidTo { get; set; }

		[JsonProperty ("Duration")]
		public int TimeOut { get; set; }

		[JsonProperty ("IsActive")]
		public bool IsActive { get; set; }
	}
}