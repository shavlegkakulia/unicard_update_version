using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DB
{
	[Table ("NewsInfo")]
	public class NewsInfo : DBModel
	{
		public int Id { get; set; }

		public string UserId { get; set; }

		public DateTime CreateDate { get; set; }

		public string Description { get; set; }

		public string Image { get; set; }

		public string Title { get; set; }

		public bool IsRead { get; set; }
	}
}
