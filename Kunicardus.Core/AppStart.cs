
using System;
using MvvmCross.Core.ViewModels;
using Kunicardus.Core.ViewModels;
using MvvmCross;
using Kunicardus.Core.Helpers.Device;
using Kunicardus.Core.ViewModels.iOSSpecific;
using MvvmCross.Platform;

namespace Kunicardus.Core
{
	public class AppStart : MvxNavigatingObject, IMvxAppStart
	{
		bool _isAuthed;

		public AppStart (bool authed = false)
		{
			_isAuthed = authed;
		}

		/// <summary>
		/// Start is called on startup of the app
		/// Hint contains information in case the app is started with extra parameters
		/// </summary>
		public void Start (object hint = null)
		{
			var device = Mvx.Resolve<IDevice> ();
			if (_isAuthed) {
				if (device.Platform == "ios") {
					ShowViewModel<RootViewModel> ();
				} else {
					ShowViewModel<MainViewModel> ();
				}
			} else {
				ShowViewModel<LoginViewModel> ();
			}
		}

	}
}

