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

namespace Kuni.Core.ViewModels
{
	public class CatalogListViewModel : BaseViewModel
	{
		#region Private variables

		private IProductsService _productsService;
		private ILocalDbProvider _localDBProvider;

		#endregion

		#region Properties

		private void ProductSelected (ProductModel e)
		{
			SelectedProduct = e.ProductID;
			DataChanged = true;
		}

		public bool _dataChanged;

		public bool DataChanged {
			get{ return _dataChanged; }
			set {
				_dataChanged = value;
				RaisePropertyChanged (() => DataChanged);
				RaisePropertyChanged (() => EnableClick);
			}
		}

		public bool _dataSetUpdated;

		public bool DataSetUpdated {
			get{ return _dataSetUpdated; }
			set {
				_dataSetUpdated = value;
				RaisePropertyChanged (() => DataSetUpdated);
			}
		}

		public bool _enableClick;

		public bool EnableClick {
			get{ return !_dataChanged; }
			set {
				_enableClick = value;
				RaisePropertyChanged (() => EnableClick);
			}
		}

		public int _selectedProduct;

		public int SelectedProduct {
			get{ return _selectedProduct; }
			set {
				_selectedProduct = value;
				RaisePropertyChanged (() => SelectedProduct);
			}
		}


		private List<ProductsInfo> _products;

		public List<ProductsInfo> Products {
			get{ return _products; }
			set {
				_products = value;
				RaisePropertyChanged (() => Products);
			}
		}

		private int? _categoryId;

		public int? CategoryId {
			get { return _categoryId; }
			set{ _categoryId = value; }
		}

		private int? _userTypeId;

		public int? UserTypeId {
			get { return _userTypeId; }
			set{ _userTypeId = value; }
		}

		private int? _lastModified;

		public int? LastModified {
			get { return _lastModified; }
			set{ _lastModified = value; }
		}

		private bool? _productsBySale;

		public bool? ProductsBySale {
			get { return _productsBySale; }
			set{ _productsBySale = value; }
		}

		private int? _selectedRangeId;

		public int? SelectedRangeId {
			get { return _selectedRangeId; }
			set{ _selectedRangeId = value; }
		}

		private string _searchText;

		public string SearchText {
			get { return _searchText; }
			set{ _searchText = value; }
		}

		private int? _isDiscounted;

		public int? Discounted {
			get { return _isDiscounted; }
			set{ _isDiscounted = value; }
		}


		public readonly int rowCount = 20;
		private int _pageindex;

		public int PageIndex {
			get { return _pageindex; }
			set{ _pageindex = value; }
		}

		private ICommand _productSelectedCommand;

		public ICommand ProductSelectedCommand {
			get {
				_productSelectedCommand = _productSelectedCommand ?? new MvvmCross.Commands.MvxCommand<ProductModel> (ProductSelected);
				return _productSelectedCommand;
			}
		}

		private string _displayText;

		public string DisplayText {
			get { return _displayText; }
			set{ _displayText = value; }
		}

		#endregion

		#region Constructor Implementation

		public CatalogListViewModel (
			IProductsService productService,
			ILocalDbProvider localDbProvider)
		{
			_localDBProvider = localDbProvider;
			_productsService = productService;
			PageIndex = 1;
//			GetOrFilterProductList (false);
		}

		#endregion

		#region Methods

		//holding information for refresh
		public void AssignFilterInfo (int? categoryId, int? userTypeId, int? lastModified, 
		                              int? selectedPriceRangeId, int? discounted, string search)
		{
			this.CategoryId = categoryId;
			this.UserTypeId = userTypeId;
			this.LastModified = lastModified;
			this.SelectedRangeId = selectedPriceRangeId;
			this.SearchText = search;
			this.Discounted = discounted;
		}

		public void SearchByName (string searchText)
		{
			if (!string.IsNullOrEmpty (searchText)) {
				SearchText = searchText.Trim ();
				GetOrFilterProductList (true);
			}
		}

		public void FilterBySale ()
		{
			Discounted = 1;
			GetOrFilterProductList (true);
		}

		public void LoadMore ()
		{
			Task.Run (async () => {
				UserInfo currentUser = _localDBProvider.Get<UserInfo> ().FirstOrDefault ();
				var productResponse = await _productsService.GetProductList (
					                      PageIndex,
					                      rowCount,
					                      LastModified,
					                      CategoryId,
					                      null,
					                      null,
					                      UserTypeId,
					                      null,
					                      null,
					                      null,
					                      Discounted,
					                      SelectedRangeId,
					                      SearchText,
					                      int.Parse (currentUser.UserId));
				if (productResponse.Success) {
					if (productResponse.Result != null) {
						Products.AddRange (productResponse.Result.Products);
					}
				}
				DataSetUpdated = false;
			});
		}

		public void GetOrFilterProductList (bool showDialog)
		{
			if (showDialog) {
				_dialog.ShowProgressDialog (ApplicationStrings.Loading);
			}
			Task.Run ((async () => {
				PageIndex = 1;
				try {
					DisplayText = string.Empty;
					UserInfo currentUser = _localDBProvider.Get<UserInfo> ().FirstOrDefault ();
					var productResponse = await _productsService.GetProductList (
						                      PageIndex,
						                      rowCount,
						                      LastModified,
						                      CategoryId,
						                      null,
						                      null,
						                      UserTypeId,
						                      null,
						                      null,
						                      null,
						                      Discounted,
						                      SelectedRangeId,
						                      SearchText,
						                      int.Parse (currentUser.UserId));
					if (productResponse.Success) {
						if (productResponse.Result != null) {
							Products = productResponse.Result.Products;
						}
					} else {
						DisplayText = productResponse.DisplayMessage;
					}
					DataSetUpdated = true;
					if (showDialog)
						InvokeOnMainThread (() => _dialog.DismissProgressDialog ());
				} catch (Exception ex) {
					DisplayText = ex.ToString ();
					if (showDialog)
						InvokeOnMainThread (() => _dialog.DismissProgressDialog ());
				}
			}));
		}

		public void RemoveFilter (bool showDialog)
		{
			AssignFilterInfo (null, null, null, null, null, null);
			GetOrFilterProductList (showDialog);
		}

		#endregion        
	}
}
