using Kunicardus.Core.Plugins.UIDialogPlugin;
using Kunicardus.Core.Plugins.IUIThreadPlugin;
using System.Collections.Generic;
using Kunicardus.Core.Providers.LocalDBProvider;
using Kunicardus.Core.Models.DB;
using System.Linq;
using Kunicardus.Core.Helpers.Device;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace Kunicardus.Core
{
	public class BaseViewModel : MvxViewModel, IMvxViewModel
    {

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
			_dialog = Mvx.Resolve<IUIDialogPlugin> ();
			_uiThread = Mvx.Resolve<IUIThreadPlugin> ();
			_device = Mvx.Resolve<IDevice> ();
		}

		protected void NavigationCommand<T> (object param = null, bool clearStack = false) where T : IMvxViewModel
		{
			MvxBundle bundle = null;
			if (clearStack) {
				bundle = new MvxBundle (new Dictionary<string, string> { { "CLEAR_STACK", "true" } });
			}
			ShowViewModel<T> (param, bundle);
		}

		public void Logout (bool authfailed = false)
		{
			InvokeOnMainThread (() => {
				if (_dialog != null) {
					_dialog.DismissProgressDialog ();
				}

				using (ILocalDbProvider _dbProvider = Mvx.Resolve<ILocalDbProvider> ()) {

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

