using System.Linq;
using Kunicardus.Core.Providers.LocalDBProvider;
using Kunicardus.Core.Models.DB;
using Kunicardus.Core.ViewModels;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Platform;

namespace Kunicardus.Core
{
	public class CustomAppStart : MvxNavigatingObject, IMvxAppStart
	{
		private readonly ILocalDbProvider _db;

		public CustomAppStart ()
		{
			_db = Mvx.Resolve<ILocalDbProvider> ();
		}

		public void Start (object hint = null)
		{
			var user = _db.Get<UserInfo> ().FirstOrDefault ();
			if (user != null) {
				ShowViewModel<MainViewModel> ();
			}
			{
                ShowViewModel<LoginViewModel> ();
			}
		}
	}
}