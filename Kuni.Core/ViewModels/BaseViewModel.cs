using Kuni.Core.Plugins.UIDialogPlugin;
using Kuni.Core.Plugins.IUIThreadPlugin;
using System.Collections.Generic;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Models.DB;
using System.Linq;
using Kuni.Core.Helpers.Device;
//using MvvmCross.ViewModels;
using MvvmCross;
//using MvvmCross.ViewModels;
//using MvvmCross;
using MvvmCross.Views;
using MvvmCross.ViewModels;
using MvvmCross.Navigation;

namespace Kuni.Core
{
	public class BaseViewModel : MvxViewModel, IMvxViewModel
    {
        private readonly IMvxNavigationService _navigationService;
        private bool _shouldValidateModel;

		public bool ShouldValidateModel {
			get {
				return _shouldValidateModel;
			}
			set {
				_shouldValidateModel = value;
				RaisePropertyChanged (() => ShouldValidateModel);
			}
		}

		protected readonly IUIDialogPlugin _dialog;
		protected readonly IUIThreadPlugin _uiThread;
		protected  IDevice _device;

		public BaseViewModel ()
		{
            _navigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
			_dialog = Mvx.IoCProvider.Resolve<IUIDialogPlugin> ();
			_uiThread = Mvx.IoCProvider.Resolve<IUIThreadPlugin> ();
			_device = Mvx.IoCProvider.Resolve<IDevice> ();
		}

		protected async void NavigationCommand<T> (object param = null, bool clearStack = false) where T : IMvxViewModel
		{
			MvxBundle bundle = null;
			if (clearStack) {
				bundle = new MvxBundle (new Dictionary<string, string> { { "CLEAR_STACK", "true" } });
			}
            await _navigationService.Navigate<T>(bundle, new System.Threading.CancellationToken());
		}

		public void Logout (bool authfailed = false)
		{
			InvokeOnMainThread (() => {
				if (_dialog != null) {
					_dialog.DismissProgressDialog ();
				}

				using (ILocalDbProvider _dbProvider = Mvx.IoCProvider.Resolve<ILocalDbProvider> ()) {

					var users = _dbProvider.Query<UserInfo> ("select * from UserInfo");
					if (users != null && users.Count > 0) {
						var user = users.FirstOrDefault ();
						_dbProvider.Execute ("delete from AutoCompleteFields");
						if (!user.IsFacebookUser) {							
							_dbProvider.Insert<AutoCompleteFields> (new AutoCompleteFields () {
								Id = 0,
								UserEmail = user.Username
							});
						}
					}
					_dbProvider.Execute ("delete from UserInfo");

					_dbProvider.Execute ("delete from TransactionInfo;");
				}
				if (_device.Platform == "ios" || authfailed) {
					NavigationCommand<LoginViewModel> (null, true);
				}
			});
		}
	}
}

