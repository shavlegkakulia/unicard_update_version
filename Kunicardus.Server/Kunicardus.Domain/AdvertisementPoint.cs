using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunicardus.Domain
{
    public class AdvertisementPoint
    {
        public int AdvertisementPointId { get; set; }
        public string UserId { get; set; }
        public int AdversitementId { get; set; }
        public virtual Advertisement Advertisement { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public double Points { get; set; }
    }
}
