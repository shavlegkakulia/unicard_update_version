using System;
using Autofac;
using iCunOS.BillBoards.Providers.SecurityProvider;
using Kunicardus.Billboards.Core;
using Kunicardus.Billboards.Core.Plugins;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using Kunicardus.Billboards.Core.Services.Abstract;
using Kunicardus.Billboards.Core.Services;
using Kunicardus.Billboards.Core.Services.Concrete;
using Kunicardus.Billboards.Core.ViewModels;
using iCunOS.BillBoards.Plugins.Connectivity;

namespace iCunOS.BillBoards
{
	public class App
	{
		public static IContainer Container { get; set; }

		public static void Initialize ()
		{
			var builder = new ContainerBuilder ();

			builder.RegisterType<SecurityProvider> ().As<ICustomSecurityProvider> ();
			builder.RegisterType<TouchConnectivityPlugin> ().As<IConnectivityPlugin> ();
			builder.RegisterType<UnicardApiProvider> ().As<IUnicardApiProvider> ();
			builder.RegisterType<AdsService> ().As<IAdsService> ();
			builder.RegisterType<AuthService> ().As<IAuthService> ();
			builder.RegisterType<BillboardsService> ().As<IBillboardsService> ();
			builder.RegisterType<UserService> ().As<IUserService> ();

			builder.RegisterType<MainViewModel> ();
			builder.RegisterType<SettingsViewModel> ();
			builder.RegisterType<LoginViewModel> ();
			builder.RegisterType<HomePageViewModel> ();
			builder.RegisterType<AuthViewModel> ();
			builder.RegisterType<AdsListViewModel> ();
			builder.RegisterType<AdvertismentViewModel> ();
			builder.RegisterType<BillboardsViewModel> ();
			builder.RegisterType<HistoryViewModel> ();
			builder.RegisterType<MenuViewModel> ();

			App.Container = builder.Build ();
		}
	}
}

