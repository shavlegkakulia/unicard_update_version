using System;
using Kunicardus.Core.Models;
using System.Threading.Tasks;

namespace Kunicardus.Core.Services.Abstract
{
	public interface IAuthService
	{
		BaseActionResult<UserModel> Auth (string username, string password, string facebookId);

		UserModel UpdateSession ();
	}
}

