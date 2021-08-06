using System;
using Kuni.Core.Models;
using Kuni.Core.Models.DataTransferObjects;
using Kuni.Core.UnicardApiProvider;
using System.Threading.Tasks;

namespace Kuni.Core
{
	public interface ISmsVerifycationService
	{
		BaseActionResult<UnicardApiBaseResponse> SendOTP (string userId, string cardNumber, string phoneNumber, string username = "");

		Task<BaseActionResult<UnicardApiBaseResponse>> SubmitOTP (string otp, string userId, string cardNumber, string phoneNumber);
	}
}

