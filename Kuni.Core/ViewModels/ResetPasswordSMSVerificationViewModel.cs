using System;
using System.Windows.Input;
using MvvmCross.ViewModels;
using Kuni.Core.Services.Abstract;
using System.Text.RegularExpressions;

namespace Kuni.Core
{
	public class ResetPasswordSMSVerificationViewModel : BaseViewModel
	{
		#region Private Variables

		IUserService _userService;
		ISmsVerifycationService _smsVerificationService;

		#endregion

		#region Constructor Implementation

		public ResetPasswordSMSVerificationViewModel (IUserService userService, ISmsVerifycationService smsVerificationService)
		{
			_userService = userService;
			_smsVerificationService = smsVerificationService;
		}

		#endregion

		#region Properties

		private string _newPassword;

		public string NewPassword { 
			get {
				return _newPassword;
			} 
			set { 
				_newPassword = value;
			} 
		}


		private string _userName;

		public string UserName { 
			get {
				return _userName;
			} 
			set { 
				_userName = value;
			} 
		}

		private string _userPhoneNumber;

		public string UserPhoneNumber { 
			get {
				return _userPhoneNumber;
			} 
			set { 
				_userPhoneNumber = value;
			} 
		}

		private string _userId;

		public string UserID { 
			get {
				return _userId;
			} 
			set { 
				_userId = value;
			} 
		}

		private string _verificationCode;

		public string VerificationCode {
			get{ return _verificationCode; }
			set {
				_verificationCode = value;
				RaisePropertyChanged (() => VerificationCode);
			}
		}

		#endregion

		#region Commands

		private ICommand _resetPassword;

		public ICommand ResetPasswordCommand { 
			get {
				_resetPassword = _resetPassword ?? new MvvmCross.Commands.MvxCommand (ResetPassword);
				return _resetPassword;
			} 
		}


		#endregion

		#region Methods

		public bool SendOTP (string userId, string userPhone)
		{
			
			var response = _smsVerificationService.SendOTP (userId, "", userPhone);
			if (!response.Success)
				_dialog.ShowToast (response.DisplayMessage);
			return response.Success;
		}

		private void ResetPassword ()
		{
			if (!string.IsNullOrWhiteSpace (_verificationCode)) {

				var resetPasswordResponse = _userService.ResetPassword (_userName, _verificationCode, _newPassword);
				if (resetPasswordResponse.Success)
					NavigationCommand<LoginAuthViewModel> ();
		
				InvokeOnMainThread (() => {
					_dialog.ShowToast (resetPasswordResponse.DisplayMessage);
				});
			} else
				_dialog.ShowToast ("შეიყვანეთ ვერიფიკაციის კოდი");
		}

		#endregion
	}
}

