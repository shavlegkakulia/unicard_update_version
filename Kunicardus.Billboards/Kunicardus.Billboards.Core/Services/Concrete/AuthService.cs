using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

using Kunicardus.Billboards.Core.Helpers;
using Kunicardus.Billboards.Core.Models;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using Kunicardus.Billboards.Core.Models.DataTransferObjects;
using Kunicardus.Billboards.Core.Services.Abstract;
using Kunicardus.Billboards.Core.DbModels;

namespace Kunicardus.Billboards.Core.Services
{
	public class AuthService : IAuthService
	{
        IUnicardApiProvider _apiProvider;
        //BillboardsDb _db;
        public AuthService(IUnicardApiProvider unicardApiProvider)
		{
            _apiProvider = unicardApiProvider; 
            //_db = new BillboardsDb(BillboardsDb.path);
        }

        public BaseActionResult<UserModel> Auth(string username, string password, string facebookId)
        {
            BaseActionResult<UserModel> result = new BaseActionResult<UserModel>();
            if (string.IsNullOrWhiteSpace(password))
                password = "";
            if (string.IsNullOrWhiteSpace(username))
                username = "";
            var authRequest = new AuthRequest
            {
                UserName = username,
                Password = password,
                FacebookId = facebookId
            };

            var url = string.Format("{0}UserAuthorization", Constants.UnicardServiceUrl);
            var json = JsonConvert.SerializeObject(authRequest,
                           Formatting.None,
                           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
            var response = _apiProvider.Post<AuthResponse>(url, null, json);
            result.Success = response.Successful;
            result.DisplayMessage = response.DisplayMessage;
            result.Result = new UserModel();
            result.Result.UserId = response.UserId;
            result.Result.SessionId = response.SessionID;

            return result;
        }
	}
}

