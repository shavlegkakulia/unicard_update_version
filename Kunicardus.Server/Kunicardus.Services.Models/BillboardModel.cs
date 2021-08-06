using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunicardus.Services.Models
{
    public class BillboardModel
    {
        public int BillboardId { get; set; }
        public int AdvertismentId { get; set; }
        public string MerchantLogo { get; set; }
        public string MerchantName { get; set; }
        public decimal Points { get; set; }
        public double AlertDistance { get; set; }
        public float StartBearing { get; set; }
        public float EndBearing { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
