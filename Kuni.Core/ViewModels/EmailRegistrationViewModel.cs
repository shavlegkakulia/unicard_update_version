using System;
using System.Windows.Input;
using MvvmCross.ViewModels;
using Kuni.Core.Services.Abstract;
using Kuni.Core.Models;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Kuni.Core
{
	public class EmailRegistrationViewModel : BaseViewModel
	{
		#region Private variables

		private IUserService _userService;

		#endregion

		#region Constructor Implementation

		public EmailRegistrationViewModel (IUserService userService)
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
				_continueCommand = _continueCommand ?? new MvvmCross.Commands.MvxCommand (ContinueRegister);
				return _continueCommand;
			}
		}

		private string _cardNumber;

		public string CardNumber {
			get{ return _cardNumber; }
			set{ _cardNumber = value; }
		}

		private bool _registrationCompleted;

		public bool RegisrationCompleted {
			get{ return _registrationCompleted; }
			set {
				_registrationCompleted = value;
				RaisePropertyChanged (() => RegisrationCompleted);
			}
		}

		#endregion

		#region Methods

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

			if (emailRegisterStatus != null) {
				
				if (emailRegisterStatus.Success) {
					RegisrationCompleted = true;
				} else if (!string.IsNullOrWhiteSpace (emailRegisterStatus.DisplayMessage)) {
					_uiThread.InvokeUIThread (() => {
						_dialog.ShowToast (emailRegisterStatus.DisplayMessage);
					});
				}
			}
		}

		public void RegistrationCompletedClick ()
		{
			NavigationCommand<LoginAuthViewModel> ();
		}

		private void ContinueRegister ()
		{
			ShouldValidateModel = true;
			string validationResult = Validation ();
			if (string.IsNullOrWhiteSpace (validationResult)) {
				Task.Run (() => {
					InvokeOnMainThread (() => _dialog.ShowProgressDialog (ApplicationStrings.Registering));
					EmailRegister ();
					InvokeOnMainThread (() => _dialog.DismissProgressDialog ());
				});
			} else
				InvokeOnMainThread (() => _dialog.ShowToast (validationResult));
		}

		#endregion

		#region Validation

		private string PasswordValidation ()
		{
			string errorText = "";
			if (!string.IsNullOrWhiteSpace (_password)) {
				Match ifNumber = Regex.Match (_password, @"\d+");
				Match ifCharacter = Regex.Match (_password, @"[a-zA-Z]");
				if (_password.Length < 8)
					errorText = "პაროლის სიგრძე უნდა აღემატებოდეს 8 სიმბოლოს";
				else if (ifNumber.Value == "")
					errorText = "პაროლში აუცილებელია 1 რიცხვი მაინც";
				else if (ifCharacter.Value == "")
					errorText = "პაროლში აუცილებელია 1 ლათინური ასო მაინც";
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

}

