using Kuni.Core.Helpers.Device;
using Kuni.Core.Models.DB;
using Kuni.Core.Providers.LocalDBProvider;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Xml.Linq;
using Kuni.Core.Services.Abstract;
using System.Windows.Input;
using MvvmCross.ViewModels;
using Kuni.Core.ViewModels.iOSSpecific;
using Kuni.Core.Models;
using MvvmCross;
//using MvvmCross;

namespace Kuni.Core.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private ILocalDbProvider _dbProvider;
        private IProductsService _productService;
        private INewsService _newsService;
        private IGoogleAnalyticsService _gaService;
        private IUserService _userService;

        public void Init(bool auth = false)
        {
            this.UserAuthed = auth;
        }

        #region Methods

        public bool PinIsCorrect(string pin)
        {
            GetUserSettings();
            if (UserSettings.Pin != null && UserSettings.Pin.Equals(pin))
                return true;
            else {
                _dialog.ShowToast(ApplicationStrings.incorrect_pin);
                return false;
            }
        }

        public void PinIncorrectToast()
        {
            _dialog.ShowToast(ApplicationStrings.repeated_pin_incorrect);
        }

        public SettingsInfo GetUserSettings()
        {
            UserSettings = _dbProvider.Get<SettingsInfo>().Where(x => x.UserId == Convert.ToInt32(UserId)).FirstOrDefault();
            return UserSettings;
        }

        public void InsertSettingsInfo(bool? location, bool? push, bool? pinIsSet, string pin)
        {
            _dbProvider.Execute("Delete from SettingsInfo");
            _dbProvider.Insert<SettingsInfo>(new SettingsInfo()
            {
                UserId = Convert.ToInt32(UserId),
                LocationEnabled = location,
                PushNotificationEnabled = push,
                PinIsSet = pinIsSet,
                Pin = pin
            });
            _gaService.TrackEvent(GAServiceHelper.From.FromDialog, GAServiceHelper.Events.SetPin);
        }

        #endregion

        public MainViewModel(IUserService userService, ILocalDbProvider dbProvider, IProductsService productService, INewsService newsService, IGoogleAnalyticsService gaService)
        {
            _productService = productService;
            _dbProvider = dbProvider;
            _newsService = newsService;
            _gaService = gaService;
            _userService = userService;
            UserId = _dbProvider.Get<UserInfo>().FirstOrDefault().UserId;
            Task.Run(() =>
            {
                UserSettings = _dbProvider.Get<SettingsInfo>().Where(x => x.UserId == Convert.ToInt32(UserId)).FirstOrDefault();
                var device = Mvx.IoCProvider.Resolve<IDevice>();
                if (device.Platform == "ios")
                {
                    GetAlerts();
                    GetProducts(null, 1);
                }
                InitCard();
            });
        }

        private List<ProductsInfo> _products;

        public List<ProductsInfo> Products
        {

            get { return _products; }
            set
            {
                _products = value;
                RaisePropertyChanged(() => Products);
            }
        }

        private ICommand _itemClick;

        public ICommand ItemClick
        {
            get
            {
                _itemClick = _itemClick ?? new MvvmCross.Commands.MvxCommand<ProductsInfo>(ProductClick);
                return _itemClick;
            }
        }

        private bool _dataPopulated;

        public bool DataPopulated
        {
            get { return _dataPopulated; }
            set
            {
                _dataPopulated = value;
                RaisePropertyChanged(() => DataPopulated);
            }
        }

        private void ProductClick(ProductsInfo product)
        {
            _gaService.TrackEvent(GAServiceHelper.From.FromHomePage, GAServiceHelper.Events.CatalogDetailClicked);
            NavigationCommand<iCatalogDetailViewModel>(new { productId = product.ProductID });
        }

        public void GetProducts(int? categoryId, int? lastModified)
        {
            Task.Run((async () =>
            {
                if (_dbProvider.GetCount<ProductsInfo>() > 0)
                {
                    Products = _dbProvider.Query<ProductsInfo>("Select * from ProductsInfo Limit 4;");
                    DataPopulated = true;
                }

                UserInfo currentUser = _dbProvider.Get<UserInfo>().FirstOrDefault();
                var productResponse = await _productService.GetProductList(
                                          1,
                                          4,
                                          lastModified,
                                          categoryId,
                                          null,
                                          null,
                                          null,
                                          null,
                                          null,
                                          null,
                                          null,
                                          null,
                                          null,
                                          int.Parse(currentUser.UserId));
                if (productResponse.Success)
                {
                    if (productResponse.Result != null)
                    {
                        Products = productResponse.Result.Products;
                    }
                    _dbProvider.Execute("delete from ProductsInfo");
                    _dbProvider.Insert<ProductsInfo>(Products);
                }
            }));
        }

        public void GetAlerts()
        {
            var news = _dbProvider.Query<NewsInfo>("Select * from NewsInfo where IsRead=0 and UserId=" + UserId);
            if (news != null)
                NewsCount = news.ToList().Count;
            Task.Run(async () =>
            {
                BaseActionResult<List<NewsInfo>> response = new BaseActionResult<List<NewsInfo>>();
                response = await _newsService.GetNews(UserId);
                if (response.Success && response.Result.Count > 0)
                {
                    var tmpNews = response.Result.OrderByDescending(x => x.CreateDate).ToList();
                    var oldReadNewsId = _dbProvider.Query<NewsInfo>("Select * from NewsInfo where IsRead=1");
                    for (int i = 0; i < tmpNews.Count; i++)
                    {
                        tmpNews[i].UserId = UserId;
                        for (int j = 0; j < oldReadNewsId.Count; j++)
                        {
                            if (tmpNews[i].Id == oldReadNewsId[j].Id)
                            {
                                tmpNews[i].IsRead = true;
                                break;
                            }
                        }
                    }
                    //					tmpNews.OrderByDescending (x => x.CreateDate).ToList ();
                    _dbProvider.Execute("Delete from NewsInfo");
                    _dbProvider.Insert<NewsInfo>(tmpNews.OrderByDescending(x => x.CreateDate).ToList());

                    news = _dbProvider.Query<NewsInfo>("Select * from NewsInfo where IsRead=0 and UserId=" + UserId);
                    NewsCount = news.ToList().Count;
                }
            });
        }

        public string UserId { get; set; }

        private void UpdateVirtualCard()
        {
            var result = _userService.GetVirtualCard(UserId);
            if (result.Success)
            {
                CardNumber = result.Result.CardNumber;
            }
        }

        public void UpdatePoints()
        {
            var result = _userService.GetUserBalance(UserId);
            if (result.Success)
            {
                Points = Math.Round(result.Result.AvailablePoints, 1);
            }
        }

        void InitCard()
        {
            var user = _dbProvider.Get<UserInfo>().FirstOrDefault();
            Points = user.Balance_AvailablePoints;
            CardNumber = user.VirtualCardNumber;

            //			UpdateVirtualCard ();
            UpdatePoints();
        }

        public SettingsInfo UserSettings
        {
            get;
            set;
        }

        private bool _userAuthed;

        public bool UserAuthed
        {
            get { return _userAuthed; }
            set { _userAuthed = value; }
        }

        private int _newsCount;

        public int NewsCount
        {
            get { return _newsCount; }
            set
            {
                _newsCount = value;
                RaisePropertyChanged(() => NewsCount);
            }
        }

        private string _cardNumber = "";

        public string CardNumber
        {
            get { return _cardNumber; }
            set
            {
                _cardNumber = value;
                RaisePropertyChanged(() => CardNumber);
            }
        }

        private decimal _points = 0;

        public decimal Points
        {
            get { return _points; }
            set
            {
                _points = value;
                RaisePropertyChanged(() => Points);
            }
        }

        public bool BackPressed { get; set; }

    }
}
