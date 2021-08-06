using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Android.Content;
using Kuni.Core;
using Kuni.Core.Helpers.AppSettings;
using Kuni.Core.Helpers.Device;
using Kuni.Core.Plugins.Connectivity;
using Kuni.Core.Plugins.IUIThreadPlugin;
using Kuni.Core.Plugins.UIDialogPlugin;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Services.Abstract;
using Kuni.Core.UnicardApiProvider;
using Kuni.Core.Utilities.Logger;
using Kuni.Core.ViewModels;
using Kunicardus.Droid.Fragments;
using Kunicardus.Droid.Helpers.AppSettings;
using Kunicardus.Droid.Helpers.Device;
using Kunicardus.Droid.Plugins.Connectivity;
using Kunicardus.Droid.Plugins.UIDialogPlugin;
using Kunicardus.Droid.Plugins.UIThreadPlugin;
using Kunicardus.Droid.Providers.DroidSqLiteProvider;
using Kunicardus.Droid.Views;
//using MvvmCross.Droid.Platform;
using MvvmCross;
//using MvvmCross.Binding.Droid.Views;
using MvvmCross.Platforms.Android.Binding.Views;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.Platforms.Android.Views;
//using MvvmCross.Platforms.Android.Core;
using MvvmCross.Plugin.File;
using MvvmCross.Views;

