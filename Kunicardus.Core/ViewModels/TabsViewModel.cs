using Kunicardus.Core.Models;
using Kunicardus.Core.Plugins.UIDialogPlugin;
using Kunicardus.Core.Providers.LocalDBProvider;
using Kunicardus.Core.Services.Abstract;
using MvvmCross.Platform;

namespace Kunicardus.Core.ViewModels
{
    public class TabsViewModel : BaseViewModel
    {
        IUserService _userService;
        ILocalDbProvider _dbProvider;
        IUIDialogPlugin _dialogPlugin;
        BaseActionResult<UserBalanceModel> response;

        public TabsViewModel(IUserService userService, ILocalDbProvider dbProvider, IUIDialogPlugin dialogPlugin)
        {
            _userService = userService;
            _dbProvider = dbProvider;
            _dialogPlugin = dialogPlugin;

            //			if (ContentView == null) {
            //				ContentView = Mvx.IocConstruct<HomePageViewModel> ();
            //			}
            if (MyPageView == null)
            {
                MyPageView = Mvx.IocConstruct<MyPageViewModel>();
            }
            //			if (Catalog == null)
            //				Catalog = Mvx.IocConstruct<BaseCatalogViewModel> ();

            if (Merchants == null)
            {
                Merchants = Mvx.IocConstruct<MerchantsViewModel>();
            }

        }

        #region Child Fragment Control

        private MyPageViewModel _myPageView;
        public MyPageViewModel MyPageView
        {
            get { return _myPageView; }
            set
            {
                _myPageView = value;
                RaisePropertyChanged(() => MyPageView);
            }
        }

        private MerchantsViewModel _merchantsView;
        public MerchantsViewModel Merchants
        {
            get { return _merchantsView; }
            set
            {
                _merchantsView = value;
                RaisePropertyChanged(() => Merchants);
            }
        }
        #endregion
    }
}
