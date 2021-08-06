using System;
using Kuni.Core.Models;
using System.Threading.Tasks;
using Kuni.Core.Models.BusinessModels;

namespace Kuni.Core.Services.Abstract
{
	public interface IUserService
	{
		BaseActionResult<ResetPasswordResponse> ResetPassword (string userName, string smsCode, string newPassword);

		BaseActionResult<VirtualCardModel> GetVirtualCard (string userId);

		BaseActionResult<CardStatusModel> GetCardStatus (string cardNumber);

		BaseActionResult<RegisterUserModel> RegisterUser (string fbId,
		                                                  string email,
		                                                  string password,
		                                                  string name,
		                                                  string surname,
		                                                  string idNumber,
		                                                  DateTime? dateOfBirth,
		                                                  string phoneNumber,
		                                                  string cardNumber,
		                                                  string newCardRegistration,
		                                                  string SMSCode
		);

		BaseActionResult<UserInfoByCardModel> GetUserInfoByCard (string cardNumber);

		BaseActionResult<UserInfoModel> GetUserInfoByUserId (string userId);

		BaseActionResult<UserBalanceModel> GetUserBalance (string userId);

		Task<BaseActionResult<UserExistsModel>> UserExists (string userName);

		Task<BaseActionResult<GetUserTypesModel>> GetUserTypeList ();

		BaseActionResult<UserPhoneModel> GetUserByPhone (string userName);

		Task<BaseActionResult<ChangePasswordModel>> ChangePassword (string oldPassword, string newPassword, string userId);

	}

}

