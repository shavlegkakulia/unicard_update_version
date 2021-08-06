using System;
using Kunicardus.Core.Models;
using Kunicardus.Core.Models.DataTransferObjects;
using Kunicardus.Core.UnicardApiProvider;
using System.Threading.Tasks;

namespace Kunicardus.Core
{
	public interface ISmsVerifycationService
	{
		BaseActionResult<UnicardApiBaseResponse> SendOTP (string userId, string cardNumber, string phoneNumber, string username = "");

		Task<BaseActionResult<UnicardApiBaseResponse>> SubmitOTP (string otp, string userId, string cardNumber, string phoneNumber);
	}
}

