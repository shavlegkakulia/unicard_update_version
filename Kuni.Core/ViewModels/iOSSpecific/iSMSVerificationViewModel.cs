using System;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Services.Abstract;
using Kuni.Core.Plugins.UIDialogPlugin;
using System.Windows.Input;
using MvvmCross.ViewModels;
using Kuni.Core.Models.DataTransferObjects;
using System.Threading.Tasks;
using Kuni.Core.Models;
using Kuni.Core.UnicardApiProvider;
using MvvmCross;
using Kuni.Core.Models.DB;
using Newtonsoft.Json;

namespace Kuni.Core.ViewModels.iOSSpecific
{
	public class iSMSVerificationViewModel : BaseViewModel
	{
		ISmsVerifycationService _verifySMSCodeService;
		ILocalDbProvider _dbProvider;
		IUserService _userService;
		IUIDialogPlugin _dialogPlugin;
		ISmsVerifycationService _smsVerificationService;
		private IAuthService _authService;

		public iSMSVerificationViewModel (IUserService userService,
		                                  IAuthService authService,
		                                  ISmsVerifycationService verifySMSCodeService, 
		                                  ILocalDbProvider dbProvider, 
		                                  IUIDialogPlugin dialogPlugin,
		                                  ISmsVerifycationService smsVerificationViewModel)
		{
			_authService = authService;
			_verifySMSCodeService = verifySMSCodeService;
			_dbProvider = dbProvider;
			_userService = userService; 
			_dialogPlugin = dialogPlugin;
			_smsVerificationService = smsVerificationViewModel;

		}

		public void Init (iSMSVerificationParams param)
		{
			
			_currentUser = new TransferUserModel ();
			_currentUser.Name = param.Name;
			_currentUser.Surname = param.Surname;
			_currentUser.PersonalId = param.PersonalId;
			_currentUser.DateOfBirth = param.DateOfBirth;
			_currentUser.Email = param.Email;
			_currentUser.PhoneNumber = param.PhoneNumber;
			_currentUser.Password = param.Password;
			_currentUser.CardNumber = param.UnicardNumber;
			NewCardRegistration = param.NewCardRegistration;
			PhoneNumberRetrieved = param.PhoneNumberRetrieved;

			FBRegistrationIsInProgress = param.FacebookRegistration;
			if (!string.IsNullOrWhiteSpace (param.FBUser)) {
				_newFbUser = JsonConvert.DeserializeObject<TransferUserModel> (param.FBUser);
			}
			ResetPassword = param.ResetPassword;
			EmailRegistration = param.EmailRegistration;
			UnicardNumber = param.UnicardNumber;
			PhoneNumber = param.PhoneNumber;
		}

		#region Properties

		public bool PhoneNumberRetrieved {
			get;
			set;
		}

		private string _phoneNumber;

		public string PhoneNumber {
			get { return _phoneNumber; }
			set {
				_phoneNumber = value;
				if (!string.IsNullOrWhiteSpace (_phoneNumber) && _phoneNumber.Length > 9 && _phoneNumber.Contains ("995")) {
					_phoneNumber = _phoneNumber.Substring (3);
					_phoneNumber = _phoneNumber.Replace ("+", "");
				}								
				string userName = "";
				if (_resetPassword && _currentUser != null && !string.IsNullOrWhiteSpace (_currentUser.Email)) {
					userName = _currentUser.Email;
				}
				var otpResult = _smsVerificationService.SendOTP ("", _unicardNumber, _phoneNumber, userName);
				if (!otpResult.Success) {
					_uiThread.InvokeUIThread (() => {
						_dialog.ShowToast (otpResult.DisplayMessage);
					});
				}
				RaisePropertyChanged (() => PhoneNumber);
				RaisePropertyChanged (() => PhoneNumberFormated);
			}
		}

