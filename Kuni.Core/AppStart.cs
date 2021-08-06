
using System;
using MvvmCross.ViewModels;
using Kuni.Core.ViewModels;
using MvvmCross;
using Kuni.Core.Helpers.Device;
using Kuni.Core.ViewModels.iOSSpecific;
using System.Threading.Tasks;
using MvvmCross.Navigation;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Models.DB;
using System.Linq;
////using MvvmCross;

namespace Kuni.Core
{
	public class AppStart : MvxAppStart
	{
        private readonly IMvxNavigationService _navigationService;

        public AppStart (IMvxApplication app, IMvxNavigationService navigationService) : base(app, navigationService)
		{
            _navigationService = navigationService;
        }

        protected async override Task NavigateToFirstViewModel(object hint = null)
        {
            bool isAuthed;
            using (var db = Mvx.IoCProvider.Resolve<ILocalDbProvider>())
            {
                var user = db.Get<UserInfo>().FirstOrDefault();
                isAuthed = user != null;
            }

            var device = Mvx.IoCProvider.Resolve<IDevice>();
            if (isAuthed)
            {
                if (device.Platform == "ios")
                {
                    await _navigationService.Navigate<RootViewModel>();
                }
                else
                {
                    await _navigationService.Navigate<MainViewModel>();
                }
            }
            else
            {
                await _navigationService.Navigate<LoginViewModel>();
            }
        }
    }
}

