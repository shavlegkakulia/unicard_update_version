using System;
using MvvmCross.ViewModels;
using System.Windows.Input;
using Kuni.Core.ViewModels;
using MvvmCross;
//using MvvmCross;

namespace Kuni.Core
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
				_unicardAvailableCommand = _unicardAvailableCommand ?? new MvvmCross.Commands.MvxCommand (ShowUnicardNumberInputViewModel);
				return _unicardAvailableCommand;
			}
		}

		private void ShowUnicardNumberInputViewModel ()
		{
			_newCardRegistration = false;
			Mvx.IoCConstruct<UnicardNumberInputViewModel> ();
		}

		private ICommand _unicartNotAvaliableCommand;

		public ICommand UnicartNotAvailableCommand {
			get { 
				_unicartNotAvaliableCommand = _unicartNotAvaliableCommand ?? new MvvmCross.Commands.MvxCommand (ShowEmailRegisterViewModel);
				return _unicartNotAvaliableCommand;
			}
		}


		private ICommand _backCommand;

		public ICommand BackCommand {
			get {
				return new MvvmCross.Commands.MvxCommand (() => {
					NavigationCommand<LoginViewModel> ();
				});
			}
		}

		private void ShowEmailRegisterViewModel ()
		{
			_newCardRegistration = true;
			Mvx.IoCConstruct<RegistrationViewModel> ();
		}
	}
}

