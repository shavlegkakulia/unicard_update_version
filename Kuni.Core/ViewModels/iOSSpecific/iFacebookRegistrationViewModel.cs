using System;
using Kuni.Core.Models.DataTransferObjects;
using Kuni.Core.Services.Abstract;
using Kuni.Core.Providers.LocalDBProvider;
using System.Windows.Input;
using MvvmCross.ViewModels;
using Kuni.Core.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kuni.Core.ViewModels.iOSSpecific
{
	public class iFacebookRegistrationViewModel : BaseViewModel
	{
		#region Private Variables

		private IUserService _userService;

		#endregion

		#region Ctors

		public iFacebookRegistrationViewModel (IUserService userService)
		{			
			_userService = userService;
		}

		#endregion

		#region Init

		public void Init (TransferUserModel fbUser)
		{
			_newFBUser = fbUser;
			FBId = fbUser.FBId;
			Name = fbUser.Name;
			SurName = fbUser.Surname;
			Email = fbUser.Email;
			FullName = fbUser.Name + " " + fbUser.Surname;
			_newCardRegistration = fbUser.NewCardRegistration;
			_cardNumber = fbUser.CardNumber;
		}

		#endregion

		#region Properties

		private string _cardNumber;
		private bool _newCardRegistration;

		private string _fbId;

		public string FBId {
			get{ return _fbId; }
			set{ _fbId = value; }
		}

		private string _name;

		public string Name {
			get{ return _name; }
			set {
				_name = value;
			}
		}

		private string _surName;

		public string SurName {
			get{ return _surName; }
			set {
				_surName = value;
			}
		}

		private string _fullName;

		public string FullName {
			get{ return _fullName; }
			set {
				_fullName = value;
				RaisePropertyChanged (() => FullName);
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
			get{ return _email; }
			set {
				_email = value;
				RaisePropertyChanged (() => Email);
			}
		}

		private ICommand _continueCommand;

		public ICommand ContinueCommand {
			get {
				_continueCommand = _continueCommand ?? new MvvmCross.Commands.MvxCommand (Continue); 
				return _continueCommand;
			}
		}

		private bool _validationSuccess;

		public bool ValidationSuccess {
			get { return _validationSuccess; }
			set {
				_validationSuccess = value;
				RaisePropertyChanged (() => ValidationSuccess);
			}
		}

		private TransferUserModel _newFBUser;

		public TransferUserModel NewFBUser {
			get{ return _newFBUser; }
			set{ _newFBUser = value; }
		}

		#endregion

		#region Methods

		private void Continue ()
		{
			ShouldValidateModel = true;
			string validationResult = Validation ();
			BaseActionResult<UserExistsModel> userExistsResult;
			InvokeOnMainThread (() => {
				_dialog.ShowProgressDialog (ApplicationStrings.Loading);
			});
			if (string.IsNullOrWhiteSpace (validationResult)) {
				Task.Run (async() => {

					userExistsResult = await _userService.UserExists (_email);
					InvokeOnMainThread (() => {
						_dialog.DismissProgressDialog ();
					});
					if (!userExistsResult.Result.Exists) {
						ValidationSuccess = true;
						_newFBUser = new TransferUserModel ();
						_newFBUser.Email = _email;
						_newFBUser.Name = (_fullName.Substring (0, _fullName.IndexOf (' ')));
						_newFBUser.Surname = (_fullName.Substring (_fullName.IndexOf (' ')));
						_newFBUser.PhoneNumber = _phoneNumber;	
						_newFBUser.DateOfBirth = _dateOfBirth;
						_newFBUser.PersonalId = _idNumber;
						_newFBUser.FBId = _fbId;
						_newFBUser.CardNumber = _cardNumber;
						_newFBUser.NewCardRegistration = _newCardRegistration;
						NavigationCommand<iSMSVerificationViewModel> (new iSMSVerificationParams () { 
							PhoneNumberRetrieved = false,
							FacebookRegistration = true,
							FBUser = JsonConvert.SerializeObject (_newFBUser),
							PhoneNumber = PhoneNumber,
							NewCardRegistration = _newCardRegistration,
							UnicardNumber = _cardNumber
						});
					} else {
						ValidationSuccess = false;
						_uiThread.InvokeUIThread (() => {
							_dialog.ShowToast ("ამ ელ-ფოსტით მომხმარებელი უკვე არსებობს");
						});
					}
				});
			} else {
				InvokeOnMainThread (() => {
					_dialog.DismissProgressDialog ();
					_dialog.ShowToast (validationResult);
				});
			}
		}

		private string Validation ()
		{
			string result = "";
			if (string.IsNullOrWhiteSpace (_idNumber) || _idNumber.Length != 11)
				result = "შეიყვანეთ პირადი ნომერი სწორი ფორმატით";
			else if (string.IsNullOrWhiteSpace (_phoneNumber) || _phoneNumber.Length != 9)
				result = "ტელეფონის ნომერი უნდა შეიყვანოთ შემდეგი ფორმატით: 5xx xx xx xx";
			else if (string.IsNullOrWhiteSpace (_fullName))
				result = "შეიყვანეთ თქვენი სახელი და გვარი";

			return result;
		}

		#endregion


	}
}

