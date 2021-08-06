using Kuni.Core.Models;
using Kuni.Core.Models.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuni.Core.Services.Abstract
{
    public interface IContactService
    {
        Task<BaseActionResult<ContactInfoModel>> GetContactInfo();
    }
}
