using System;
using Kunicardus.Billboards.Core.Services.Abstract;
using Kunicardus.Billboards.Core.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Reflection;
using Kunicardus.Billboards.Core.DbModels;
using Kunicardus.Billboards.Core.Models.DataTransferObjects;
using Kunicardus.Billboards.Core.Helpers;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using Kunicardus.Billboards.Core.Models.DTOs;

namespace Kunicardus.Billboards.Core.Services.Concrete
{
	public class UserService : IUserService
	{
        //BillboardsDb _db;
		IUnicardApiProvider _apiProvider;

		public UserService (IUnicardApiProvider unicardApiProvider)
		{
			_apiProvider = unicardApiProvider;
            //_db = new BillboardsDb (BillboardsDb.path);
		}

		public BaseActionResult<UserInfo> GetUserInfoByUserId (string userId)
		{
			BaseActionResult<UserInfo> result = new BaseActionResult<UserInfo> ();

			var request = new GetUserInfoRequest {
				UserId = userId
			};

			var url = string.Format ("{0}GetClientInfo", Constants.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (request,
				                    Formatting.None,
				                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			var response = _apiProvider.Post<GetUserInfoResponse> (url, null, json);
			
			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;
            result.ResultCode = response.ResultCode;
			result.Result = new UserInfo () {
				FirstName = response.FirstName,
				LastName = response.LastName,
				//AdditionalEmail = response.AdditionalEmail,
				Phone = response.Phone,
				Address = response.Address,
				FullAddress = response.FullAddress,
				PersonalId = response.PersonalId
			};
			return result;
		}

		public BaseActionResult<UserBalanceModel> GetUserBalance (string userID)
		{
			BaseActionResult<UserBalanceModel> result = new BaseActionResult<UserBalanceModel> ();

			var request = new GetUserBalanceRequest {
				UserId = userID
			};
			var url = string.Format ("{0}GetUserBalance", Constants.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

			var response = _apiProvider.Post<GetUserBalanceResponse> (url, null, json);

			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;
			result.Result = new UserBalanceModel ();
			result.Result.AccumulatedPoint = response.AccumulatedPoint;
			result.Result.AvailablePoints = response.AvailablePoints;
			result.Result.BlockedPoints = response.BlockedPoints;
			result.Result.SpentPoints = response.SpentPoints;

			return result;
		}

		public string SaveLoggedUserInfo (string userId, string username, string fbID)
		{
            using (BillboardsDb _db = new BillboardsDb(BillboardsDb.path))
            {

                string errorText = "";
                try
                {
                    var users = _db.Table<UserInfo>();
                    foreach (var item in users)
                    {
                        _db.Delete(item);
                    }

                    UserInfo newUser = UpdateUserInfo(userId, username, fbID).Result;

                    if (newUser != null)
                    {
                        _db.InsertOrReplace(newUser);
                    }
                    //Log.Debug("User Insert Message: ", "Save Successfull");
                    //return true;
                }
                catch (Exception ex)
                {
                    errorText = ex.Message;
                }

                return errorText;
            }
		}

        public BaseActionResult<UserInfo> GetUserInfo()
		{
            using (BillboardsDb _db = new BillboardsDb(BillboardsDb.path))
            {
                string errorText = "";
                var user = _db.Table<UserInfo>().FirstOrDefault();

                BaseActionResult<UserInfo> newUser = UpdateUserInfo(user.UserId, user.Username, null);

                try
                {
                    _db.Delete(user);

                    if (newUser.Result != null)
                    {
                        _db.InsertOrReplace(newUser.Result);
                    }
                }
                catch (Exception ex)
                {
                    errorText = ex.Message;
                    //Log.Debug("GetUserInfo Message: ", ex.ToString());
                }
                return newUser;
            }
		}

        private BaseActionResult<UserInfo> UpdateUserInfo(string userId, string username, string fbID)
        {
            var response = GetUserInfoByUserId(userId);
            response.Result.UserId = userId;
            response.Result.Username = username;

            var balance = GetUserBalance(userId);
            if (balance != null && balance.Success)
            {
                response.Result.Balance_AvailablePoints = balance.Result.AvailablePoints;
            }
            return response;
        }
	}
}