		public string PhoneNumberFormated {
			get { 
				if (PhoneNumberRetrieved)
					return _phoneNumber.Substring (0, 3) + "xxxx" + _phoneNumber.Substring (7, 2);
				else
					return _phoneNumber;
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
				_continue = _continue ?? new MvvmCross.Commands.MvxCommand (async () => {
					ShouldValidateModel = true;
					VerifyCode ();
				});
				return _continue;
			}
		}

		public ICommand Resend {
			get {
				return new MvvmCross.Commands.MvxCommand (ResendCode);
			}
		}

		private TransferUserModel _newFbUser;

		public TransferUserModel NewFBUser {
			get { return _newFbUser; }
			set{ _newFbUser = value; }
		}


		private TransferUserModel _currentUser;

		public TransferUserModel CurrentUSer {
			get{ return _currentUser; }
			set { _currentUser = value; }
		}

		private bool _resetPassword;

		public bool ResetPassword {
			get { return _resetPassword; }
			set{ _resetPassword = value; }
		}

		private bool _emailRegistration;

		public bool EmailRegistration {
			get { return _emailRegistration; }
			set{ _emailRegistration = value; }
		}

		private string _unicardNumber;

		public string UnicardNumber {
			get{ return _unicardNumber; }
			set { _unicardNumber = value; }
		}

		public bool FBRegistrationIsInProgress { get; set; }

		public bool NewCardRegistration { get; set; }

		#endregion

		#region Register Methods

		private string FormatPhoneNumber (string number)
		{
			if (!string.IsNullOrWhiteSpace (number) && number.Length == 9) {
				number = Constants.GeorgiaMobIndex + number;
			}
			return number;
		}

		private async void ResendCode ()
		{
			//var user = _dbProvider.Get<UserInfo>().First();
			InvokeOnMainThread (() => {
				_dialogPlugin.ShowProgressDialog ("Loading...");
			});
			await Task.Run (() => {
				string userName = "";
				if (_resetPassword && _currentUser != null && !string.IsNullOrWhiteSpace (_currentUser.Email)) {
					userName = _currentUser.Email;
				}
				var response = _verifySMSCodeService.SendOTP ("", _unicardNumber, _phoneNumber, userName);
				if (response != null) {				
					InvokeOnMainThread (() => {
						if (!string.IsNullOrEmpty (response.DisplayMessage)) {
							_dialog.ShowToast (response.DisplayMessage);
						}
					});
				}
				InvokeOnMainThread (() => {
					_dialogPlugin.DismissProgressDialog ();
				});
			});
		}

		private void DoResetPassword ()
		{
			var resetPasswordResponse = _userService.ResetPassword (_currentUser.Email, _verificationCode, _currentUser.Password);
			InvokeOnMainThread (() => {
				_dialog.DismissProgressDialog ();
			});
			if (resetPasswordResponse.Success) {
				NavigationCommand<LoginViewModel> (null, true);
				NavigationCommand<LoginAuthViewModel> ();
			} else {
				InvokeOnMainThread (() => {
					_dialog.ShowToast (resetPasswordResponse.DisplayMessage);
				});
			}
		}

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
				FormatPhoneNumber (_currentUser.PhoneNumber),
				_currentUser.CardNumber ?? "",
				newCardRegistration,
				null);

			InvokeOnMainThread (() => {
				_dialogPlugin.DismissProgressDialog ();
			});

