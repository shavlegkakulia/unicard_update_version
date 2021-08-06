using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kuni.Core.Models;
using System.Windows.Input;
using MvvmCross.ViewModels;
using Kuni.Core.Services.Abstract;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Models.DB;
using Newtonsoft.Json.Schema;

namespace Kuni.Core.ViewModels.iOSSpecific
{
    public class iCatalogListViewModel : BaseViewModel
    {
        #region Private variables

        private IProductsService _productsService;
        private ILocalDbProvider _localDBProvider;
        private IUserService _userService;
        private IPaymentService _paymentService;
        private IGoogleAnalyticsService _gaService;

        #endregion

        #region Properties

        public bool IsLoading
        {
            get;
            set;
        }

        public List<PriceRange> PriceRanges
        {
            get;
            set;
        }

        public List<UserTypesInfo> UserTypes
        {
            get;
            set;
        }

        public List<ProductCategoryInfo> ProductCategories
        {
            get;
            set;
        }

        private List<ProductsInfo> _products;

        public List<ProductsInfo> Products
        {
            get
            {
                return _products;
            }
            set
            {
                _products = value;
                RaisePropertyChanged(() => Products);
            }
        }

        private bool _dataPopulated;

        public bool DataPopulated
        {
            get
            {
                return _dataPopulated;
            }
            set
            {
                _dataPopulated = value;
                RaisePropertyChanged(() => DataPopulated);
            }
        }


        private int _rowCount = 20;

        public int RowCount { get { return _rowCount; } set { _rowCount = value; } }

        private int _pageindex;

        public int PageIndex
        {
            get { return _pageindex; }
            set { _pageindex = value; }
        }

        private string _displayText;

        public string DisplayText
        {
            get { return _displayText; }
            set { _displayText = value; }
        }

        private ProductFilterInfo _productFilterInfo;

        public ProductFilterInfo FilterInfo
        {
            get
            {
                return _productFilterInfo;
            }
            set
            {
                _productFilterInfo = value;
                GetOrFilterProductList(true);
            }
        }

        #endregion

        #region Constructor Implementation

        public iCatalogListViewModel(IProductsService productService,
                                      ILocalDbProvider localDbProvider,
                                      IPaymentService paymentService,
                                      IUserService userService,
                                      IGoogleAnalyticsService gaService)
        {
            _localDBProvider = localDbProvider;
            _productsService = productService;
            _userService = userService;
            _paymentService = paymentService;
            _productFilterInfo = new ProductFilterInfo();
            _gaService = gaService;
            PageIndex = 1;

            GetOrFilterProductList(true);

            //			Task.Run (() => {
            //				FillDeliveryTypes ();
            //			});
        }

        #endregion

        #region Methods

        //		private void FillDeliveryTypes ()
        //		{
        //			var data = _localDBProvider.Get<DeliveryMethod> ();
        //			if (data.Count == 0) {
        //				var methods = _paymentService.GetDeliveryMethods ();
        //				if (methods != null && methods.Result != null) {
        //					_localDBProvider.Insert<DeliveryMethod> (methods.Result.Result);
        //				}
        //			}
        //
        //		}

        public void LoadMore()
        {
            Task.Run(async () =>
            {
                UserInfo currentUser = _localDBProvider.Get<UserInfo>().FirstOrDefault();
                var productResponse = await _productsService.GetProductList(
                                          PageIndex,
                                          RowCount,
                                          (FilterInfo.LastAdded == true) ? 1 : (int?)null,
                                          FilterInfo.CategoryId,
                                          null,
                                          null,
                                          FilterInfo.UserTypeId,
                                          null,
                                          null,
                                          null,
                                          (FilterInfo.Discounted == true) ? 1 : (int?)null,
                                          FilterInfo.PointRangeId,
                                          FilterInfo.SearchCriteria ?? "",
                                          int.Parse(currentUser.UserId));
                if (productResponse.Success)
                {
                    if (productResponse.Result != null)
                    {
                        Products.AddRange(productResponse.Result.Products);
                        DataPopulated = true;
                    }
                }
            });
        }

