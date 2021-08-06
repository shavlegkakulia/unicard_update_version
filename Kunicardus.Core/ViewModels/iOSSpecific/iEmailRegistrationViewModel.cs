using System;
using Kunicardus.Core.Services.Abstract;
using System.Windows.Input;
using Kunicardus.Core.Models;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using System.Text.RegularExpressions;

namespace Kunicardus.Core.ViewModels.iOSSpecific
{
	public class iEmailRegistrationViewModel : BaseViewModel
	{
		
		#region Private variables

		private IUserService _userService;

		#endregion

		#region Constructor Implementation

		public iEmailRegistrationViewModel (IUserService userService)
		{
			_userService = userService;
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

		private ICommand _continueCommand;

		public ICommand ContinueCommand {
			get {
				_continueCommand = _continueCommand ?? new MvxCommand (ContinueRegister);
				return _continueCommand;
			}
		}

		private string _cardNumber;

		#endregion

		#region Methods

		public void Init (iEmailRegistrationViewModelParams param)
		{
			_cardNumber = param.UnicardNumber;
		}

		void EmailRegister ()
		{
			BaseActionResult<RegisterUserModel> emailRegisterStatus;
			emailRegisterStatus = _userService.RegisterUser ("",
				_email,
				_password,
				"",
				"",
				"",
				null,
				"",
				_cardNumber,
				"0",
				null);
			
			InvokeOnMainThread (() => {
				_dialog.DismissProgressDialog ();
			});

			if (emailRegisterStatus != null) {
				if (!string.IsNullOrWhiteSpace (emailRegisterStatus.DisplayMessage)) {
					_uiThread.InvokeUIThread (() => {
						_dialog.ShowToast (emailRegisterStatus.DisplayMessage);
					});
				}
				if (emailRegisterStatus.Success) {
					NavigationCommand<LoginViewModel> (null, true);
					ShowViewModel<LoginAuthViewModel> ();
				}
			}
		}

		private void ContinueRegister ()
		{
			ShouldValidateModel = true;
			string validationResult = Validation ();
			if (string.IsNullOrWhiteSpace (validationResult)) {
				InvokeOnMainThread (() => {
					_dialog.ShowProgressDialog (ApplicationStrings.Registering);
				});
				Task.Run (async () => {					
					EmailRegister ();
				});
			} else {
				InvokeOnMainThread (() => {
					_dialog.ShowToast (validationResult);
				});
			}
		}

		#endregion

		#region Validation

		private string PasswordValidation ()
		{
			string errorText = "";
			if (!string.IsNullOrWhiteSpace (_password)) {
				Match ifNumber = Regex.Match (_password, @"\d+");
				Match ifCharacter = Regex.Match (_password, @"[a-zA-Z]");
				if (_password.Length < 8 || ifNumber.Value == "" || ifCharacter.Value == "")
					errorText = ApplicationStrings.CorePasswordHint;	
			}
			return errorText;
		}

		private string Validation ()
		{
			string result = "";
			Regex regex = new Regex (@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
			string passValidation = PasswordValidation ();
			if (string.IsNullOrWhiteSpace (_email))
				result = "შეიყვანეთ ელ-ფოსტა";
			else if (!regex.Match (_email).Success)
				result = "შეიყვანეთ ელ-ფოსტა სწორი ფორმატით";
			else if (string.IsNullOrWhiteSpace (_password))
				result = "შეიყვანეთ პაროლი";
			else if (!string.IsNullOrWhiteSpace (passValidation))
				result = passValidation;
			else if (!string.Equals (_password, _confirmPassword))
				result = "პაროლი და განმეორებითი პაროლი ერთმანეთს არ ემთხვევა";
			return result;
		}

		#endregion
	}

	public class iEmailRegistrationViewModelParams
	{
		public string UnicardNumber {
			get;
			set;
		}
	}
}

