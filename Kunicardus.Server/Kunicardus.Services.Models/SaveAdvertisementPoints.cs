using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Services.Models
{
    public class SaveAdvertisementPointsRequest
    {
        public int UserId { get; set; }
        public int AdvertisementId { get; set; }
    }

    public class SaveAdvertisementPointsResponse
    {
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; }
    }
}
