using System.Linq;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Models.DB;
using Kuni.Core.ViewModels;
using MvvmCross.ViewModels;
using MvvmCross.Views;
using MvvmCross;
using MvvmCross.Navigation;
using System.Threading.Tasks;
//using MvvmCross;

namespace Kuni.Core
{
	public class CustomAppStart : IMvxAppStart
	{
        private readonly ILocalDbProvider _db;
        private readonly IMvxNavigationService _navigationService;

		public CustomAppStart ()
		{
            //_db = Mvx.IoCProvider.Resolve<ILocalDbProvider> ();
            _navigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
        }

        public bool IsStarted => throw new System.NotImplementedException();

        public void ResetStart()
        {
            throw new System.NotImplementedException();
        }

        public async void Start (object hint = null)
		{
			var user = _db.Get<UserInfo> ().FirstOrDefault ();
			if (user != null) {
                await _navigationService.Navigate<MainViewModel>();
			}
			{
               await _navigationService.Navigate<LoginViewModel>();
            }
        }

        public Task StartAsync(object hint = null)
        {
            return Task.Run(() => Start(hint));
        }
    }
}