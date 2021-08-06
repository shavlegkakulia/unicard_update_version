using Kunicardus.Billboards.Core.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uniboard.UnitTests.Fakes
{
    public class FakeUserService : IUserService
    {
        public Kunicardus.Billboards.Core.Models.BaseActionResult<Kunicardus.Billboards.Core.DbModels.UserInfo> GetUserInfoByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public Kunicardus.Billboards.Core.Models.BaseActionResult<Kunicardus.Billboards.Core.Models.UserBalanceModel> GetUserBalance(string userId)
        {
            throw new NotImplementedException();
        }

        public string SaveLoggedUserInfo(string userId, string username, string fbID)
        {
            return "";
        }

        public Kunicardus.Billboards.Core.Models.BaseActionResult<Kunicardus.Billboards.Core.DbModels.UserInfo> GetUserInfo()
        {
            throw new NotImplementedException();
        }
    }
}
