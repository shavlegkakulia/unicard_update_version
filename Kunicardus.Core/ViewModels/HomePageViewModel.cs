using System;
using System.Windows.Input;
using Kunicardus.Core.ViewModels;
using Kunicardus.Core.Services.Abstract;
using System.Threading.Tasks;
using Kunicardus.Core.Providers.LocalDBProvider;
using Kunicardus.Core.Models.DB;
using System.Linq;
using System.Collections.Generic;
using Kunicardus.Core.Models;
using MvvmCross;
using MvvmCross.Platform;
using MvvmCross.Core.ViewModels;

namespace Kunicardus.Core
{
    public class HomePageViewModel : BaseViewModel
    {
        #region Variables

        private IProductsService _productsService;
        private IUserService _userService;
        private INewsService _newsService;
        private UserInfo _user;

        #endregion

        #region Properties

        private bool _alertsUpdated;

        public bool AlertsUpdated
        {
            get { return _alertsUpdated; }
            set
            {
                _alertsUpdated = value;
                RaisePropertyChanged(() => AlertsUpdated);
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

        public void GetProducts(int? categoryId, int? lastModified)
        {
            Task.Run((async () =>
            {
                using (var dbProvider = Mvx.Resolve<ILocalDbProvider>())
                {
                    if (dbProvider.GetCount<ProductsInfo>() > 0)
                    {
                        Products = dbProvider.Query<ProductsInfo>("Select * from ProductsInfo Limit 4;");
                    }

                    UserInfo currentUser = dbProvider.Get<UserInfo>().FirstOrDefault();
                    var productResponse = await _productsService.GetProductList(
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
                            DataPopulated = true;
                            dbProvider.Execute("delete from ProductsInfo");
                            dbProvider.Insert<ProductsInfo>(Products);
                        }
                    }
                }
            }
            ));
        }

        private List<int> _productItemIndexes;

        public List<int> ProductItemIndexes
        {
            get { return _productItemIndexes; }
            set
            {
                _productItemIndexes = value;
                RaisePropertyChanged(() => ProductItemIndexes);
            }
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


        private ICommand _productSelectedCommand;

        public ICommand ProductSelectedCommand
        {
            get
            {
                _productSelectedCommand = _productSelectedCommand ?? new MvxCommand<ProductModel>(ProductSelected);
                return _productSelectedCommand;
            }
        }

        private decimal _acumulatedPoints = 0;

        public decimal AcumulatedPoints
        {
            get { return _acumulatedPoints; }
            set
            {
                _acumulatedPoints = value;
                RaisePropertyChanged(() => AcumulatedPoints);
            }
        }


        private int _newsCount;

        public int NewsCount
        {
            get { return _newsCount; }
            set
            {
                _newsCount = value;
                RaisePropertyChanged(() => NewsCount);
                AlertsUpdated = true;
            }
        }

        #endregion

        #region Constructor Implementation

        public HomePageViewModel(IUserService userService,
                                  IProductsService productService,
                                  INewsService newsService)
        {
            _productsService = productService;
            _userService = userService;
            _newsService = newsService;
            using (var dbProvider = Mvx.Resolve<ILocalDbProvider>())
            {

                _user = dbProvider.Get<UserInfo>().FirstOrDefault();
                if (_user != null)
                {
                    UserId = _user.UserId;
                    CardNumber = _user.VirtualCardNumber;
                    AcumulatedPoints = _user.Balance_AvailablePoints;
                }
                else {
                    UserId = "0";
                    CardNumber = "";
                    AcumulatedPoints = 0;
                }
            }
            GetNewsList();
        }


        #endregion

        #region Messages

        private void ProductSelected(ProductModel e)
        {
            DataChanged = true;
            SelectedProduct = e.ProductID;
        }

        public bool _dataChanged;

        public bool DataChanged
        {
            get { return _dataChanged; }
            set
            {
                _dataChanged = value;
                RaisePropertyChanged(() => DataChanged);
                RaisePropertyChanged(() => EnableClick);
            }
        }

        public bool _enableClick;

        public bool EnableClick
        {
            get { return !_dataChanged; }
            set
            {
                _enableClick = value;
                RaisePropertyChanged(() => EnableClick);
            }
        }

        public int _selectedProduct;

        public int SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                RaisePropertyChanged(() => SelectedProduct);
            }
        }

        #endregion

        #region Methods

        public void UpdatePoints()
        {
            using (var dbProvider = Mvx.Resolve<ILocalDbProvider>())
            {
                var userInfo = dbProvider.Get<UserInfo>().FirstOrDefault();

                var balance = _userService.GetUserBalance(userInfo.UserId);
                if (balance != null && balance.Success)
                {
                    AcumulatedPoints = Math.Round(balance.Result.AvailablePoints, 1);
                }
                else {
                    AcumulatedPoints = Math.Round(userInfo.Balance_AvailablePoints, 1);
                }
            }
        }

        public async void GetAlerts()
        {
            using (var dbProvider = Mvx.Resolve<ILocalDbProvider>())
            {
                var user = _user;
                if (user == null)
                {
                    //					user = dbProvider.Get<UserInfo> ().First ();
                    return;
                }

                BaseActionResult<List<NewsInfo>> response = new BaseActionResult<List<NewsInfo>>();
                response = await _newsService.GetNews(user.UserId);
                if (response.Success && response.Result.Count > 0)
                {
                    var tmpNews = response.Result;
                    var oldReadNewsId = dbProvider.Query<NewsInfo>("Select * from NewsInfo where IsRead=1 and UserId=" + user.UserId);
                    for (int i = 0; i < tmpNews.Count; i++)
                    {
                        tmpNews[i].UserId = user.UserId;
                        for (int j = 0; j < oldReadNewsId.Count; j++)
                        {
                            if (tmpNews[i].Id == oldReadNewsId[j].Id)
                            {
                                tmpNews[i].IsRead = true;
                                break;
                            }
                        }
                    }
                    var news = tmpNews.OrderByDescending(x => x.CreateDate).ToList();

                    NewsCount = news.Count(x => !x.IsRead);
                    dbProvider.Execute("Delete from NewsInfo");
                    dbProvider.Insert<NewsInfo>(news);
                }
            }
        }

        public string UserId
        {
            get;
            set;
        }

        public void GetNewsList()
        {
            Task.Run(() =>
            {
                using (var dbProvider = Mvx.Resolve<ILocalDbProvider>())
                {
                    var user = dbProvider.Get<UserInfo>().First();
                    var newsList = dbProvider.Get<NewsInfo>().Where(x => x.UserId == user.UserId).ToList();
                    if (newsList != null && newsList.Count > 0)
                    {
                        int count = newsList.Count(x => !x.IsRead);
                        if (count > 0)
                        {
                            NewsCount = count;
                        }
                    }
                }
            });
        }

        #endregion
    }
}


