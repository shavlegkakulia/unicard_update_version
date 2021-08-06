using System;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using MvvmCross;
using Kunicardus.Core.Services.Abstract;
using Kunicardus.Core.ViewModels;
using Kunicardus.Core.Models;
using System.Threading.Tasks;
using Kunicardus.Core.Models.DataTransferObjects;
using Kunicardus.Core.Providers.LocalDBProvider;
using Kunicardus.Core.Models.DB;
using Kunicardus.Core.Plugins.UIDialogPlugin;

namespace Kunicardus.Core
{
	public class FBRegisterViewModel : BaseViewModel
	{
		#region Private Variables

		private IUserService _userService;
		private ILocalDbProvider _dbProvider;
		private IUIDialogPlugin _dialogPlugin;

		#endregion

		#region Constructor Implementation

		public FBRegisterViewModel (IUserService userService, ILocalDbProvider dbProvider, IUIDialogPlugin dialogPlugin)
		{
			_dialogPlugin = dialogPlugin;
			_userService = userService;
			_dbProvider = dbProvider;
		}

		#endregion

		#region Properties

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
				_continueCommand = _continueCommand ?? new MvxCommand (Continue); 
				return _continueCommand;
			}
		}

		private ICommand _backCommand;

		public ICommand BackCommand {
			get {
				return new MvxCommand (() => {
					ShowViewModel<LoginViewModel> ();
				});
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
			if (string.IsNullOrWhiteSpace (validationResult)) {
				Task.Run (async() => {
					InvokeOnMainThread (() => {
						_dialog.ShowProgressDialog (ApplicationStrings.Loading);
					});
					userExistsResult = await _userService.UserExists (_email);
					_dialog.DismissProgressDialog ();
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
					} else {
						ValidationSuccess = false;
						_uiThread.InvokeUIThread (() => {
							_dialog.ShowToast ("ამ იმეილით მომხმარებელი უკვე არსებობს");
						
						});
					}
				});
			} else
				InvokeOnMainThread (() => {
					_dialog.ShowToast (validationResult);
				});
		}

		private string Validation ()
		{
			string result = "";
			if (string.IsNullOrWhiteSpace (_idNumber) || _idNumber.Length != 11)
				result = "შეიყვანეთ პირადი ნომერი სწორად";
			else if (string.IsNullOrWhiteSpace (_phoneNumber) || _phoneNumber.Length != 9)
				result = "შეიყვანეთ ტელეფონის ნომერი სწორ ფორმატში: 5xx xx xx xx";
			else if (string.IsNullOrWhiteSpace (_fullName))
				result = "შეიყვანეთ თქვენი სახელი და გვარი";
			
			return result;
		}

		#endregion
	}
}

