using System;
using Kuni.Core.Services.Abstract;
using System.Windows.Input;
using MvvmCross.ViewModels;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using MvvmCross;

namespace Kuni.Core.ViewModels.iOSSpecific
{
	public class iRegistrationViewModel : BaseViewModel
	{
		#region Private Variables

		private IUserService _userService;

		#endregion

		#region Constructor Implementation

		public iRegistrationViewModel (IUserService userService)
		{
			_userService = userService;
		}

		private bool _newCardRegistration;

		public void Init (bool newCardRegistration, string unicardNumber)
		{
			_newCardRegistration = newCardRegistration;
			_userUnicardNumber = unicardNumber;
		}

		#endregion

		#region Properties

		private string _name;

		public string Name {
			get { return _name; }
			set {
				_name = value;
				RaisePropertyChanged (() => Name);
			}
		}

		private string _surName;

		public string Surname {
			get { return _surName; }
			set {
				_surName = value;
				RaisePropertyChanged (() => Surname);
			}
		}

		private string _idNumber;

		public string IdNumber {
			get { return _idNumber; }
			set {
				_idNumber = value;
				RaisePropertyChanged (() => IdNumber);
			}
		}

		private DateTime? _dateOfBirth;

		public DateTime? DateOfBirth {
			get { return _dateOfBirth; }
			set {
				_dateOfBirth = value;
				RaisePropertyChanged (() => DateOfBirth);
			}
		}

		private string _phoneNumber;

		public string PhoneNumber {
			get { return _phoneNumber; }
			set {
				_phoneNumber = value;
				RaisePropertyChanged (() => PhoneNumber);
			}
		}

		private string _email;

		public string Email {
			get { return _email; }
			set {
				_email = value;
				RaisePropertyChanged (() => Email);
			}
		}

		private bool _showPassword = false;

		public bool ShowPassword {
			get { return _showPassword; }
			set {
				_showPassword = value;
				RaisePropertyChanged (() => ShowPassword);
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
			get{ return _confirmPassword; }
			set {
				_confirmPassword = value;
				RaisePropertyChanged (() => ConfirmPassword);
			}
		}

		private ICommand _registerUserCommand;

		public ICommand RegisterUserCommand {
			get {
				_registerUserCommand = _registerUserCommand ?? new MvvmCross.Commands.MvxCommand (RegisterUser); 
				return _registerUserCommand;
			}
		}

		private bool _validationStatus;

		public bool ValidationStatus {
			get{ return _validationStatus; }
			set { _validationStatus = value; }
		}

		private string _userUnicardNumber;

		public string UserUnicardNumber {
			get{ return _userUnicardNumber; }
			set { _userUnicardNumber = value; }
		}

		private bool _userExists;

		public bool UserExists {
			get{ return _userExists; }
			set { 
				_userExists = value; 
				RaisePropertyChanged (() => UserExists);
			}
		}

		#endregion

		#region Methods

		private void RegisterUser ()
		{	
			ShouldValidateModel = true;
			string validationResult = Validation ();

			if (string.IsNullOrWhiteSpace (validationResult)) {
				_validationStatus = true;
				InvokeOnMainThread (() => {
					_dialog.ShowProgressDialog (ApplicationStrings.Loading);
				});
				Task.Run (async () => {					
					var userExistsService = await _userService.UserExists (_email);
					InvokeOnMainThread (() => {
						_dialog.DismissProgressDialog ();
					});
					if (!userExistsService.Result.Exists) {
						UserExists = false;
						NavigationCommand<iSMSVerificationViewModel> (new iSMSVerificationParams () {
							PhoneNumberRetrieved = false,
							PhoneNumber = _phoneNumber,
							Name = _name,
							Surname = _surName,
							PersonalId = _idNumber,
							DateOfBirth = _dateOfBirth.Value,
							Email = _email,
							Password = _password,
							UnicardNumber = _userUnicardNumber,
							NewCardRegistration = _newCardRegistration
						});		

						//Mvx.IoCConstruct<SMSVerificationViewModel> ();

					} else {
						InvokeOnMainThread (() => {
							_dialog.ShowToast (ApplicationStrings.UserExists);
						});
						UserExists = true;
					}
				});

			} else {
				InvokeOnMainThread (() => {
					_dialog.ShowToast (validationResult);
				});
				_validationStatus = false;
			}
		}

		#endregion

		#region Validation Methods

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
			string passValidation = PasswordValidation ();
			string errorText = "";
			if (string.IsNullOrWhiteSpace (_name))
				errorText = "შეიყვანეთ თქვენი სახელი";
			else if (string.IsNullOrWhiteSpace (_surName))
				errorText = "შეიყვანეთ თქვენი გვარი";
			else if (string.IsNullOrWhiteSpace (_idNumber) || _idNumber.Length != 11)
				errorText = "შეიყვანეთ პირადი ნომერი სწორი ფორმატით";
			else if (!_dateOfBirth.HasValue)
				errorText = "შეიყვანეთ დაბადების თარიღი";
			else if (string.IsNullOrWhiteSpace (_email))
				errorText = "შეიყვანეთ იმეილი";
			else if (string.IsNullOrWhiteSpace (_phoneNumber) || _phoneNumber.Length != 9)
				errorText = "შეიყვანეთ ტელეფონის ნომერი სწორი ფორმატით: 5xx xx xx xx";
			else if (!string.IsNullOrWhiteSpace (passValidation)) {
				errorText = passValidation;
			} else if (string.IsNullOrWhiteSpace (_password) || _password != _confirmPassword)
				errorText = "პაროლები ერთმანეთს არ ემთხვევა";
			else {
				Regex regex = new Regex (@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
				Match match = regex.Match (_email);
				if (!match.Success)
					errorText = "შეიყვანეთ მეილი სწორი ფორმატით";
			}
			return errorText;
		}

		#endregion
	}
}

