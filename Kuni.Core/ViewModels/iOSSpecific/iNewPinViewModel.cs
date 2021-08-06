using System;

namespace Kuni.Core
{
	public class iNewPinViewModel : BaseViewModel
	{

		public void Init (string headerTitle, string pageTitle)
		{
			this.HeaderTitle = headerTitle;
			this.PageTitle = pageTitle;
		}

		#region Properties

		private string _newPin;

		public string NewPin {
			get{ return _newPin; }
			set {
				_newPin = value;
				RaisePropertyChanged (() => NewPin);
			}
		}

		private string _headerTitle;

		public string HeaderTitle {
			get{ return _headerTitle; }
			set {
				_headerTitle = value;
			}
		}

		private string _pageTitle;

		public string PageTitle {
			get{ return _pageTitle; }
			set {
				_pageTitle = value;
			}
		}

		public bool FromSetPin {
			get;
			set;
		}

		private bool _pinInputFinished;

		public bool PinInputFinished {
			get{ return _pinInputFinished; }
			set {
				if (value) {
					if (!FromSetPin)
						NavigationCommand<iConfirmNewPinViewModel> (new {
						fromSetPin = FromSetPin,
						newPin = _newPin,
						headerTitle = ApplicationStrings.ChangePin,
						pageTitle = ApplicationStrings.repeat_new_pin});
					else {
						NavigationCommand<iConfirmNewPinViewModel> (new {
							fromSetPin = FromSetPin,
							newPin = _newPin,
							headerTitle = ApplicationStrings.set_pin,
							pageTitle = ApplicationStrings.repeat_new_pin});
					} 
						
				}
				_pinInputFinished = value;
			}
		}

		#endregion


	}
}

