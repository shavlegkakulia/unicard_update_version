using System;
using System.Windows.Input;
using MvvmCross.ViewModels;
using Kuni.Core.Services.Abstract;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using MvvmCross;

namespace Kuni.Core
{
	public class ResetPasswordViewModel : BaseViewModel
	{
		#region Private Variables

		private IUserService _userService;

		#endregion

		#region Constructor Implementation

		public ResetPasswordViewModel (IUserService userService)
		{
            _userService = userService;
#if DEBUG
			Email = "smamuchishvili@gmail.com";
			Password = "bulvari123";
			ConfirmPassword = "bulvari123";
#else
#endif
        }

		#endregion

		#region Properties

		//		private bool _dataPopulated;
		//
		//		public bool DataPopulated {
		//			get { return _dataPopulated; }
		//			set {
		//				_dataPopulated = value;
		//				RaisePropertyChanged (() => DataPopulated);
		//			}
		//		}

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

		//		private ICommand _continue;
		//
		//		public ICommand ContinueCommand {
		//			get {
		//				_continue = _continue ?? new MvvmCross.Commands.MvxCommand (ContinueResetPassword);
		//				return _continue;
		//			}
		//		}

		private ICommand _backCommand;

		public ICommand BackCommand {
			get {
				return new MvvmCross.Commands.MvxCommand (() => {
					NavigationCommand<LoginAuthViewModel> ();
				});
			}
		}

		#endregion

		#region Methods


		private string PasswordValidation (string newPassword)
		{
			string errorText = "";
			if (!string.IsNullOrWhiteSpace (newPassword)) {
				Match ifNumber = Regex.Match (newPassword, @"\d+");
				Match ifCharacter = Regex.Match (newPassword, @"[a-zA-Z]");
				if (newPassword.Length < 8 || ifNumber.Value == "" || ifCharacter.Value == "")
					errorText = "პაროლი უნდა შეიცავდეს ციფრებს და ლათინურ ასოებს. სიგრძე მინიმუმ 8 სიმბოლო";
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

		public string DisplayMessage  { get; set; }

		public void GetUserData ()
		{
			DisplayMessage = string.Empty;
			string validationStatus = Validation ();
			if (string.IsNullOrWhiteSpace (validationStatus)) {
				#region check if user Exists
				InvokeOnMainThread (() => {
					_dialog.ShowProgressDialog (ApplicationStrings.Loading);
				});
				var userExistsResponse = _userService.UserExists (_email);
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
						} else
							userExistsResponse.Result.DisplayMessage = phoneResponse.DisplayMessage;
					} else {
						userExistsResponse.Result.DisplayMessage = userExistsResponse.Result.DisplayMessage;
					}
				} 
					#endregion
					#region User Doesnt exist
					else {
					DisplayMessage = userExistsResponse.Result.DisplayMessage;
				}

				InvokeOnMainThread (() => {
					_dialog.DismissProgressDialog ();
				});

				#endregion
			} else
				DisplayMessage = validationStatus;
			
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