namespace Kunicardus.Droid
{
    public class Setup : MvxAndroidSetup
    {
        public static string PicturesPath
        {
            get
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Pics");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        public Setup() : base()
        {

        }

        public Setup(Context applicationContext) : base()
		{
		}

        protected override MvvmCross.ViewModels.IMvxApplication CreateApp()
        {
            Mvx.IoCProvider.RegisterType<IUIDialogPlugin, DroidUIDialogPlugin>();
          
            InitializeIoc();
            return new App();
        }

		protected override void InitializeLastChance ()
		{
			base.InitializeLastChance ();

			CreateConfigIfNotExists ();
		}

        protected override IEnumerable<Assembly> AndroidViewAssemblies {
			get {
				var toReturn = base.AndroidViewAssemblies.ToList();
                toReturn.Add (typeof(LoginView).Assembly);
                toReturn.Add (typeof(Kunicardus.Droid.BaseTextView).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.BaseEditText).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.BaseButton).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.FocusableBaseEditText).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.BindableWebView).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.Adapters.CatalogListAdapter).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.Adapters.TransfersListViewAdapter).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.Adapters.NewsListViewAdapter).Assembly);
				toReturn.Add (typeof(MvxListView).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.BaseFBButton).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.BaseIngiriTextView).Assembly);
				toReturn.Add (typeof(Android.Support.V4.App.FragmentTabHost).Assembly);
				return toReturn;
			}
		}

		private void CreateConfigIfNotExists ()
		{
			var fileStore = Mvx.IoCProvider.Resolve<IMvxFileStore> ();
			var device = Mvx.IoCProvider.Resolve<IDevice> ();
			var bundleConfig = Mvx.IoCProvider.Resolve<IConfigBundlePlugin> ();
			var fullPath = fileStore.PathCombine (device.DataPath, Constants.ConfigFileName);

			fileStore.EnsureFolderExists (device.DataPath);

			if (fileStore.Exists (fullPath))
				return;


			fileStore.WriteFile (fullPath, bundleConfig.ConfigText);
		}

        protected void InitializeIoc ()
		{
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "unicard.db");
            if(!File.Exists(dbPath))
            {
                System.Data.SQLite.SQLiteConnection.CreateFile(dbPath);
            }
            
            //base.InitializeIoC();
            Mvx.IoCProvider.RegisterType<IMvxViewsContainer, ViewsContainer>();
            Mvx.IoCProvider.RegisterType<IDevice, DroidDevicePlugin> ();
			Mvx.IoCProvider.RegisterType<IConfigBundlePlugin, DroidConfigBundleProvider> ();
			Mvx.IoCProvider.RegisterType<IAppSettings, AppSettings> ();
			Mvx.IoCProvider.RegisterType<ILocalDbProvider, DroidSqLiteProvider> ();
			Mvx.IoCProvider.RegisterType<IConnectivityPlugin, DroidConnectivityProviderPlugin> ();
			Mvx.IoCProvider.RegisterType<ILoggerService, CoreLogger> ();
			Mvx.IoCProvider.RegisterType<IUnicardApiProvider, UnicardApiProvider> ();
			Mvx.IoCProvider.RegisterType<IAuthService,AuthService> ();
			Mvx.IoCProvider.RegisterType<IUIThreadPlugin,UIThreadPlugin> ();
			Mvx.IoCProvider.RegisterType<ICustomSecurityProvider,CustomSecurityProvider> ();
			Mvx.IoCProvider.RegisterType<IConfigReader, ConfigReader> ();
			Mvx.IoCProvider.RegisterType<IGoogleAnalyticsService, GoogleAnalyticsDroid> ();
         //   InitializeViewLookup(new Dictionary<Type, Type>());

        }
  
        protected override IMvxViewsContainer InitializeViewLookup(IDictionary<Type, Type> viewModelViewLookup)
        {
           //  base.InitializeViewLookup(viewModelViewLookup);
            viewModelViewLookup = new Dictionary<Type, Type>()
            {
                { typeof (LoginViewModel), typeof(LoginView) },
                { typeof (LoginAuthViewModel), typeof(LoginAuthView) },
                { typeof (BaseRegisterViewModel), typeof(BaseRegisterView) },
                { typeof (BaseResetPasswordViewModel), typeof(BaseResetPasswordView) },
                { typeof (ImageSliderViewModel), typeof(ImageSliderAdapter) },
                { typeof (MainViewModel), typeof(MainView) },
                { typeof (MerchantsViewModel), typeof(MerchantsView) },
                { typeof (NewsDetailsViewModel), typeof(NewsDetailsView) },
                { typeof (OrganizationDetailsViewModel), typeof(OrganizationDetailsViewModel) },
            };

            var container = Mvx.IoCProvider.Resolve<IMvxViewsContainer>();
            container.AddAll(viewModelViewLookup);
            return base.InitializeViewLookup(viewModelViewLookup);
        }



        //protected override void InitializeViewLookup()
        //{
        //    var viewModelViewLookup = new Dictionary<Type, Type>()
        //    {
        //        { typeof (LoginViewModel), typeof(LoginView) },
        //        { typeof (LoginAuthViewModel), typeof(LoginAuthView) },
        //        { typeof (BaseRegisterViewModel), typeof(BaseRegisterView) },
        //        { typeof (BaseResetPasswordViewModel), typeof(BaseResetPasswordView) },
        //        //{ typeof (AboutViewModel), typeof(AboutFragment) },
        //        //{ typeof (BuyProductViewModel), typeof(BuyProductFragment) },
        //        //{ typeof (BaseCatalogViewModel), typeof(BaseCatalogFragment) },
        //        //{ typeof (CatalogDetailViewModel), typeof(CatalogDetailFragment) },
        //        //{ typeof (CatalogListViewModel), typeof(CatalogListViewFragment) },
        //        //{ typeof (ChangePasswordViewModel), typeof(ChangePasswordFragment) },
        //        //{ typeof (MenuViewModel), typeof(MenuFragment) },
        //        //{ typeof (ChooseCardExistanceViewModel), typeof(ChooseCardExistanceViewFragment) },
        //        //{ typeof (EmailRegistrationViewModel), typeof(EmailRegistrationFragment) },
        //        //{ typeof (FBRegisterViewModel), typeof(FBRegisterFragment) },
        //        //{ typeof (HomePageViewModel), typeof(HomePageFragment) },
        //        //{ typeof (OrganisationListViewModel), typeof(OrganisationListFragment) },
        //        //{ typeof (PinViewModel), typeof(SettingsPinFragment) },
        //        //{ typeof (SMSVerificationViewModel), typeof(SMSVerificationFragment) },
        //        //{ typeof (TabsViewModel), typeof(tabs) },
        //        //{ typeof (TransactionVerificationViewModel), typeof(TransactionVerificationFragment) },
        //        //{ typeof (UnicardNumberInputViewModel), typeof(UnicardInputFragment) },
        //        //{ typeof (MyPageViewModel), typeof(MyPageFragment) },
        //        //{ typeof (NewsListViewModel), typeof(NewsListFragment) },
        //        { typeof (ImageSliderViewModel), typeof(ImageSliderAdapter) },
        //        { typeof (MainViewModel), typeof(MainView) },
        //        { typeof (MerchantsViewModel), typeof(MerchantsView) },
        //        { typeof (NewsDetailsViewModel), typeof(NewsDetailsView) },
        //        { typeof (OrganizationDetailsViewModel), typeof(OrganizationDetailsViewModel) },
        //    };

        //    var container = Mvx.IoCProvider.Resolve<IMvxViewsContainer>();
        //    container.AddAll(viewModelViewLookup);
        //}

    }
}