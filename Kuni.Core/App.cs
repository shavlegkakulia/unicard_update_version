using System.Linq;
using Kuni.Core.Models.DB;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Services.Abstract;
using Kuni.Core.Services.Concrete;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Plugin;
using MvvmCross.ViewModels;

namespace Kuni.Core
{
    public class App : MvvmCross.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            Mvx.IoCProvider.RegisterType<ITransactionsService, TransactionsService>();
            Mvx.IoCProvider.RegisterType<IUserService, UserService>();
            Mvx.IoCProvider.RegisterType<IOrganizationService, OrganizationService>();
            Mvx.IoCProvider.RegisterType<ISmsVerifycationService, SmsVerifycationService>();
            Mvx.IoCProvider.RegisterType<IGetUnicardStatusService, GetUnicardStatusService>();

            RegisterCustomAppStart<AppStart>();
        }
    }
}
