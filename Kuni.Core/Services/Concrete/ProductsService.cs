using System;
using Kuni.Core.Services.Abstract;
using Kuni.Core.Models;
using Newtonsoft.Json;
using Kuni.Core.UnicardApiProvider;
using Kuni.Core.Helpers.AppSettings;
using Kuni.Core.Models.DataTransferObjects;
using System.Linq;
using System.Threading.Tasks;
using Kuni.Core.Models.DB;
using System.Collections.Generic;
using Kuni.Core.Models.DataTransferObjects.Response;

namespace Kuni.Core.Services.Concrete
{
    public class ProductsService : IProductsService
    {
        private IUnicardApiProvider _apiProvider;
        private IAppSettings _appSettings;

        public ProductsService(
            IUnicardApiProvider unicardApiProvider,
            IAppSettings appSettings)
        {
            _apiProvider = unicardApiProvider;
            _appSettings = appSettings;
        }

        public BaseActionResult<CheckVersionResponse> CheckVersionNumber()
        {
            var result = new BaseActionResult<CheckVersionResponse>();
            var url = string.Format("{0}CheckVersion", _appSettings.UnicardServiceUrl);
            var json = JsonConvert.SerializeObject(new UnicardApiBaseRequest(),
                           Formatting.None,
                           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var response = _apiProvider.Post<CheckVersionResponse>(url, null, json).Result;
            result.Success = response.Successful;
            result.DisplayMessage = response.DisplayMessage;
            result.Result = new CheckVersionResponse
            {
                Versions = new List<VersionsModel>()
            };
            if (response.Versions != null)
            {
                result.Result.Versions = response.Versions;
            }
            return result;
        }

        public async Task<BaseActionResult<ProductCategoryModel>> GetProductCategoryList()
        {
            BaseActionResult<ProductCategoryModel> result = new BaseActionResult<ProductCategoryModel>();

            var url = string.Format("{0}GetPrizeCategoryList", _appSettings.UnicardServiceUrl);
            var json = JsonConvert.SerializeObject(new UnicardApiBaseRequest(),
                           Formatting.None,
                           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var response = _apiProvider.Post<GetProductCategoryListResponse>(url, null, json).Result;
            result.Success = response.Successful;
            result.DisplayMessage = response.DisplayMessage;
            result.Result = new ProductCategoryModel();
            result.Result.Categories = new System.Collections.Generic.List<CategoryModel>();
            if (response.Categories != null)
            {
                result.Result.Categories =
                response.Categories.Select(x => new CategoryModel
                {
                    CategoryID = x.CategoryID,
                    CategoryName = x.CategoryName,
                    IsHidden = x.IsHidden
                }).ToList();
            }
            return result;
        }

        public async Task<BaseActionResult<ProductsListModel>> GetProductList(int? pageIndex,
                                                                               int? rowCount,
                                                                               int? recentProducts,
                                                                               int? categoryID,
                                                                               int? subCategoryID,
                                                                               int? brandID,
                                                                               int? customerTypeID,
                                                                               int? deliveryMethodID,
                                                                               int? productTypeID,
                                                                               bool? specialOffers,
                                                                               int? discounted,
                                                                               int? rangeId,
                                                                               string name,
                                                                               int _userId)
        {
            BaseActionResult<ProductsListModel> result = new BaseActionResult<ProductsListModel>();

            var url = string.Format("{0}GetProductList", _appSettings.UnicardServiceUrl);
            var request = new GetProductListRequest()
            {
                PageIndex = pageIndex,
                RowCount = rowCount,
                CategoryID = categoryID,
                SubCategoryID = subCategoryID,
                BrandID = brandID,
                CustomerTypeID = customerTypeID,
                DeliveryMethodID = deliveryMethodID,
                SpecialOffers = specialOffers,
                Discounted = discounted,
                PriceRangeId = rangeId,
                Name = name,
                UserId = _userId
            };

            var json = JsonConvert.SerializeObject(request,
                           Formatting.None,
                           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var response = _apiProvider.Post<GetProductListResponse>(url, null, json).Result;
            result.Success = response.Successful;
            result.DisplayMessage = response.DisplayMessage;
            result.Result = new ProductsListModel();
            result.Result.Products = new System.Collections.Generic.List<ProductsInfo>();
            if (response.Products != null)
            {
                result.Result.Products =
                    response.Products.Select(x => new ProductsInfo()
                    {
                        ProductID = x.ProductID,
                        ProductName = x.ProductName,
                        ImageUrls = x.ImageURLs,
                        DiscountPrice = x.DiscountPrice.HasValue ? x.DiscountPrice.Value : 0,
                        DiscountPercent = x.DiscountPercent.HasValue ? x.DiscountPercent.Value : 0,
                        ProductPrice = x.ProductPrice.HasValue ? x.ProductPrice.Value : 0,
                        CategoryID = x.CategoryID,
                        BrandID = x.BrandID,
                        CustomerTypeID = x.CustomerTypeID,
                        ImageURL = x.ImageURLs[0]
                    }).ToList();
            }
            return result;
        }

        public async Task<BaseActionResult<DetailedProductModel>> GetProductByID(int productID, int userId)
        {
            BaseActionResult<DetailedProductModel> result = new BaseActionResult<DetailedProductModel>();
            var request = new GetProductByIDRequest()
            {
                ProductID = productID,
                UserId = userId
            };
            var url = string.Format("{0}GetProductById", _appSettings.UnicardServiceUrl);
            var json = JsonConvert.SerializeObject(request,
                           Formatting.None,
                           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var response = _apiProvider.Post<GetProductByIDResponse>(url, null, json).Result;
            result.Success = response.Successful;
            result.DisplayMessage = response.DisplayMessage;
            var userDiscounts = response.UserDiscounts;
            result.Result = new DetailedProductModel()
            {
                BrandID = response.BrandID,
                CatalogID = response.CatalogID,
                ProductName = response.ProductName,
                PoductDescription = response.PoductDescription,
                CategoryID = response.CategoryID,
                SubCategoryID = response.SubCategoryID,
                ProductTypeID = response.ProductTypeID,
                ProductImages = response.ProductImages,
                ProductPrice = response.ProductPrice ?? 0,
                DiscountedPrice = response.DiscountedPrice ?? 0,
                DiscountedPercent = response.DiscountedPercent ?? 0,
                DeliveryMethods = response.DeliveryMethods
            };
            if (userDiscounts != null)
                result.Result.UserDiscounts = userDiscounts.Select(x => new DiscountModel()
                {
                    DiscountDescription = x.DiscountDescription,
                    DiscountPercent = x.DiscountedPercent,
                    DiscountID = x.DiscountID
                }).ToList();
            return result;
        }

        public async Task<BaseActionResult<List<PriceRange>>> GetPriceRange()
        {
            BaseActionResult<List<PriceRange>> result = new BaseActionResult<List<PriceRange>>();
            var url = string.Format("{0}GetProductPriceRange", _appSettings.UnicardServiceUrl);
            var json = JsonConvert.SerializeObject(new UnicardApiBaseRequest(),
                           Formatting.None,
                           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var response = _apiProvider.Post<PriceRangeDTO>(url, null, json).Result;
            result.Success = response.Successful;
            result.DisplayMessage = response.DisplayMessage;
            result.Result = response.PriceRanges;

            return result;
        }
    }
}

