using System;
using System.Threading.Tasks;
using Kunicardus.Billboards.Core.Models;

namespace Kunicardus.Billboards.Core.Services.Abstract
{
	public interface IAuthService
	{
        BaseActionResult<UserModel> Auth(string username, string password, string facebookId);
        //UserModel UpdateSession();
	}
}