        public void GetFilterFields()
        {
            // UserTypes
            if (_localDBProvider.Get<UserTypesInfo>().Count == 0)
            {
                var userTypesResponse = _userService.GetUserTypeList();
                if (userTypesResponse.Result.Success)
                {
                    if (userTypesResponse.Result.Result != null && userTypesResponse.Result.Result.UserTypes != null)
                    {
                        var usertypeModels = userTypesResponse.Result.Result.UserTypes;
                        var listToStore = new List<UserTypesInfo>();
                        listToStore =
                        usertypeModels.Select(x => new UserTypesInfo
                        {
                            UserTypeId = x.UserTypeID,
                            UserTypeName = x.UserTypeName
                        }).ToList();
                        if (_localDBProvider.Get<UserTypesInfo>().Count == 0)
                            _localDBProvider.Insert<UserTypesInfo>(listToStore);

                        UserTypes = listToStore;
                    }
                }
            }
            else
            {
                UserTypes = _localDBProvider.Get<UserTypesInfo>();
            }

            var categoruIsUpTodate = CategoryIsUpToDate(out int categoryVersion);

            // Categories
            if (_localDBProvider.Get<ProductCategoryInfo>().Count == 0 || !categoruIsUpTodate)
            {
                var categoryListResponse = _productsService.GetProductCategoryList();
                if (categoryListResponse.Result.Success && categoryListResponse.Result.Result != null)
                {
                    var categorymodels = categoryListResponse.Result.Result.Categories;
                    var listToStore = new List<ProductCategoryInfo>();
                    listToStore =
                    categorymodels.Select(x => new ProductCategoryInfo
                    {
                        CategoryID = x.CategoryID,
                        CategoryName = x.CategoryName,
                        IsHidden = x.IsHidden
                    }).ToList();

                    if (_localDBProvider.Get<ProductCategoryInfo>().Count == 0)
                    {
                        _localDBProvider.Insert<ProductCategoryInfo>(listToStore);
                    }
                    else
                    {
                        _localDBProvider.Execute("delete from ProductCategoryInfo");
                        _localDBProvider.Insert<ProductCategoryInfo>(listToStore);
                    }

                    ProductCategories = listToStore;

                    if (!categoruIsUpTodate)
                    {
                        var versionInfo = _localDBProvider.Get<VersionsModel>().FirstOrDefault(v => v.Type == VersionType.category);
                        if (versionInfo == null)
                        {
                            versionInfo = new VersionsModel
                            {
                                Type = VersionType.category,
                                Version = categoryVersion
                            };
                            _localDBProvider.Insert(versionInfo);
                        }
                        else
                        {
                            versionInfo.Version = categoryVersion;
                            _localDBProvider.Update(versionInfo);
                        }
                    }
                }
            }
            else
            {
                ProductCategories = _localDBProvider.Get<ProductCategoryInfo>();
            }

            // Price ranges
            PriceRanges = _productsService.GetPriceRange().Result.Result;
        }

        private bool CategoryIsUpToDate(out int serverVersion)
        {
            serverVersion = 0;
            var categoryVersion = _localDBProvider.Get<VersionsModel>().FirstOrDefault(v => v.Type == VersionType.category);

            var verionsInfo = _productsService.CheckVersionNumber();
            if (verionsInfo != null)
            {
                var categoryServerVersion = verionsInfo.Result.Versions.FirstOrDefault(v => v.Type == VersionType.category);
                if (categoryServerVersion != null)
                {
                    serverVersion = categoryServerVersion.Version;
                    return serverVersion <= categoryVersion?.Version;
                }
                return true;
            }
            return true;
        }

        public void ProductClick(ProductsInfo product)
        {
            _gaService.TrackScreen(GAServiceHelper.Page.CatalogDetail);
            NavigationCommand<iCatalogDetailViewModel>(new { productId = product.ProductID });
        }

        public void GetOrFilterProductList(bool showDialog)
        {
            if (showDialog)
                InvokeOnMainThread(() =>
                {
                    _dialog.ShowProgressDialog(ApplicationStrings.Loading);
                });
            Task.Run((async () =>
            {
                PageIndex = 1;
                try
                {
                    DisplayText = string.Empty;
                    UserInfo currentUser = _localDBProvider.Get<UserInfo>().FirstOrDefault();
                    var productResponse = await _productsService.GetProductList(
                                              PageIndex,
                                              RowCount,
                                              (FilterInfo.LastAdded == true) ? 1 : (int?)null,
                                              FilterInfo.CategoryId,
                                              null,
                                              null,
                                              FilterInfo.UserTypeId,
                                              null,
                                              null,
                                              null,
                                              (FilterInfo.Discounted == true) ? 1 : (int?)null,
                                              FilterInfo.PointRangeId,
                                              FilterInfo.SearchCriteria ?? "",
                                              int.Parse(currentUser.UserId));
                    if (showDialog)
                        InvokeOnMainThread(() =>
                        {
                            _dialog.DismissProgressDialog();
                        });
                    if (productResponse.Success)
                    {
                        if (productResponse.Result != null)
                        {
                            Products = productResponse.Result.Products;
                            DataPopulated = true;
                        }
                    }
                    else
                    {
                        DisplayText = productResponse.DisplayMessage;
                    }
                }
                catch (Exception ex)
                {
                    DisplayText = ex.ToString();
                    if (showDialog)
                        _dialog.DismissProgressDialog();
                }
               ;
            }));
        }

        #endregion

    }

    public class ProductFilterInfo
    {
        public bool? Discounted
        {
            get;
            set;
        }

        public bool? LastAdded
        {
            get;
            set;
        }

        public int? CategoryId
        {
            get;
            set;
        }

        public int? UserTypeId
        {
            get;
            set;
        }

        public int? PointRangeId
        {
            get;
            set;
        }

        public string SearchCriteria
        {
            get;
            set;
        }

        public bool IsFiltered
        {
            get
            {
                return (Discounted == true) || (LastAdded == true) || (CategoryId.HasValue) || (UserTypeId.HasValue) || (PointRangeId.HasValue)
                || !string.IsNullOrWhiteSpace(SearchCriteria);
            }
        }
    }
}

