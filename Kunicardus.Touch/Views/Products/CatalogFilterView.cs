using System;
using UIKit;
using Kunicardus.Touch.Helpers.UI;
using CoreGraphics;
using System.Collections.Generic;
using Kunicardus.Core.ViewModels.iOSSpecific;
using Kunicardus.Core;
using Kunicardus.Core.Models.DB;
using System.Linq;

namespace Kunicardus.Touch
{
	public class CatalogFilterView : UIView
	{
		#region Vars

		private AppDelegate app;
		private nfloat _yforClosedCondition;
		private nfloat _yforOpenedCondition;
		private nfloat _width;
		private nfloat _height;
		private nfloat toggleAnimationDuration = 0.2f;

		private List<CatalogFilterItem> _items;

		#endregion

		#region Props

		public List<PriceRange> PriceRanges {
			get;
			set;
		}

		public List<UserTypesInfo> UserTypes {
			get;
			set;
		}

		public List<ProductCategoryInfo> ProductCategories {
			get;
			set;
		}

		private bool _opened;

		public bool Opened {
			get {
				return _opened;
			}
			set {
				_opened = value;
			}
		}

		public ProductFilterInfo FilterInfo { get; set; }

		#endregion

		#region EventHandlers

		public event EventHandler<ProductFilterInfo> Filtered;

		#endregion

		#region UI

		private UIView _ovarlay;

		#endregion

		#region Ctors

		public CatalogFilterView (ProductFilterInfo filterInfo)
		{			
			FilterInfo = filterInfo;

			app = UIApplication.SharedApplication.Delegate as AppDelegate;
			_yforClosedCondition = app.Window.Frame.Width + 10;
			_yforOpenedCondition = 65f;
			_width = app.Window.Frame.Width - _yforOpenedCondition;
			_height = app.Window.Frame.Height;

			this.Frame = new CoreGraphics.CGRect (_yforClosedCondition, 0, _width, _height);
			this.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			this.Layer.ShadowColor = UIColor.Black.CGColor;
			this.Layer.ShadowOpacity = 0.8f;
			this.Layer.ShadowRadius = 10f;
			this.Layer.ShadowOffset = new CGSize (3, 3);

			_ovarlay = new UIView (new CoreGraphics.CGRect (0, 0, app.Window.Frame.Width, app.Window.Frame.Height));
			_ovarlay.BackgroundColor = UIColor.Gray;
			_ovarlay.Alpha = 0;

			UITapGestureRecognizer tap = new UITapGestureRecognizer (() => {
				Close ();
			});
			_ovarlay.UserInteractionEnabled = true;
			_ovarlay.AddGestureRecognizer (tap);
		}

		#endregion

		#region Methods

		UISearchBar _searchBox;

		public void InitUI ()
		{			
			// Close button initialisation
			UIButton close = new UIButton (UIButtonType.RoundedRect);
			close.SetImage (ImageHelper.MaxResizeImage (UIImage.FromBundle ("close"), 16, 0), UIControlState.Normal);
			close.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			close.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			close.BackgroundColor = UIColor.Clear;
			close.SetTitleColor (UIColor.White, UIControlState.Normal);
			close.TintColor = UIColor.White;
			close.SizeToFit ();
			close.Frame = new CGRect (3, 22, close.Frame.Width + 20, close.Frame.Height + 20);
			close.TouchUpInside += delegate {
				Close ();
			};
			this.AddSubview (close);

			// Filter title initialisation
			UILabel title = new UILabel ();
			title.Font = UIFont.FromName (Styles.Fonts.BPGExtraSquare, 16);
			title.TextColor = UIColor.White;
			title.Text = ApplicationStrings.Catalogue;
			title.SizeToFit ();
			title.Frame = new CGRect ((this.Frame.Width - title.Frame.Width) / 2.0f, 30, title.Frame.Width, title.Frame.Height);
			this.AddSubview (title);

			nfloat buttonLeftPadding = 5f;
			nfloat buttonWidth = this.Frame.Width - (buttonLeftPadding * 2.0f);
			nfloat buttonHeight = 30f;
			nfloat bottomPadding = 5;

			_searchBox = new UISearchBar (new CGRect (0, title.Frame.Bottom + 20, buttonWidth + buttonLeftPadding + buttonLeftPadding, buttonHeight));
			_searchBox.TintColor = UIColor.White;
			_searchBox.ShowsCancelButton = true;
			_searchBox.BackgroundColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);

			UITextField searchBarTextField = (UITextField)_searchBox.ValueForKey (new Foundation.NSString ("_searchField"));
			UIImageView leftImageView = (UIImageView)searchBarTextField.LeftView;
			leftImageView.Image = leftImageView.Image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
			leftImageView.TintColor = UIColor.White;

