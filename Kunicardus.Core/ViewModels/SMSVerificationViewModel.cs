using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using Kunicardus.Core.ViewModels;
using Kunicardus.Core.Plugins.UIDialogPlugin;
using Kunicardus.Core.Providers.LocalDBProvider;
using Kunicardus.Core.Models.DB;
using Kunicardus.Core.UnicardApiProvider;
using Kunicardus.Core.Models;
using Kunicardus.Core.Services.Abstract;
using MvvmCross;
using System.Threading.Tasks;
using Kunicardus.Core.Models.DataTransferObjects;
using MvvmCross.Platform;

namespace Kunicardus.Core
{
	public class SMSVerificationViewModel : BaseViewModel
	{
		ISmsVerifycationService _verifySMSCodeService;
		ILocalDbProvider _dbProvider;
		IUserService _userService;
		IUIDialogPlugin _dialogPlugin;
		ISmsVerifycationService _smsVerificationService;
		IAuthService _authService;
		ICustomSecurityProvider _securityProvider;

		public SMSVerificationViewModel (IUserService userService, 
		                                 IAuthService authService,
		                                 ISmsVerifycationService verifySMSCodeService, 
		                                 ILocalDbProvider dbProvider, 
		                                 IUIDialogPlugin dialogPlugin,
		                                 ISmsVerifycationService smsVerificationViewModel,
		                                 ICustomSecurityProvider securityProvider)
		{
			_authService = authService;
			_verifySMSCodeService = verifySMSCodeService;
			_dbProvider = dbProvider;
			_userService = userService; 
			_dialogPlugin = dialogPlugin;
			_smsVerificationService = smsVerificationViewModel;
			_securityProvider = securityProvider;
		}

		public void Init (string phoneNumber)
		{
			_phoneNumber = phoneNumber;
		}

		public bool Mask {
			get;
			set;
		}

		private string _phoneNumber;
		private bool smsSentSuccessfully;

		public string PhoneNumber {
			get { return _phoneNumber; }
			set {
				_phoneNumber = value;
				if (Mask)
					_phoneNumber = _phoneNumber.Substring (3);
				var otpResult = _smsVerificationService.SendOTP ("", _unicardNumber, _phoneNumber);
				if (otpResult.Success)
					smsSentSuccessfully = true;
				else {
					_uiThread.InvokeUIThread (() => {
						_dialog.ShowToast (otpResult.DisplayMessage);
					});
				}
				RaisePropertyChanged (() => PhoneNumber);
			}
		}

		private string _verificationCode;

		public string VerificationCode {
			get { return _verificationCode; }
			set {
				_verificationCode = value;
				RaisePropertyChanged (() => VerificationCode);
			}
		}

		private ICommand _continue;

		public ICommand ContinueCommand {
			get {
				_continue = _continue ?? new MvxCommand (async () => {
					ShouldValidateModel = true;
					VerifyCode ();
				});
				return _continue;
			}
		}

		public ICommand Resend {
			get {
				return new MvxCommand (ResendCode);
			}
		}

		private async void ResendCode ()
		{
			//var user = _dbProvider.Get<UserInfo>().First();
			_dialogPlugin.ShowProgressDialog ("გთხოვთ დაელოდოთ...");
			await Task.Run (() => {
				var response = _verifySMSCodeService.SendOTP ("", _unicardNumber, _phoneNumber);
				if (response != null) {				
					InvokeOnMainThread (() => {
						if (!string.IsNullOrEmpty (response.DisplayMessage)) {
							_dialog.ShowToast (response.DisplayMessage);
						}
					});
				}
				InvokeOnMainThread (() => _dialogPlugin.DismissProgressDialog ());
			});
		}

		private TransferUserModel _currentUser;

		public TransferUserModel CurrentUSer {
			get{ return _currentUser; }
			set { _currentUser = value; }
		}

		private bool _emailRegistrationIsInProgress;

		public bool EmailRegistrationIsInProgress {
			get { return _emailRegistrationIsInProgress; }
			set{ _emailRegistrationIsInProgress = value; }
		}

		private string _unicardNumber;

		public string UnicardNumber {
			get{ return _unicardNumber; }
			set { _unicardNumber = value; }
		}

		private bool _registrationCompleted;

		public bool RegisrationCompleted {
			get{ return _registrationCompleted; }
			set {
				_registrationCompleted = value; 
				RaisePropertyChanged (() => RegisrationCompleted);
			}
		}

		public bool FBRegistrationIsInProgress{ get; set; }

		public bool NewCardRegistration { get; set; }

		#region Register Methods

		void Register ()
		{
			BaseActionResult<RegisterUserModel> registerStatus;

			string newCardRegistration = "0";
			if (NewCardRegistration)
				newCardRegistration = "1";
			registerStatus = _userService.RegisterUser ("",
				_currentUser.Email,
				_currentUser.Password,
				_currentUser.Name,
				_currentUser.Surname,
				_currentUser.PersonalId,
				_currentUser.DateOfBirth,
				_currentUser.PhoneNumber,
				_currentUser.CardNumber,
				newCardRegistration,
				null);
			

			if (registerStatus != null) {
				
				if (registerStatus.Success) {
					RegisrationCompleted = true;
					//TODO: remove
//					ShowViewModel<LoginAuthViewModel> ();
				} else if (!string.IsNullOrWhiteSpace (registerStatus.DisplayMessage)) {
					_uiThread.InvokeUIThread (() => {
						_dialog.ShowToast (registerStatus.DisplayMessage);
					});
				}
			}
		
		}

