//using MvvmCross.Binding.Droid.Views;
using Android.Runtime;
using Android.Content;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Android.Views;
using Android.Widget;
using Kuni.Core.Models.DB;
using System.Collections.Generic;
using MvvmCross.Platforms.Android.Binding.Views;

namespace Kunicardus.Droid.Adapters
{
	[Register ("Kunicardus.Droid.Adapters.CatalogListAdapter")]
	public class CatalogListAdapter : MvxAdapter
	{
		Context _context;

		public CatalogListAdapter (Context context, IMvxAndroidBindingContext bindingContext)
			: base (context, bindingContext)
		{
			_context = context;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var View = GetView (position, convertView, parent, ItemTemplateId);
			if (View == null) {
				var inflater = _context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				View = inflater.Inflate (Resource.Layout.Grid_Product_Item, null);
			}

			var source = (List<ProductsInfo>)ItemsSource;

			var saleLayout = View.FindViewById<LinearLayout> (Resource.Id.grid_item_sale_layout);
			if (source [position].DiscountPercent == 0 || source [position].DiscountPercent == 100) {
				saleLayout.Visibility = ViewStates.Invisible;
			} else
				saleLayout.Visibility = ViewStates.Visible;

			var productPriceLayout = View.FindViewById<RelativeLayout> (Resource.Id.product_price_layout);

			if (source [position].DiscountPercent == 100)
				productPriceLayout.Visibility = ViewStates.Invisible;
			else
				productPriceLayout.Visibility = ViewStates.Visible;
			return View;

		}
	}
}

