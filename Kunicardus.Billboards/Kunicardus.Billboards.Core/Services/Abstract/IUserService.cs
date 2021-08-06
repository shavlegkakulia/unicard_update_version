using System;
using Kunicardus.Billboards.Core.Models;
using System.Threading.Tasks;
using Kunicardus.Billboards.Core.DbModels;

namespace Kunicardus.Billboards.Core.Services.Abstract
{
	public interface IUserService
    {
        BaseActionResult<UserInfo> GetUserInfoByUserId(string userId);

		BaseActionResult<UserBalanceModel> GetUserBalance (string userId);

		string SaveLoggedUserInfo (string userId, string username, string fbID);

        BaseActionResult<UserInfo> GetUserInfo();
	}
}

