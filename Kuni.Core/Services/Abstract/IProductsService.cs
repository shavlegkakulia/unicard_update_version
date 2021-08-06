using System;
using Kuni.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Kuni.Core.Models.DataTransferObjects.Response;

namespace Kuni.Core.Services.Abstract
{
	public interface IProductsService
	{
		Task<BaseActionResult<ProductCategoryModel>> GetProductCategoryList ();

        BaseActionResult<CheckVersionResponse> CheckVersionNumber();

        Task<BaseActionResult<ProductsListModel>> GetProductList (
			int? pageIndex,
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
			int userId
		);

		Task<BaseActionResult<DetailedProductModel>> GetProductByID (int productID, int userId);

		Task<BaseActionResult<List<PriceRange>>> GetPriceRange ();
	}
}