		public void RegistrationCompletedClick ()
		{
			ShowViewModel<LoginAuthViewModel> ();
		}

		public void FacebookRegister ()
		{
			var newCard = "0";
			if (NewCardRegistration) {
				newCard = "1";
				_unicardNumber = "";
			}
			
			BaseActionResult<RegisterUserModel> registerStatus;
			registerStatus = _userService.RegisterUser (
				_currentUser.FBId,
				_currentUser.Email,
				"",
				_currentUser.Name,
				_currentUser.Surname,
				_currentUser.PersonalId,
				_currentUser.DateOfBirth,
				_currentUser.PhoneNumber,
				_unicardNumber,
				newCard,
				null);
			if (registerStatus.Success) {
				var response = _authService.Auth (_currentUser.Email, null, _currentUser.FBId);
				if (response != null) {					
					if (response.Success) {
						_userId = response.Result.UserId;
						_securityProvider.SaveCredentials (_userId, _currentUser.Email, null, response.Result.SessionId, _currentUser.FBId);

						SaveLoggedInUserInfoForFB ();
						ShowViewModel<MainViewModel> ();
					}
				}
			}

			_uiThread.InvokeUIThread (() => {
				if (!string.IsNullOrEmpty (registerStatus.DisplayMessage)) {
					_dialog.ShowToast (registerStatus.DisplayMessage);
				}
			});
		}

		private bool _otpWasCorrect;

		public bool OtpWasCorrect {
			get { return _otpWasCorrect; }
			set {
				_otpWasCorrect = value; 
				RaisePropertyChanged (() => OtpWasCorrect);
			}
		}

		#endregion

		private string _userId;

		private void SaveLoggedInUserInfoForFB ()
		{
			var users = _dbProvider.Get<UserInfo> ();
			foreach (var item in users) {
				_dbProvider.Delete<UserInfo> (item);
			}

			UserInfo newUser = new UserInfo () { UserId = _userId, Username = _currentUser.Email };

			var userFromUnicard = _userService.GetUserInfoByUserId (_userId);
			if (userFromUnicard != null && userFromUnicard.Success) {
				newUser.FirstName = userFromUnicard.Result.FirstName;
				newUser.LastName = userFromUnicard.Result.LastName;
				newUser.Address = userFromUnicard.Result.Address;
				newUser.FullAddress = userFromUnicard.Result.FullAddress;
				string phone = userFromUnicard.Result.Phone;
				if (phone.Length >= 12) {
					phone = phone.Substring (3, phone.Length - 3);
				}
				newUser.Phone = phone;
				newUser.PersonalId = userFromUnicard.Result.PersonalNumber;
			}

			var balance = _userService.GetUserBalance (_userId);
			if (balance != null && balance.Success) {
				newUser.Balance_AccumulatedPoint = balance.Result.AccumulatedPoint;
				newUser.Balance_AvailablePoints = balance.Result.AvailablePoints;
				newUser.Balance_BlockedPoints = balance.Result.BlockedPoints;
				newUser.Balance_SpentPoints = balance.Result.SpentPoints;
			}

			var virtualCardNumber = _userService.GetVirtualCard (_userId);
			if (virtualCardNumber != null && virtualCardNumber.Success) {
				newUser.VirtualCardNumber = virtualCardNumber.Result.CardNumber;
			}
			//			newUser.IsAuthed = true;
			_dbProvider.Insert<UserInfo> (newUser);
		}

		private void VerifyCode ()
		{
			if (!string.IsNullOrWhiteSpace (_verificationCode)) {
				BaseActionResult<UnicardApiBaseResponse> result;
				_dialogPlugin.ShowProgressDialog (ApplicationStrings.SubmittingOTP);
				Task.Run (async () => {
					result = await _verifySMSCodeService.SubmitOTP (_verificationCode, "", "", _phoneNumber);
					InvokeOnMainThread (() => _dialogPlugin.DismissProgressDialog ());
					OtpWasCorrect = result.Success;
					if (result.Success) {
						if (!string.IsNullOrWhiteSpace (result.DisplayMessage))
							InvokeOnMainThread (() => _dialog.ShowToast (result.DisplayMessage));
						if (_emailRegistrationIsInProgress) {
							Mvx.IocConstruct<EmailRegistrationViewModel> ();
						} else if (FBRegistrationIsInProgress) {
							InvokeOnMainThread (() => _dialogPlugin.ShowProgressDialog (ApplicationStrings.Registering));
							FacebookRegister ();
							InvokeOnMainThread (() => _dialogPlugin.DismissProgressDialog ());
						} else {
							InvokeOnMainThread (() => _dialogPlugin.ShowProgressDialog (ApplicationStrings.Registering));
							Register ();
							InvokeOnMainThread (() => _dialogPlugin.DismissProgressDialog ());
						}

					} else
						InvokeOnMainThread (() => {
							_dialog.ShowToast ("შეყვანილი კოდი არასწორია");
						});
				});
			} else
				InvokeOnMainThread (() => {
					_dialog.ShowToast (ApplicationStrings.VerificationCodeIsNull);
				});
		}
	}
}

