using System;
using Kunicardus.Core.Services.Abstract;
using Kunicardus.Core.UnicardApiProvider;
using Kunicardus.Core.Helpers.AppSettings;
using Kunicardus.Core.Models;
using Kunicardus.Core.Models.DataTransferObjects;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using MvvmCross;
using System.Linq;
using System.Reflection;

namespace Kunicardus.Core.Services.Concrete
{
	public class UserService : IUserService
	{
		private IUnicardApiProvider _apiProvider;
		private IAppSettings _appSettings;

		public UserService (
			IUnicardApiProvider unicardApiProvider,
			IAppSettings appSettings
		)
		{
			_apiProvider = unicardApiProvider;
			_appSettings = appSettings;
		}

		public BaseActionResult<ResetPasswordResponse> ResetPassword (string userName, string smsCode, string newPassword)
		{
			BaseActionResult<ResetPasswordResponse> result = new BaseActionResult<ResetPasswordResponse> ();
			var url = string.Format ("{0}ResetPassword", _appSettings.UnicardServiceUrl);
			var request = new ResetPasswordRequest () { 
				UserName = userName,
				SmsCode = smsCode,
				NewPassword = newPassword
			};
			var json = JsonConvert.SerializeObject (request, 
				           Formatting.None, 
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			var response = _apiProvider.Post<ResetPasswordResponse> (url, null, json).Result; 
			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;
			return result;
		}

		public async Task<BaseActionResult<ChangePasswordModel>> ChangePassword (string oldPassword, string newPassword, string userId)
		{
			BaseActionResult<ChangePasswordModel> result = new BaseActionResult<ChangePasswordModel> ();
			var url = string.Format ("{0}ChangePassword", _appSettings.UnicardServiceUrl);
			var request = new ChangePasswordRequest () {
				NewPassword = newPassword,
				OldPassword = oldPassword,
				UserId = userId
			};
			var json = JsonConvert.SerializeObject (request, 
				           Formatting.None, 
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			var response = _apiProvider.Post<ChangePasswordResponse> (url, null, json).Result;
			result.DisplayMessage = response.DisplayMessage;
			result.Success = response.Successful;

			return result;
		}


		public async Task<BaseActionResult<GetUserTypesModel>> GetUserTypeList ()
		{

			BaseActionResult<GetUserTypesModel> result = new BaseActionResult<GetUserTypesModel> ();
			var url = string.Format ("{0}GetCustomerTypeList", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (new UnicardApiBaseRequest (), 
				           Formatting.None, 
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			var response = _apiProvider.Post<GetUserTypesResponse> (url, null, json).Result; 
			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;
			result.Result = new GetUserTypesModel ();
			if (response.UserTypes != null) {
				result.Result.UserTypes =
					response.UserTypes.Select (x => new UserTypeModel () {
					UserTypeID = x.UserTypeID,
					UserTypeName = x.UserTypeName
				}).ToList ();
			}
			return result;
		}

		public BaseActionResult<UserPhoneModel> GetUserByPhone (string userName)
		{
			BaseActionResult<UserPhoneModel> result = new BaseActionResult<UserPhoneModel> ();
			var url = string.Format ("{0}GetUserPhone", _appSettings.UnicardServiceUrl);
			var request = new GetUserByPhoneRequest () { 
				UserName = userName
			};
			var json = JsonConvert.SerializeObject (request, 
				           Formatting.None, 
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			var response = _apiProvider.Post<GetUserByPhoneResponse> (url, null, json).Result; 
			result.DisplayMessage = response.DisplayMessage;
			result.Success = response.Successful;
			result.Result = new UserPhoneModel ();
			result.Result.UserPhone = response.UserPhoneNumber;
			result.Result.UserId = response.UserId;
			return result;
		}

		#region IUserService implementation

		public BaseActionResult<VirtualCardModel> GetVirtualCard (string userId)
		{
			BaseActionResult<VirtualCardModel> result = new BaseActionResult<VirtualCardModel> ();

			var request = new GetVirtualCardRequest { 
				UserId = userId
			};
			var url = string.Format ("{0}GetVirtualCard", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (request, 
				           Formatting.None, 
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			var response = _apiProvider.Post<GetVirtualCardResponse> (url, null, json).Result; 
			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;
			result.Result = new VirtualCardModel ();
			result.Result.CardNumber = response.CardNumber;

			return result;
		}


		public async Task<BaseActionResult<UserExistsModel>> UserExists (string userName)
		{
			BaseActionResult<UserExistsModel> result = new BaseActionResult<UserExistsModel> ();

			var request = new UserExistsRequest { 
				UserName = userName
			};
			var url = string.Format ("{0}UserNameExists", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (request, 
				           Formatting.None, 
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			var response = _apiProvider.Post<UserExistsResponse> (url, null, json).Result; 
			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;
			result.Result = new UserExistsModel ();
			result.Success = response.Successful;
			result.Result.Exists = response.Exists;

			return result;
		}

		public BaseActionResult<CardStatusModel> GetCardStatus (string cardNumber)
		{
			BaseActionResult<CardStatusModel> result = new BaseActionResult<CardStatusModel> ();

			var request = new GetCardStatusRequest { 
				CardNumber = cardNumber
			};
			var url = string.Format ("{0}GetCardStatus", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (request, 
				           Formatting.None, 
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			var response = _apiProvider.Post<GetCardStatusResponse> (url, null, json).Result; 
			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;
			result.Result = new CardStatusModel ();
			result.Result.CardIsValid = response.CardIsValid;
			result.Result.ExistsUserData = response.ExistsUserData;
			result.Result.HasTransactions = response.HasTransactions;

			return result;
		}

		public BaseActionResult<RegisterUserModel> RegisterUser (string fbId,
		                                                         string email,
		                                                         string password,
		                                                         string name,
		                                                         string surname,
		                                                         string idNumber,
		                                                         DateTime? dateOfBirth,
		                                                         string phoneNumber,
		                                                         string cardNumber,
		                                                         string newCardRegistration,
		                                                         string SMSCode)
		{
			BaseActionResult<RegisterUserModel> result = new BaseActionResult<RegisterUserModel> ();
			var request = new RegisterUserRequest () { 
				FbId = fbId,
				UserName = email,
				Email = email,
				Password = password,
				Name = name,
				Surname = surname,
				UserIdNumber = idNumber,
				DateOfBirth = dateOfBirth,
				PhoneNumber = phoneNumber,
				UnicardNumber = cardNumber,
				NewCardRegistration = newCardRegistration
			};
			var url = string.Format ("{0}UserRegistration", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (request, 
				           Formatting.None, 
				           new JsonSerializerSettings {
					NullValueHandling = NullValueHandling.Include,
					DateFormatString = Constants.ApiDateTimeFormat,
				});
			var response = _apiProvider.Post<RegisterUserResponse> (url, null, json).Result; 
			result.DisplayMessage = response.DisplayMessage;
			result.Result = new RegisterUserModel ();
			result.Result.Result = response.Result;
			result.Result.SessionId = response.SessionId;
			result.Result.UserId = response.UserId;
			result.Success = response.Successful;
			return result;
		}

		public  BaseActionResult<UserInfoModel> GetUserInfoByUserId (string userId)
		{
			BaseActionResult<UserInfoModel> result = new BaseActionResult<UserInfoModel> ();

			var request = new GetUserInfoRequest { 
				UserId = userId
			};

			var url = string.Format ("{0}GetClientInfo", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (request, 
				           Formatting.None, 
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			var response = _apiProvider.Post<GetUserInfoResponse> (url, null, json).Result; 

			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;
			result.Result = new UserInfoModel () { 
				FirstName = response.FirstName,
				LastName = response.LastName,
				AdditionalEmail = response.AdditionalEmail,
				Phone = response.Phone,
				Address = response.Address,
				FullAddress = response.FullAddress,
				PersonalNumber = response.PersonalId
			};
			return result;
		}

		public BaseActionResult<UserInfoByCardModel> GetUserInfoByCard (string cardNumber)
		{
			BaseActionResult<UserInfoByCardModel> result = new BaseActionResult<UserInfoByCardModel> ();

			var request = new GetUserInfoByCardRequest { 
				Card = cardNumber
			};
			var url = string.Format ("{0}GetClientInfoByCard", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (request, 
				           Formatting.None, 
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			var response = _apiProvider.Post<GetUserInfoByCardResponse> (url, null, json).Result; 
			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;
			result.Result = new UserInfoByCardModel () { 
				Name = response.Name,
				Surname = response.Surname,
				UserIdNumber = response.UserIdNumber,
				PhoneNumber = response.PhoneNumber,
				Result = response.Result
			};
			return result;
		}

		public BaseActionResult<UserBalanceModel> GetUserBalance (string userID)
		{
			BaseActionResult<UserBalanceModel> result = new BaseActionResult<UserBalanceModel> ();

			var request = new GetUserBalanceRequest {
				UserId = userID
			};
			var url = string.Format ("{0}GetUserBalance", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

			var response = _apiProvider.Post<GetUserBalanceResponse> (url, null, json).Result;

			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;
			result.Result = new UserBalanceModel ();
			result.Result.AccumulatedPoint = response.AccumulatedPoint;
			result.Result.AvailablePoints = response.AvailablePoints;
			result.Result.BlockedPoints = response.BlockedPoints;
			result.Result.SpentPoints = response.SpentPoints;

			return result;
		}

		#endregion


		
	}
}

