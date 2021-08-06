using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kunicardus.Billboards.Core.Services.Abstract;
using Kunicardus.Billboards.Core.Helpers;

namespace Kunicardus.Billboards.Core.ViewModels
{
	public class LoginViewModel
	{
		IAuthService _authService;
		ICustomSecurityProvider _securityProvider;
		IUserService _userService;

		public LoginViewModel (IAuthService authService, ICustomSecurityProvider securityProvider, IUserService userService)
		{
			_authService = authService;
			_securityProvider = securityProvider;
			_userService = userService;
		}

		public FacebookResult FacebookConnect (string name, string surname, string email, string fbId)
		{
			FacebookResult result = new FacebookResult ();
			var response = _authService.Auth (email, null, fbId);
			if (response != null) {
				if (response.Success) {
					
					var username = email;
					var _userId = response.Result.UserId;

					_securityProvider.SaveCredentials (_userId, username, fbId, response.Result.SessionId, fbId);
					var infoSaved = _userService.SaveLoggedUserInfo (response.Result.UserId, username, fbId);
					result.Success = true;
					return result;
				} 
				result.DisplayMessage = response.DisplayMessage;
			}
			result.Success = false;
			return result;
		}

		public class FacebookResult
		{
			public bool Success {
				get;
				set;
			}

			public string DisplayMessage {
				get;
				set;
			}
		}
	}
}