			if (registerStatus != null) {
				if (!string.IsNullOrWhiteSpace (registerStatus.DisplayMessage)) {
					InvokeOnMainThread (() => {
						_dialog.ShowToast (registerStatus.DisplayMessage);
					});
				}
				if (registerStatus.Success) {
					NavigationCommand<LoginViewModel> (null, true);
					NavigationCommand<LoginAuthViewModel> ();
				}
			}

		}

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
			newUser.IsFacebookUser = true;
			_dbProvider.Insert<UserInfo> (newUser);
		}

		private void FacebookRegister ()
		{
			var newCard = "0";
			if (NewCardRegistration) {
				newCard = "1";
				_unicardNumber = "";
			}

			BaseActionResult<RegisterUserModel> registerStatus;
			registerStatus = _userService.RegisterUser (
				_newFbUser.FBId,
				_newFbUser.Email,
				"",
				_newFbUser.Name,
				_newFbUser.Surname,
				_newFbUser.PersonalId,
				_newFbUser.DateOfBirth,
				_newFbUser.PhoneNumber,
				_unicardNumber ?? "",
				newCard,
				null);
			if (registerStatus.Success) {
				var response = _authService.Auth (_newFbUser.Email, null, _newFbUser.FBId);
				InvokeOnMainThread (() => {
					_dialog.DismissProgressDialog ();
				});
				if (response != null) {					
					if (response.Success) {
						_userId = response.Result.UserId;
						SaveLoggedInUserInfoForFB ();
						NavigationCommand<RootViewModel> (null, true);
					} else {
						if (!string.IsNullOrEmpty (response.DisplayMessage)) {
							InvokeOnMainThread (() => {
								_dialog.ShowToast (registerStatus.DisplayMessage);
							});
						}
					}
				}
			} else {
				InvokeOnMainThread (() => {
					_dialog.DismissProgressDialog ();
				});
			}



			if (!string.IsNullOrEmpty (registerStatus.DisplayMessage)) {
				InvokeOnMainThread (() => {
					_dialog.ShowToast (registerStatus.DisplayMessage);
				});
			}
			
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

		private void VerifyCode ()
		{
			if (!string.IsNullOrWhiteSpace (_verificationCode)) {
				BaseActionResult<UnicardApiBaseResponse> result;
				Task.Run (async () => {
					result = null;
					if (_resetPassword) {	
						InvokeOnMainThread (() => {
							_dialogPlugin.ShowProgressDialog ("");
							DoResetPassword ();
						});
					} else {
						InvokeOnMainThread (() => {
							_dialogPlugin.ShowProgressDialog (ApplicationStrings.SubmittingOTP);
						});
						result = await _verifySMSCodeService.SubmitOTP (_verificationCode, "", "", _phoneNumber);
						InvokeOnMainThread (() => {
							_dialogPlugin.DismissProgressDialog ();
						});
					}
					if (result != null) {
						OtpWasCorrect = result.Success;
						if (result.Success) {
							if (!string.IsNullOrWhiteSpace (result.DisplayMessage))
								InvokeOnMainThread (() => {
									_dialog.ShowToast (result.DisplayMessage);
								});
							if (_emailRegistration) {	
								NavigationCommand<iEmailRegistrationViewModel> (new iEmailRegistrationViewModelParams () {
									UnicardNumber = _unicardNumber
								});
							} else if (FBRegistrationIsInProgress) {
								InvokeOnMainThread (() => {
									_dialogPlugin.ShowProgressDialog (ApplicationStrings.Registering);
								});
								FacebookRegister ();
							} else {
								InvokeOnMainThread (() => {
									_dialogPlugin.ShowProgressDialog (ApplicationStrings.Registering);
								});
								Register ();
							}
						} else
							InvokeOnMainThread (() => {
								_dialog.ShowToast ("შეყვანილი კოდი არასწორია");
							});
					}
				});
			} else
				InvokeOnMainThread (() => {
					_dialog.ShowToast (ApplicationStrings.VerificationCodeIsNull);
				});
		}
	}

	public class iSMSVerificationParams
	{
		public bool PhoneNumberRetrieved {
			get;
			set;
		}

		public bool ResetPassword {
			get;
			set;
		}

		public string UserId {
			get;
			set;
		}

		public string UnicardNumber {
			get;
			set;
		}

		public string PhoneNumber {
			get;
			set;
		}

		public bool EmailRegistration {
			get;
			set;
		}

		public string Email {
			get;
			set;
		}

		public string Password {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public string Surname {
			get;
			set;
		}

		public string PersonalId {
			get;
			set;
		}

		public DateTime DateOfBirth {
			get;
			set;
		}


		public bool NewCardRegistration {
			get;
			set;
		}

		public bool FacebookRegistration {
			get;
			set;
		}

		public string FBUser {
			get;
			set;
		}
	}
}

