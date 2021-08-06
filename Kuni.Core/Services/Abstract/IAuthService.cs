using System;
using Kuni.Core.Models;
using System.Threading.Tasks;

namespace Kuni.Core.Services.Abstract
{
	public interface IAuthService
	{
		BaseActionResult<UserModel> Auth (string username, string password, string facebookId);

		UserModel UpdateSession ();
	}
}

