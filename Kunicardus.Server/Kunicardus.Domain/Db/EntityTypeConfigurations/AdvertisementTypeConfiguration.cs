using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Domain.Db.EntityTypeConfigurations
{
    public class AdvertisementTypeConfiguration : EntityTypeConfiguration<Advertisement>
    {
        public AdvertisementTypeConfiguration()
        {
            HasMany(advertisement => advertisement.UserPoints).WithRequired(pt => pt.Advertisement);
        }
    }
}
