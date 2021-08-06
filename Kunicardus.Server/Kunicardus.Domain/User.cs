using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Domain
{
    public class User : IdentityUser
    {
        public virtual ICollection<Advertisement> Advertisements { get; set; }
    }
}
