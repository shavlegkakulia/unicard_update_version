using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Autofac;
using Kunicardus.Billboards.Core.Services.Abstract;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using Kunicardus.Billboards.Core.Services;
using Kunicardus.Billboards.Core.ViewModels;
using Kunicardus.Billboards.Core.Services.Concrete;
using Kunicardus.Billboards.Plugins;
using Kunicardus.Billboards.Core.Plugins;
using Kunicardus.Billboards.Core;
using Kunicardus.Billboards.Fragments;

namespace Kunicardus.Billboards
{
    [Application(Icon = "@drawable/app_icon", Label = "Uniboard")]
    public class App : Application
    {
        public static IContainer Container { get; set; }
        public App(IntPtr h, JniHandleOwnership jho) : base(h, jho)
        {

        }

        public override void OnCreate()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<CustomSecurityProvider>().As<ICustomSecurityProvider>();
            builder.RegisterType<ConnectivityPlugin>().As<IConnectivityPlugin>();
            builder.RegisterType<UnicardApiProvider>().As<IUnicardApiProvider>();
            builder.RegisterType<AdsService>().As<IAdsService>();
            builder.RegisterType<AuthService>().As<IAuthService>();
            builder.RegisterType<BillboardsService>().As<IBillboardsService>();
            builder.RegisterType<UserService>().As<IUserService>();

            builder.RegisterType<SettingsViewModel>();
            builder.RegisterType<AdsFragment>();
            builder.RegisterType<MainViewModel>();
            builder.RegisterType<LoginViewModel>();
            builder.RegisterType<HomePageViewModel>();
            builder.RegisterType<AuthViewModel>();
            builder.RegisterType<AdsListViewModel>();
            builder.RegisterType<BillboardsViewModel>();
            builder.RegisterType<HistoryViewModel>();
            App.Container = builder.Build();

            base.OnCreate();
        }
    }
}