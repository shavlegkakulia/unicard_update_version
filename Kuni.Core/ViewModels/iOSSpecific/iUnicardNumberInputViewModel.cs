using System;
using Kuni.Core.Services.Abstract;
using System.Windows.Input;
using MvvmCross.ViewModels;
using Kuni.Core.Models;
using Kuni.Core.Models.DataTransferObjects;
using System.Collections.Generic;
using MvvmCross;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kuni.Core.ViewModels.iOSSpecific
{
    public class iUnicardNumberInputViewModel : BaseViewModel
    {
        IUserService _userService;
        ITransactionsService _transactionService;

        public iUnicardNumberInputViewModel(ISmsVerifycationService smsVerificationService, IUserService userService, ITransactionsService transactionService)
        {
            _userService = userService;
            _transactionService = transactionService;
        }

        TransferUserModel _fbUser;

        public void Init(TransferUserModel fbUser)
        {
            if (fbUser != null && !string.IsNullOrWhiteSpace(fbUser.FBId))
            {
                _fbUser = fbUser;
            }
        }

        #region Card Parts Control

        private string _part1;

        public string Part1
        {
            get { return _part1; }
            set
            {
                _part1 = value;
                RaisePropertyChanged(() => Part1);
                UpdateCard();
            }
        }

        private string _part2;

        public string Part2
        {
            get { return _part2; }
            set
            {
                _part2 = value;
                RaisePropertyChanged(() => Part2);
                UpdateCard();
            }
        }

        private string _part3;

        public string Part3
        {
            get { return _part3; }
            set
            {
                _part3 = value;
                RaisePropertyChanged(() => Part3);
                UpdateCard();
            }
        }

        private string _part4;

        public string Part4
        {
            get { return _part4; }
            set
            {
                _part4 = value;
                RaisePropertyChanged(() => Part4);
                UpdateCard();
            }
        }

        private string _cardNumber;

        public string CardNumber
        {
            get { return _cardNumber; }
            set
            {
                _cardNumber = value;
                RaisePropertyChanged(() => CardNumber);
            }
        }

        private void UpdateCard()
        {
            CardNumber = Part1 + Part2 + Part3 + Part4;
            //CardNumber = "";
            //CardNumber = string.Concat(Part1,Part2,Part3,Part4);
        }

        #endregion

        private ICommand _continueCommand;

        public ICommand ContinueCommand
        {
            get
            {
                _continueCommand = _continueCommand ?? new MvvmCross.Commands.MvxCommand(Continue);
                return _continueCommand;
            }
        }

        //public ICommand BackCommand
        //{
        //    get { return new MvvmCross.Commands.MvxCommand(() => { Mvx.IoCConstruct<RegisterViewModel>(); }); }
        //}

        private CardStatusModel _unicardStatus;

        public CardStatusModel UnicardStatus
        {
            get { return _unicardStatus; }
            set { _unicardStatus = value; }
        }

        private string _userPhoneNumber;

        public string UserPhoneNumber
        {
            get { return _userPhoneNumber; }
            set { _userPhoneNumber = value; }
        }

        private List<Merchant> _merchants = new List<Merchant>();

        public List<Merchant> Merchants
        {
            get { return _merchants; }
            set { _merchants = value; }
        }

        private string _retrievedUserPhoneNumber;

        public string RetrievedUserPhoneNumber
        {
            get { return _retrievedUserPhoneNumber; }
            set { _retrievedUserPhoneNumber = value; }
        }

        private bool Validation()
        {
            if (string.IsNullOrWhiteSpace(_part1) || string.IsNullOrWhiteSpace(_part2)
                || string.IsNullOrWhiteSpace(_part3) || string.IsNullOrWhiteSpace(_part4))
            {
                return false;
            }
            if (_part1.Length != 4
                || _part2.Length != 4
                || _part3.Length != 4
                || _part4.Length != 4)
            {
                return false;
            }
            return true;
        }

        private void Continue()
        {
            InvokeOnMainThread(() => _dialog.ShowProgressDialog(""));
            Task.Run(async () =>
            {
                ShouldValidateModel = true;
                if (!Validation())
                {
                    InvokeOnMainThread(() => _dialog.DismissProgressDialog());
                    return;
                }

                BaseActionResult<CardStatusModel> status = _userService.GetCardStatus(_cardNumber);
                string message = string.Empty;
                if (!string.IsNullOrWhiteSpace(status.DisplayMessage))
                {
                    message = status.DisplayMessage;
                }
                if (status.Success)
                {
                    Merchants = new List<Merchant>();
                    _unicardStatus = status.Result;
                    if (_unicardStatus.CardIsValid)
                    {
                        if (_unicardStatus.ExistsUserData)
                        {
                            var userInfo = _userService.GetUserInfoByCard(_cardNumber);
                            _retrievedUserPhoneNumber = userInfo.Result.PhoneNumber;
                            InvokeOnMainThread(() => _dialog.DismissProgressDialog());
                            NavigationCommand<iSMSVerificationViewModel>(new iSMSVerificationParams()
                            {
                                PhoneNumberRetrieved = true,
                                PhoneNumber = _retrievedUserPhoneNumber,
                                UnicardNumber = _cardNumber,
                                EmailRegistration = (_fbUser == null),
                                NewCardRegistration = false,
                                FBUser = JsonConvert.SerializeObject(_fbUser),
                                FacebookRegistration = (_fbUser != null)
                            });
                            message = status.DisplayMessage;
                        }
                        else if (_unicardStatus.HasTransactions)
                        {
                            var lastTransactions = _transactionService.GetTransactions(_cardNumber);
                            var retrievedMerchants = lastTransactions.Result.Merchants;
                            message = status.DisplayMessage;
                            if (retrievedMerchants != null)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    var tempMerchant = new Merchant(
                                                           retrievedMerchants[i].MerchantName,
                                                           retrievedMerchants[i].MerchantId
                                                       );
                                    Merchants.Add(tempMerchant);
                                }
                            }
                            InvokeOnMainThread(() =>
                            {
                                _dialog.DismissProgressDialog();
                            });
                            var tranParam = new iTransactionVerificationViewModelParams()
                            {
                                UnicardNumber = _cardNumber,
                                Merchants = JsonConvert.SerializeObject(this.Merchants),
                                FBUser = JsonConvert.SerializeObject(_fbUser)
                            };
                            NavigationCommand<iTransactionVerificationViewModel>(tranParam);
                        }
                        else if (_unicardStatus.CardIsValid && !_unicardStatus.ExistsUserData && !_unicardStatus.HasTransactions)
                        {
                            InvokeOnMainThread(() =>
                            {
                                _dialog.ShowToast(ApplicationStrings.ValidWithNoTransactions);
                            });
                        }
                    }
                }
                InvokeOnMainThread(() => _dialog.DismissProgressDialog());
                if (!string.IsNullOrWhiteSpace(message))
                {
                    InvokeOnMainThread(() => _dialog.ShowToast(message));
                }
            });

        }
    }
}

