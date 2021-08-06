using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Services.Models
{
    public class GetBillboardsRequest
    {

    }

    public class GetBillboardsResponse
    {
        public IEnumerable<BillboardModel> Billboards { get; set; }
    }
}
