using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Services.Models
{
    public class GetAdvertisementsRequest
    {
        public IEnumerable<int> BillboardIds { get; set; }
    }

    public class GetAdvertisementsResponse
    {
        public IEnumerable<AdvertisementModel> Advertisements { get; set; }
    }
}
