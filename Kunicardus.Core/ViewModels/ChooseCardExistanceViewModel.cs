using System;
using MvvmCross.Core.ViewModels;
using System.Windows.Input;
using Kunicardus.Core.ViewModels;
using MvvmCross;
using MvvmCross.Platform;

namespace Kunicardus.Core
{
	public class ChooseCardExistanceViewModel : BaseViewModel
	{
		public ChooseCardExistanceViewModel ()
		{
		}

		private bool _newCardRegistration;

		public bool NewCardRegistration {
			get { return _newCardRegistration; }
			set{ _newCardRegistration = value; }
		}

		private ICommand _unicardAvailableCommand;

		public ICommand UnicardAvailableCommand {
			get { 
				_unicardAvailableCommand = _unicardAvailableCommand ?? new MvxCommand (ShowUnicardNumberInputViewModel);
				return _unicardAvailableCommand;
			}
		}

		private void ShowUnicardNumberInputViewModel ()
		{
			_newCardRegistration = false;
			Mvx.IocConstruct<UnicardNumberInputViewModel> ();
		}

		private ICommand _unicartNotAvaliableCommand;

		public ICommand UnicartNotAvailableCommand {
			get { 
				_unicartNotAvaliableCommand = _unicartNotAvaliableCommand ?? new MvxCommand (ShowEmailRegisterViewModel);
				return _unicartNotAvaliableCommand;
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

		private void ShowEmailRegisterViewModel ()
		{
			_newCardRegistration = true;
			Mvx.IocConstruct<RegistrationViewModel> ();
		}
	}
}

