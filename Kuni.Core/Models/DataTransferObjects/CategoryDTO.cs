using System;
using Newtonsoft.Json;

namespace Kuni.Core.Models.DataTransferObjects
{
	public class CategoryDTO
	{
		[JsonProperty ("id")]
		public string CategoryID { get; set; }

		[JsonProperty ("is_hidden")]
		public bool IsHidden { get; set; }

		[JsonProperty ("name")]
		public string CategoryName { get; set; }

	}
}

