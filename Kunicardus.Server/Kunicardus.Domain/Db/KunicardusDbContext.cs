using Kunicardus.Domain.Db.EntityTypeConfigurations;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Domain.Db
{
    public class KunicardusDbContext : IdentityDbContext<User>
    {
        public KunicardusDbContext()
            : this(nameOrConnectionString: "KunicardusDbContext")
        {

        }

        public KunicardusDbContext(string nameOrConnectionString = "KunicardusDbContext")
            : base(nameOrConnectionString)
        {
            this.Database.Log = log => Debug.WriteLine(log);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new BillboardTypeConfiguration());
            modelBuilder.Entity<Advertisement>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
