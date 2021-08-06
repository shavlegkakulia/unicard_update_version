using System;
using Kuni.Core.Services.Abstract;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Plugins.UIDialogPlugin;
using System.Windows.Input;
using MvvmCross.ViewModels;
using Kuni.Core.Models.DB;
using Kuni.Core.Models;
using System.Threading.Tasks;
using Kuni.Core.ViewModels;
using Kuni.Core.ViewModels.iOSSpecific;
using Kuni.Core.Helpers.Device;
using System.Linq;
using MvvmCross;

namespace Kuni.Core
{
    public class LoginAuthViewModel : BaseViewModel
    {
        #region Variables

        IAuthService _authService;
        IUserService _userService;
        ILocalDbProvider _dbProvider;
        IUIDialogPlugin _dialogPlugin;
        //IDevice _device;
        ICustomSecurityProvider _securityProvider;
        IPaymentService _paymentService;
        IGoogleAnalyticsService _iGoogleAnalyticsService;

        #endregion

        #region Constructor Implementation

        public LoginAuthViewModel(
            IAuthService authService,
            IUserService userService,
            ILocalDbProvider dbProvider,
            IUIDialogPlugin dialogPlugin,
            IPaymentService paymentService,
            IDevice device,
            ICustomSecurityProvider securityProvider,
            IGoogleAnalyticsService iGoogleAnalyticsService)
        {
            _paymentService = paymentService;
            _dialogPlugin = dialogPlugin;
            _authService = authService;
            _userService = userService;
            _dbProvider = dbProvider;
            base._device = device;
            _securityProvider = securityProvider;
            _iGoogleAnalyticsService = iGoogleAnalyticsService;
            //			using (ILocalDbProvider db = Mvx.IoCProvider.Resolve<ILocalDbProvider> ()) {
            //				
            //			}

            var user = _dbProvider.Get<AutoCompleteFields>().FirstOrDefault();
            if (user != null)
            {
                UserName = user.UserEmail;
            }

#if DEBUG
			UserName = "smamuchishvili@gmail.com";
			Password = "bulvari111";
#else
#endif
        }

        #endregion

        #region Properties

        private string _userId;
        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }

        #endregion

        #region Commands

        private ICommand _forgotPassword;

        public ICommand ForgotPassword
        {
            get
            {
                _forgotPassword = _forgotPassword ?? new MvvmCross.Commands.MvxCommand(ResetPassword);
                return _forgotPassword;
            }
        }

        private ICommand _authCommand;

        public ICommand AuthCommand
        {
            get
            {
                _authCommand = _authCommand ?? new MvvmCross.Commands.MvxCommand(Auth);
                return _authCommand;
            }
        }

        private ICommand _backCommand;

        public ICommand BackCommand
        {
            get
            {
                return new MvvmCross.Commands.MvxCommand(() =>
                {
                    NavigationCommand<LoginViewModel>();
                });
            }
        }

        #endregion

        #region Methods

        private void ResetPassword()
        {
            if (_device.Platform == "ios")
            {
                NavigationCommand<iResetPasswordViewModel>(new { email = this.UserName });
            }
            else
            {
                NavigationCommand<BaseResetPasswordViewModel>(new { email = this.UserName });
            }
        }

        public void Auth()
        {
            ShouldValidateModel = true;

            if (string.IsNullOrWhiteSpace(_userName) || string.IsNullOrWhiteSpace(_password))
            {
                return;
            }
            InvokeOnMainThread(() =>
            {
                _dialogPlugin.ShowProgressDialog(ApplicationStrings.Authorizing);
            });
            Task.Run(() =>
            {
                var response = _authService.Auth(_userName, _password, null);

                if (response.Success)
                {
                    _userId = response.Result.UserId;
                    _securityProvider.SaveCredentials(_userId, UserName, Password, response.Result.SessionId);
                    SaveLoggedInUserInfo();
                    Task.Run(() =>
                    {
                        FillDeliveryTypes();
                    });
                    if (_device.Platform == "ios")
                    {
                        NavigationCommand<RootViewModel>(new { auth = true }, true);
                        InvokeOnMainThread(() =>
                        {
                            _dialog.DismissProgressDialog();
                        });
                    }
                    else
                    {
                        try
                        {
                            NavigationCommand<MainViewModel>(new { auth = true });
                        }
                        catch (Exception ex)
                        {
                            _dialog.ShowToast("ავტორიზაცია ვერ მოხერხდა, სცადეთ კიდევ ერთხელ");
                            throw;
                        }
                    }
                    if (_iGoogleAnalyticsService != null)
                    {
                        _iGoogleAnalyticsService.TrackEvent(GAServiceHelper.Events.Authorization, GAServiceHelper.Events.Authorization);
                    }
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        _dialogPlugin.DismissProgressDialog();
                        if (!string.IsNullOrEmpty(response.DisplayMessage))
                        {
                            _dialog.ShowToast(response.DisplayMessage);
                        }
                        else
                        {
                            _dialog.ShowToast("შეუძლებელია ავტორიზაცია, გთხოვთ სცადოთ მოგვიანებით.");
                        }
                    });
                }
            });
        }

        private void FillDeliveryTypes()
        {
            var data = _dbProvider.Get<DeliveryMethod>();
            if (data.Count == 0)
            {
                var methods = _paymentService.GetDeliveryMethods();
                if (methods != null && methods.Result != null)
                {
                    _dbProvider.Insert<DeliveryMethod>(methods.Result.Result);
                }
            }

        }

        public void DismissDialog()
        {
            InvokeOnMainThread(() => _dialogPlugin.DismissProgressDialog());
        }

        private void SaveLoggedInUserInfo()
        {
            var users = _dbProvider.Get<UserInfo>();
            foreach (var item in users)
            {
                _dbProvider.Delete<UserInfo>(item);
            }

            UserInfo newUser = new UserInfo() { UserId = _userId, Username = _userName };

            var userFromUnicard = _userService.GetUserInfoByUserId(_userId);
            if (userFromUnicard != null && userFromUnicard.Success)
            {
                newUser.FirstName = userFromUnicard.Result.FirstName;
                newUser.LastName = userFromUnicard.Result.LastName;
                newUser.Address = userFromUnicard.Result.Address;
                newUser.FullAddress = userFromUnicard.Result.FullAddress;
                string phone = userFromUnicard.Result.Phone;
                if (phone.Length >= 12)
                {
                    phone = phone.Substring(3, phone.Length - 3);
                }
                newUser.Phone = phone;
                newUser.PersonalId = userFromUnicard.Result.PersonalNumber;
            }

            var balance = _userService.GetUserBalance(_userId);
            if (balance != null && balance.Success)
            {
                newUser.Balance_AccumulatedPoint = balance.Result.AccumulatedPoint;
                newUser.Balance_AvailablePoints = balance.Result.AvailablePoints;
                newUser.Balance_BlockedPoints = balance.Result.BlockedPoints;
                newUser.Balance_SpentPoints = balance.Result.SpentPoints;
            }

            var virtualCardNumber = _userService.GetVirtualCard(_userId);
            if (virtualCardNumber != null && virtualCardNumber.Success)
            {
                newUser.VirtualCardNumber = virtualCardNumber.Result.CardNumber;
            }
            _dbProvider.Insert<UserInfo>(newUser);
        }

        #endregion
    }
}

