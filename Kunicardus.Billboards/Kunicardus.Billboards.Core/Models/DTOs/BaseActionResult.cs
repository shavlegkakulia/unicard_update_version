using System;

namespace Kunicardus.Billboards.Core.Models
{
	public class BaseActionResult<T>
	{
		public bool Success { get; set; }
        public string ResultCode { get; set; }

		public string DisplayMessage { get; set; }

		public T Result { get; set; }
	}
}

