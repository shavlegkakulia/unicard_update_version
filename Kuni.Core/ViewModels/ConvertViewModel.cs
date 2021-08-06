using System;

namespace Kuni.Core
{
	public class ConvertViewModel :BaseViewModel
	{
		private string _unicardNumber;

		public string UnicardNumber {
			get{ return _unicardNumber; }
			set {
				_unicardNumber = value;
				RaisePropertyChanged (() => UnicardNumber);
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
	}
}

