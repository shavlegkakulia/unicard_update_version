using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kunicardus.Billboards.Core.UnicardApiProvider;








namespace Kunicardus.Billboards.Core.Models.DTOs.Request
{
	public class GetAdvertisementsRequest : UnicardApiBaseRequestForMethods
	{
		public List<int> BillboardIds { get; set; }
	}
}