using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunicardus.Domain
{
    public class Advertisement
    {
        public int AdvertisementId { get; set; }
        public string Name { get; set; }
        public virtual UploadedFile Image { get; set; }
        public double Points { get; set; }
        public string ExternalUrl { get; set; }
        public int Timeout { get; set; }
        public AdvertisementStatus Status { get; set; }
        public virtual Billboard Billboard { get; set; }

        public virtual ICollection<AdvertisementPoint> UserPoints { get; set; }
    }
}
