using MvvmCross.Core.ViewModels;
using System.Windows.Input;
using System.Collections.Generic;
using Kunicardus.Core.Models;
using Kunicardus.Core.Services.Abstract;
using System.Threading.Tasks;
using Kunicardus.Core.Providers.LocalDBProvider;
using Kunicardus.Core.Models.DB;
using System.Linq;

namespace Kunicardus.Core
{
    public class BaseCatalogViewModel : MvxViewModel
    {
        #region Private variables

        private IProductsService _productService;
        private IUserService _userService;
        private ILocalDbProvider _localDbProvider;

        #endregion

        #region Constructor Implementation

        public BaseCatalogViewModel(IProductsService productService,
                                     IUserService userService,
                                     ILocalDbProvider localDbProvider)
        {
            _productService = productService;
            _userService = userService;
            _localDbProvider = localDbProvider;

            #region Retrieving Filer Data
            Task.Run(async () =>
            {
                FilterDataModel dataToSendForFilter = new FilterDataModel();

                var localUserTypes = _localDbProvider.Get<UserTypesInfo>();
                var localProductTypes = _localDbProvider.Get<ProductCategoryInfo>();

                #region UserTypes retrieve logic
                if (localUserTypes != null && localUserTypes.Count > 0)
                {
                    _userTypeCategoryList =
                        localUserTypes.Select(x => new UserTypeModel
                        {
                            UserTypeID = x.UserTypeId,
                            UserTypeName = x.UserTypeName
                        }).ToList();
                }
                else
                {
                    var userTypesResponse = await _userService.GetUserTypeList();
                    if (userTypesResponse.Success)
                    {
                        if (userTypesResponse.Result != null && userTypesResponse.Result.UserTypes != null)
                        {
                            _userTypeCategoryList = userTypesResponse.Result.UserTypes;
                            var listToStore = new List<UserTypesInfo>();
                            listToStore =
                                _userTypeCategoryList.Select(x => new UserTypesInfo
                                {
                                    UserTypeId = x.UserTypeID,
                                    UserTypeName = x.UserTypeName
                                }).ToList();
                            if (_localDbProvider.Get<UserTypesInfo>().Count == 0)
                                _localDbProvider.Insert<UserTypesInfo>(listToStore);
                        }
                    }
                }
                #endregion
                #region ProductTypes retrive logic
                var categoruIsUpTodate = CategoryIsUpToDate(out int categoryVersion);

                if (localProductTypes != null && localProductTypes.Count > 0 && categoruIsUpTodate)
                {
                    dataToSendForFilter.ProductsCategory =
                        localProductTypes.Select(x => new CategoryModel
                        {
                            CategoryID = x.CategoryID,
                            CategoryName = x.CategoryName,
                            IsHidden = x.IsHidden
                        }).ToList();
                }
                else
                {
                    var categoryListResponse = await _productService.GetProductCategoryList();
                    if (categoryListResponse.Success && categoryListResponse.Result != null)
                    {
                        dataToSendForFilter.ProductsCategory = categoryListResponse.Result.Categories;
                        var listToStore = new List<ProductCategoryInfo>();
                        listToStore =
                            dataToSendForFilter.ProductsCategory.Select(x => new ProductCategoryInfo
                            {
                                CategoryID = x.CategoryID,
                                CategoryName = x.CategoryName,
                                IsHidden = x.IsHidden
                            }).ToList();
                        if (_localDbProvider.Get<ProductCategoryInfo>().Count == 0)
                        {
                            _localDbProvider.Insert<ProductCategoryInfo>(listToStore);
                        }
                        else
                        {
                            _localDbProvider.Execute("delete from ProductCategoryInfo");
                            _localDbProvider.Insert<ProductCategoryInfo>(listToStore);
                        }

                        if (!categoruIsUpTodate)
                        {
                            var versionInfo = _localDbProvider.Get<VersionsModel>().FirstOrDefault(v => v.Type == VersionType.category);
                            if (versionInfo == null)
                            {
                                versionInfo = new VersionsModel
                                {
                                    Type = VersionType.category,
                                    Version = categoryVersion
                                };
                                _localDbProvider.Insert(versionInfo);
                            }
                            else
                            {
                                versionInfo.Version = categoryVersion;
                                _localDbProvider.Update(versionInfo);
                            }
                        }
                    }
                }
                #endregion

                var resp = await _productService.GetPriceRange();
                _pointsCategoryList = resp.Result;

                dataToSendForFilter.PointsCategory = _pointsCategoryList;
                dataToSendForFilter.UsersTypeCategory = _userTypeCategoryList;
                _dataForFilter = dataToSendForFilter;
                this.FilterDataPopulated = dataToSendForFilter;
            });
            #endregion
        }

