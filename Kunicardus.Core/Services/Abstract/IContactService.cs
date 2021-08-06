using Kunicardus.Core.Models;
using Kunicardus.Core.Models.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunicardus.Core.Services.Abstract
{
    public interface IContactService
    {
        Task<BaseActionResult<ContactInfoModel>> GetContactInfo();
    }
}