			_searchBox.BarTintColor = UIColor.Clear.FromHexString (Styles.Colors.HeaderGreen);
			_searchBox.BarStyle = UIBarStyle.Black;
			_searchBox.Translucent = true;
			_searchBox.SearchBarStyle = UISearchBarStyle.Minimal;
			this.AddSubview (_searchBox);
			_searchBox.SearchButtonClicked += delegate {
				FilterInfo.SearchCriteria = _searchBox.Text;
				_searchBox.ResignFirstResponder ();
				if (Filtered != null) {
					Filtered (this, FilterInfo);
				}
				Close ();
			};
			_searchBox.CancelButtonClicked += delegate {
				_searchBox.Text = "";
				FilterInfo.SearchCriteria = "";
				_searchBox.ResignFirstResponder ();
				if (Filtered != null) {
					Filtered (this, FilterInfo);
				}
				Close ();
			};


			_items = new List<CatalogFilterItem> ();

			UIScrollView scroll = new UIScrollView (new CGRect (buttonLeftPadding, _searchBox.Frame.Bottom + bottomPadding, buttonWidth, app.Window.Frame.Height -
			                      _searchBox.Frame.Bottom - bottomPadding - bottomPadding));
			this.AddSubview (scroll);

			CatalogFilterItem discounted = 
				new CatalogFilterItem (new CGRect (0, bottomPadding, buttonWidth, buttonHeight));		
			discounted.SetTitle (ApplicationStrings.Discounted, UIControlState.Normal);
			discounted.Discounted = true;
			discounted.TouchUpInside += FilterItemClick;
			_items.Add (discounted);
			scroll.AddSubview (discounted);

			CatalogFilterItem lastAdded = 
				new CatalogFilterItem (new CGRect (0, discounted.Frame.Bottom + bottomPadding, buttonWidth, buttonHeight));		
			lastAdded.SetTitle (ApplicationStrings.LastAdded, UIControlState.Normal);
			lastAdded.LastAdded = true;
			lastAdded.TouchUpInside += FilterItemClick;
			_items.Add (lastAdded);
			scroll.AddSubview (lastAdded);


			// Categories -----------------------
			FilterExpandableList categoriesView = new FilterExpandableList (buttonWidth, 0,
				                                      lastAdded.Frame.Bottom + bottomPadding, ApplicationStrings.Categories);
			nfloat tmpTop = categoriesView.Button.Frame.Bottom;
			if (ProductCategories!=null && ProductCategories.Count > 0) {
				foreach (var category in ProductCategories.Where(x=>!string.IsNullOrWhiteSpace(x.CategoryID)).ToList()) {
					CatalogFilterItem categoryFilterItem = 
						new CatalogFilterItem (new CGRect (0, tmpTop + bottomPadding, buttonWidth, buttonHeight));		
					categoryFilterItem.SetTitle (category.CategoryName, UIControlState.Normal);
					categoryFilterItem.CategoryId = int.Parse (category.CategoryID);
					categoryFilterItem.TouchUpInside += FilterItemClick;
					_items.Add (categoryFilterItem);
					categoriesView.AddSubview (categoryFilterItem);
					categoriesView.contentHeight = categoryFilterItem.Frame.Bottom + 5;
					tmpTop = categoryFilterItem.Frame.Bottom;
				}
				scroll.AddSubview (categoriesView);
			}
			// UserTypes -----------------------
			//FilterExpandableList userTypesView = new FilterExpandableList (buttonWidth, 0,
			//	                                     categoriesView.Frame.Bottom + bottomPadding, ApplicationStrings.UserType);
			//tmpTop = userTypesView.Button.Frame.Bottom;
			//if (UserTypes!=null && UserTypes.Count > 0) {
			//	foreach (var usertype in UserTypes) {
			//		CatalogFilterItem userTypeFilterItem = 
			//			new CatalogFilterItem (new CGRect (0, tmpTop + bottomPadding, buttonWidth, buttonHeight));		
			//		userTypeFilterItem.SetTitle (usertype.UserTypeName, UIControlState.Normal);
			//		userTypeFilterItem.UserTypeId = usertype.UserTypeId;
			//		userTypeFilterItem.TouchUpInside += FilterItemClick;
			//		_items.Add (userTypeFilterItem);
			//		userTypesView.AddSubview (userTypeFilterItem);
			//		userTypesView.contentHeight = userTypeFilterItem.Frame.Bottom + 5;
			//		tmpTop = userTypeFilterItem.Frame.Bottom;
			//	}
			//	scroll.AddSubview (userTypesView);
			//}
			// Price Ranges -----------------------
			FilterExpandableList priceRangesView = new FilterExpandableList (buttonWidth, 0,
                                                       categoriesView.Frame.Bottom + bottomPadding, ApplicationStrings.PriceRanges);
			tmpTop = priceRangesView.Button.Frame.Bottom;
			if (PriceRanges!=null && PriceRanges.Count > 0) {
				foreach (var pricerange in PriceRanges) {
					CatalogFilterItem priceRangeFilterItem = 
						new CatalogFilterItem (new CGRect (0, tmpTop + bottomPadding, buttonWidth, buttonHeight));		
					priceRangeFilterItem.SetTitle (pricerange.Name, UIControlState.Normal);
					priceRangeFilterItem.PointRangeId = pricerange.ID;
					priceRangeFilterItem.TouchUpInside += FilterItemClick;
					_items.Add (priceRangeFilterItem);
					priceRangesView.AddSubview (priceRangeFilterItem);
					priceRangesView.contentHeight = priceRangeFilterItem.Frame.Bottom + 5;
					tmpTop = priceRangeFilterItem.Frame.Bottom;
				}
				scroll.AddSubview (priceRangesView);
			}

