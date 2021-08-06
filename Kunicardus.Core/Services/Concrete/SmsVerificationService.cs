using Kunicardus.Core.Helpers.AppSettings;
using Kunicardus.Core.Models;
using Kunicardus.Core.Models.DataTransferObjects;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Kunicardus.Core
{
	public class SmsVerifycationService : ISmsVerifycationService
	{
		private IUnicardApiProvider _apiProvider;
		private IAppSettings _appSettings;

		public SmsVerifycationService (IUnicardApiProvider unicardApiProvider, IAppSettings appSettings)
		{
			_apiProvider = unicardApiProvider;
			_appSettings = appSettings;
		}

		public async Task<BaseActionResult<UnicardApiBaseResponse>> SubmitOTP (string otp, string userId, string cardNumber, string phoneNumber)
		{
			BaseActionResult<UnicardApiBaseResponse> result = new BaseActionResult<UnicardApiBaseResponse> ();

			var request = new SubmitOTPRequest {
				UserId = userId,
				Card = cardNumber,
				OTP = otp,
				PhoneNumber = String.Format ("{0}{1}", Constants.GeorgiaMobIndex, phoneNumber)
			};

			var url = string.Format ("{0}SubmitOTP", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

			var response = await _apiProvider.Post<UnicardApiBaseResponse> (url, null, json);

			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;

			return result;
		}

		public BaseActionResult<UnicardApiBaseResponse> SendOTP (string userId, string cardNumber, string phoneNumber, string username = "")
		{
			BaseActionResult<UnicardApiBaseResponse> result = new BaseActionResult<UnicardApiBaseResponse> ();

			var request = new SendOTPRequest {
				UserId = userId,
				Card = cardNumber,
				PhoneNumber = String.Format ("{0}{1}", Constants.GeorgiaMobIndex, phoneNumber),
				UserName = username
			};
			var url = string.Format ("{0}SendOTP", _appSettings.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

			var response = _apiProvider.Post<UnicardApiBaseResponse> (url, null, json).Result;

			result.Success = response.Successful;
			result.DisplayMessage = response.DisplayMessage;

			return result;
		}
	}
}