        private bool CategoryIsUpToDate(out int serverVersion)
        {
            serverVersion = 0;
            var categoryVersion = _localDbProvider.Get<VersionsModel>().FirstOrDefault(v => v.Type == VersionType.category);

            var verionsInfo = _productService.CheckVersionNumber();
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

        #endregion

        #region Properties

        private FilterDataModel _filterDataPopulated;

        public FilterDataModel FilterDataPopulated
        {
            get { return _filterDataPopulated; }
            set
            {
                _filterDataPopulated = value;
                RaisePropertyChanged(() => FilterDataPopulated);
            }
        }

        private string _recentlyAdded = "ბოლოს დამატებულები";

        public string RecentlyAdded
        {
            get { return _recentlyAdded; }
            set
            {
                _recentlyAdded = value;
                RaisePropertyChanged(() => RecentlyAdded);
            }
        }

        private List<CategoryModel> _drawerItems;

        public List<CategoryModel> DrawerItems
        {
            get { return _drawerItems; }
            set
            {
                _drawerItems = value;
                RaisePropertyChanged(() => DrawerItems);
            }
        }


        private List<UserTypeModel> _userTypeCategoryList;

        public List<UserTypeModel> UserTypeCategoryList
        {
            get { return _userTypeCategoryList; }
            set
            {
                _userTypeCategoryList = value;
                RaisePropertyChanged(() => UserTypeCategoryList);
            }
        }

        private List<PriceRange> _pointsCategoryList;

        public List<PriceRange> PointsCategoryList
        {

            get { return _pointsCategoryList; }
            set
            {
                _pointsCategoryList = value;
                RaisePropertyChanged(() => PointsCategoryList);
            }
        }

        private string _groupName;

        public string GroupName
        {
            get { return _groupName; }
            set
            {
                _groupName = value;
                RaisePropertyChanged(() => GroupName);
            }
        }

        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged(() => SearchText);
            }
        }

        //		private ICommand _searchCommand;
        //
        //		public ICommand SearchCommand {
        //			get {
        //				_searchCommand = _searchCommand ?? new MvxCommand (Search);
        //				return _searchCommand;
        //			}
        //		}

        private ICommand _drawerItemSelectedCommand;

        public ICommand DrawerItemSelectedCommand
        {
            get
            {
                _drawerItemSelectedCommand = _drawerItemSelectedCommand ?? new MvxCommand<CategoryModel>(SelectCategory);
                return _drawerItemSelectedCommand;
            }
        }

        private void SelectCategory(CategoryModel categoryItem)
        {
        }

        //		public void Search ()
        //		{
        //			var message = new MessageForSearch (this, this.SearchText);
        //			_messenger.Publish (message);
        //		}

        #endregion

        #region Message for Filter

        public class FilterDataModel
        {
            public List<UserTypeModel> UsersTypeCategory { get; set; }

            public List<PriceRange> PointsCategory { get; set; }

            public List<CategoryModel> ProductsCategory { get; set; }
        }

        private FilterDataModel _dataForFilter;

        public FilterDataModel DataToSendForFilter
        {
            get { return _dataForFilter; }
            set { _dataForFilter = value; }
        }

        #endregion

    }

}