			// Collapsable view click events
			categoriesView.AfterClick = () => {
				//userTypesView.Frame = new CGRect (userTypesView.Frame.X,
					//categoriesView.Frame.Bottom + bottomPadding,
					//userTypesView.Frame.Width,
					//userTypesView.Frame.Height);
				priceRangesView.Frame = new CGRect (priceRangesView.Frame.X,
                    categoriesView.Frame.Bottom + bottomPadding,
					priceRangesView.Frame.Width,
					priceRangesView.Frame.Height);
				scroll.ContentSize = new CGSize (scroll.Frame.Width, 
					priceRangesView.Frame.Bottom + bottomPadding);
			};
			//userTypesView.AfterClick = () => {
			//	userTypesView.Frame = new CGRect (userTypesView.Frame.X,
			//		categoriesView.Frame.Bottom + bottomPadding,
			//		userTypesView.Frame.Width,
			//		userTypesView.Frame.Height);
			//	priceRangesView.Frame = new CGRect (priceRangesView.Frame.X,
			//		userTypesView.Frame.Bottom + bottomPadding,
			//		priceRangesView.Frame.Width,
			//		priceRangesView.Frame.Height);
			//	scroll.ContentSize = new CGSize (scroll.Frame.Width, 
			//		priceRangesView.Frame.Bottom + bottomPadding);
			//};
			priceRangesView.AfterClick = () => {
				//userTypesView.Frame = new CGRect (userTypesView.Frame.X,
					//categoriesView.Frame.Bottom + bottomPadding,
					//userTypesView.Frame.Width,
					//userTypesView.Frame.Height);
				priceRangesView.Frame = new CGRect (priceRangesView.Frame.X,
                    categoriesView.Frame.Bottom + bottomPadding,
					priceRangesView.Frame.Width,
					priceRangesView.Frame.Height);
				scroll.ContentSize = new CGSize (scroll.Frame.Width, 
					priceRangesView.Frame.Bottom + bottomPadding);
			};
		}

		void FilterItemClick (object sender, EventArgs e)
		{
			MakeAllGreen ();
			ClearFilterInfoData ();
			var filterItem = sender as CatalogFilterItem;
			if (filterItem.Discounted) {
				FilterInfo.Discounted = true;
			}
			if (filterItem.LastAdded) {
				FilterInfo.LastAdded = true;
			}
			if (filterItem.CategoryId.HasValue) {
				FilterInfo.CategoryId = filterItem.CategoryId.Value;
			}
			if (filterItem.PointRangeId.HasValue) {
				FilterInfo.PointRangeId = filterItem.PointRangeId.Value;
			}
			if (filterItem.UserTypeId.HasValue) {
				FilterInfo.UserTypeId = filterItem.UserTypeId.Value;
			}
			filterItem.BackgroundColor = UIColor.Clear.FromHexString ("#f5a72b");
			FilterInfo.SearchCriteria = _searchBox.Text;

			if (Filtered != null) {
				Filtered (this, FilterInfo);
			}
			Close ();
		}

		public void Clear ()
		{
			MakeAllGreen ();
			ClearFilterInfoData ();
			FilterInfo.SearchCriteria = string.Empty;
			_searchBox.Text = string.Empty;
		}

		private void ClearFilterInfoData ()
		{
			FilterInfo.CategoryId = null;
			FilterInfo.Discounted = null;
			FilterInfo.LastAdded = null;
			FilterInfo.PointRangeId = null;
			FilterInfo.UserTypeId = null;
		}

		private void MakeAllGreen ()
		{
			foreach (var item in _items) {
				item.BackgroundColor = UIColor.Clear.FromHexString ("#9ccd45");
			}
		}

		public void Open ()
		{
			if (!_opened) {
				app.Window.AddSubview (_ovarlay);
				app.Window.BringSubviewToFront (this);
				UIView.Animate (toggleAnimationDuration, () => {
					_ovarlay.Alpha = 0.3f;
					this.Frame = new CoreGraphics.CGRect (_yforOpenedCondition, 0, _width, _height);
				},
					() => {
						_opened = true;
					});				
			}

		}

		public void Close ()
		{
			if (_opened) {
				_searchBox.ResignFirstResponder ();
				UIView.Animate (toggleAnimationDuration, () => {
					_ovarlay.Alpha = 0.0f;
					this.Frame = new CoreGraphics.CGRect (_yforClosedCondition, 0, _width, _height);
				},
					() => {
						_ovarlay.RemoveFromSuperview ();
						_opened = false;	
					});		
				
			}
		}

		#endregion
	}
}

