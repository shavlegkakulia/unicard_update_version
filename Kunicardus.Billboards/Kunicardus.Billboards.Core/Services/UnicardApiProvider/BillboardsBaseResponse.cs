using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;







using Kunicardus.Billboards.Core.UnicardApiProvider;

namespace Kunicardus.Billboards.Core.Services.UnicardApiProvider
{
	public class BillboardsBaseResponse<T> : UnicardApiBaseResponse
	{
		public T Result { get; set; }
	}
}