using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;








namespace Kunicardus.Billboards.Core.Models.DTOs.Request
{
    public class SaveAdvertisementPointsRequest
    {
        public string UserId { get; set; }
        public int AdvertisementId { get; set; }
    }
}