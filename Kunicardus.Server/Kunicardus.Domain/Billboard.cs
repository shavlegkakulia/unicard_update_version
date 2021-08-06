using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Domain
{
    public class Billboard
    {
        public int BillboardId { get; set; }
        public virtual Advertisement Advertisement { get; set; }
        public virtual UploadedFile MerchantLogo { get; set; }
        public string Code { get; set; }
        public string MerchantName { get; set; }
        public decimal Points { get; set; }
        public double AlertDistance { get; set; }
        public float StartBearing { get; set; }
        public float EndBearing { get; set; }
        public DbGeography Location { get; set; }
    }
}
