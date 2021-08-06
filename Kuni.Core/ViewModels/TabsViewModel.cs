using Kuni.Core.Models;
using Kuni.Core.Plugins.UIDialogPlugin;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Services.Abstract;
using MvvmCross;
//using MvvmCross;

namespace Kuni.Core.ViewModels
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
            //				ContentView = Mvx.IoCConstruct<HomePageViewModel> ();
            //			}
            if (MyPageView == null)
            {
                MyPageView = Mvx.IoCConstruct<MyPageViewModel>();
            }
            //			if (Catalog == null)
            //				Catalog = Mvx.IoCConstruct<BaseCatalogViewModel> ();

            if (Merchants == null)
            {
                Merchants = Mvx.IoCConstruct<MerchantsViewModel>();
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
