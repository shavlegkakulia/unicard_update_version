using System;
using System.Collections.Generic;
using System.Linq;
using Kuni.Core.UnicardApiProvider;
using Kuni.Core.Models.DataTransferObjects;
using Kuni.Core.Helpers.AppSettings;
using Newtonsoft.Json;
using Kuni.Core.Models;
using Kuni.Core.Services.Abstract;
using System.Threading.Tasks;
using MvvmCross;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Plugins.UIDialogPlugin;
using Kuni.Core.Helpers.Device;
using Kuni.Core.Plugins.Connectivity;
using MvvmCross.Views;
using MvvmCross.ViewModels;
//using MvvmCross;

namespace Kuni.Core
{
	public class AuthService : IAuthService
	{
		private IUnicardApiProvider _apiProvider;
		private IAppSettings _appSettings;
		private IDevice _device;

		public AuthService (
			IUnicardApiProvider unicardApiProvider,
			IAppSettings appSettings,
			IDevice device
		)
		{
			_apiProvider = unicardApiProvider;
			_appSettings = appSettings;
			_device = device;
		}

		public BaseActionResult<UserModel> Auth (string username, string password, string facebookId)
		{
			BaseActionResult<UserModel> result = new BaseActionResult<UserModel> ();
			if (string.IsNullOrWhiteSpace (password))
				password = "";
			if (string.IsNullOrWhiteSpace (username))
				username = "";
			var authRequest = new AuthRequest { 
				UserName = username,
				Password = password,
				FacebookId = facebookId
			};

			var url = string.Format ("{0}UserAuthorization", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (authRequest, 
				           Formatting.None,
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
            
			var response = _apiProvider.Post<AuthResponse> (url, json).Result; 

			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;
			result.Result = new UserModel ();
			result.Result.UserId = response.UserId;
			result.Result.SessionId = response.SessionID;

			return result;
		}

		public UserModel UpdateSession ()
		{
            var securityProvider = Mvx.IoCProvider.Resolve<ICustomSecurityProvider> ();
			var info = securityProvider.GetCredentials ();
			var authRequest = new AuthRequest { 
				UserName = info.UserName,
				Password = info.Password,
				FacebookId = info.fbToken
			};
			var url = string.Format ("{0}UserAuthorization", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (authRequest, 
				           Formatting.None, 
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
			var response = _apiProvider.Post<AuthResponse> (url, json).Result; 

			if (response.Successful && !string.IsNullOrEmpty (response.SessionID)) {
				securityProvider.SaveCredentials (response.UserId, authRequest.UserName, authRequest.Password, response.SessionID, 
					(string.IsNullOrWhiteSpace (authRequest.FacebookId) ? null : authRequest.FacebookId));
				var data = new UserModel ();
				data.UserId = response.UserId;
				data.SessionId = response.SessionID;
				return data;
			} else {				
				new BaseViewModel ().Logout (true);
			}

			return null;
		}
	}
}

