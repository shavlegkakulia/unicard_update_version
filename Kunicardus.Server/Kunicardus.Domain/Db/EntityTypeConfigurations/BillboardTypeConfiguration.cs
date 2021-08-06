using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Domain.Db.EntityTypeConfigurations
{
    public class BillboardTypeConfiguration : EntityTypeConfiguration<Billboard>
    {
        public BillboardTypeConfiguration()
        {
            HasRequired(billboard => billboard.Advertisement).WithOptional(advertisement => advertisement.Billboard);
        }
    }
}
