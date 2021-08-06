using System;
using System.Windows.Input;
using MvvmCross.ViewModels;
using Kuni.Core.Services.Abstract;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace Kuni.Core.ViewModels.iOSSpecific
{
	public class iResetPasswordViewModel : BaseViewModel
	{
		#region Private Variables

		private IUserService _userService;

		#endregion

		#region Constructor Implementation

		public iResetPasswordViewModel (IUserService userService)
		{			
			_userService = userService;
			#if DEBUG
//			this.Email = "smamuchishvili@gmail.com";
			this.Email = "smamuchishvili@gmail.com";
			this.Password = "kalmistari1";
			this.ConfirmPassword = "kalmistari1";
			#endif
		}

		public void Init (string email)
		{
			this.Email = email;
		}

		#endregion

		#region Properties

		private string _email;

		public string Email {
			get { return _email; }
			set {
				_email = value; 
				RaisePropertyChanged (() => Email);
			}
		}


		private string _password;

		public string Password {
			get { return _password; }
			set {
				_password = value; 
				RaisePropertyChanged (() => Password);
			}
		}

		private string _confirmPassword;

		public string ConfirmPassword {
			get { return _confirmPassword; }
			set {
				_confirmPassword = value;
				RaisePropertyChanged (() => ConfirmPassword);

			}
		}

		private ICommand _continue;

		public ICommand ContinueCommand {
			get {
				_continue = _continue ?? new MvvmCross.Commands.MvxCommand (ContinueResetPassword);
				return _continue;
			}
		}

		#endregion

		#region Methods

		private void ContinueResetPassword ()
		{
			Task.Run (() => {
				GetUserData ();
			});
		}

		private string PasswordValidation (string newPassword)
		{
			string errorText = "";
			if (!string.IsNullOrWhiteSpace (newPassword)) {
				Match ifNumber = Regex.Match (newPassword, @"\d+");
				Match ifCharacter = Regex.Match (newPassword, @"[a-zA-Z]");
				if (newPassword.Length < 8 || ifNumber.Value == "" || ifCharacter.Value == "")
					errorText = ApplicationStrings.CorePasswordHint;
				//				else if (ifNumber.Value == "")
				//					errorText = "პაროლში აუცილებელია 1 რიცხვი მაინც";
				//				else if (ifCharacter.Value == "")
				//					errorText = "პაროლში აუცილებელია 1 ლათინური ასო მაინც";
			} else
				errorText = "შეავსეთ პაროლი";
			return errorText;
		}

		private string Validation ()
		{
			string errorText = "";
			if (string.IsNullOrWhiteSpace (_email)) {
				errorText = "შეიყვანეთ ელ-ფოსტა";
			} else if (!string.IsNullOrWhiteSpace (_email)) {
				Regex regex = new Regex (@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
				Match match = regex.Match (_email);
				if (!match.Success)
					errorText = "შეიყვანეთ ელ-ფოსტა სწორი ფორმატით";
				else if (string.IsNullOrWhiteSpace (_password))
					errorText = "შეიყვანეთ ახალი პაროლი";
				else if (_password != _confirmPassword)
					errorText = "პაროლები ერთმანეთს არ ემთხვევა";
				else
					errorText = PasswordValidation (_password);
			} 

			return errorText;
		}

		private UserModelForResetPassword _userData;

		public UserModelForResetPassword UserData {
			get { return _userData; }
			set {
				_userData = value;
				RaisePropertyChanged (() => UserData);

			}
		}

		public  void GetUserData ()
		{
			
			string validationStatus = Validation ();
			if (string.IsNullOrWhiteSpace (validationStatus)) {
				//await Task.Run (async () => {
				#region check if user Exists
				InvokeOnMainThread (() => {
					_dialog.ShowProgressDialog (ApplicationStrings.Loading);
				});
				var userExistsResponse = _userService.UserExists (_email);
				InvokeOnMainThread (() => {
					_dialog.DismissProgressDialog ();
				});
				#endregion
				#region User Exists
				if (userExistsResponse.Result.Success) {
					if (userExistsResponse.Result.Result.Exists) {
						var phoneResponse = _userService.GetUserByPhone (_email);
						if (phoneResponse.Success) {
							UserModelForResetPassword userData = new UserModelForResetPassword () {
								UserPhoneNumber = phoneResponse.Result.UserPhone,
								UserId = phoneResponse.Result.UserId,
								UserName = _email,
								NewPassword = _password
							};
							UserData = userData;
							NavigationCommand<iSMSVerificationViewModel> (new iSMSVerificationParams () {
								PhoneNumberRetrieved = true,
								Password = _password,
								Email = _email,
								UserId = phoneResponse.Result.UserId,
								PhoneNumber = phoneResponse.Result.UserPhone,
								ResetPassword = true
							});
							// TODO: goto sms verification
						} else
							InvokeOnMainThread (() => {
								_dialog.ShowToast (phoneResponse.DisplayMessage);
							});
					} else {
						InvokeOnMainThread (() => {
							_dialog.ShowToast ("მითითებული ელ-ფოსტით მომხმარებელი არ არსებობს");
						});
					}
				} 
				#endregion
				#region User Doesnt exist
				else {
					InvokeOnMainThread (() => {
						_dialog.ShowToast (userExistsResponse.Result.DisplayMessage);
					});
				}
				#endregion
				//});
			} else
				InvokeOnMainThread (() => {
					_dialog.ShowToast (validationStatus);
				});
		}

		#endregion
	}

	#region Message Classes
	public class UserModelForResetPassword
	{
		public string UserPhoneNumber{ get; set; }

		public string UserId { get; set; }

		public string UserName { get; set; }

		public string NewPassword { get; set; }
	}

	#endregion
}

