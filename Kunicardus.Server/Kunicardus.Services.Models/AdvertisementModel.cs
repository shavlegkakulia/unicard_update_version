using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunicardus.Services.Models
{
    public class AdvertisementModel
    {
        public int AdvertismentId { get; set; }
        public string Image { get; set; }
        public double Points { get; set; }
        public string ExternalLink { get; set; }
        public int TimeOut { get; set; }
    }
}
