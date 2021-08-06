using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kunicardus.Billboards.Core.Helpers;
using Kunicardus.Billboards.Core.Services.Abstract;

namespace Kunicardus.Billboards.Core.ViewModels
{
	public class AuthViewModel
	{
		IAuthService _authService;
		ICustomSecurityProvider _securityProvider;
		IUserService _userService;

		public string DisplayMessage { get; set; }

		public string ErrorText { get; set; }

		public AuthViewModel (IAuthService authService, ICustomSecurityProvider securityProvider, IUserService userService)
		{
			_authService = authService;
			_securityProvider = securityProvider;
			_userService = userService;
		}

		public bool Auth (string username, string password)
		{
			if (string.IsNullOrWhiteSpace (username) || string.IsNullOrWhiteSpace (password)) {
				return false;
			}

			var response = _authService.Auth (username, password, null);

            if (response.Success)
            {

                var user = username;
                var _userId = response.Result.UserId;

                _securityProvider.SaveCredentials(_userId, user, password, response.Result.SessionId);

                var infoSaved = _userService.SaveLoggedUserInfo(_userId, user, null);
                if (string.IsNullOrEmpty(infoSaved))
                {
                    return true;
                }
            }
			DisplayMessage = response.DisplayMessage;
			return false;
		}
	}
}