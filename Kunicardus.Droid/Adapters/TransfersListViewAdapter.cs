using System;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Kuni.Core.Models.DB;
using Kunicardus.Droid.Plugins;
using Kuni.Core.Plugins;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Kunicardus.Droid.Adapters
{
	[Register ("Kunicardus.Droid.Adapters.CustomAdapter")]
	public class TransfersListViewAdapter : BaseAdapter<TransactionInfo>
	{
		Context _context;
		public List<TransactionInfo> _itemSource;

		public TransfersListViewAdapter (Context context, List<TransactionInfo> itemSource, IMvxAndroidBindingContext bindingContext)
			//: base (context, bindingContext)
		{
			_context = context;
			_itemSource = itemSource;
		}

		public override View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			TransactionViewHolder holder = null;
			var view = convertView;

			if (view != null)
				holder = view.Tag as TransactionViewHolder;
			else if (view == null) {
				holder = new TransactionViewHolder ();
				var inflater = _context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				view = inflater.Inflate (Resource.Layout.MyPageViewListItem, null);

				holder.Name = view.FindViewById<BaseTextView> (Resource.Id.txtShopName);
				holder.Date = view.FindViewById<BaseTextView> (Resource.Id.txtDate);
				holder.Amount = view.FindViewById<BaseTextView> (Resource.Id.txtAmount);
				holder.Address = view.FindViewById<BaseTextView> (Resource.Id.txtAddress);
				holder.Points = view.FindViewById<BaseTextView> (Resource.Id.txtPoints);
				view.Tag = holder;
			}

			var source = _itemSource;
			var item = source [position];
			holder.Name.Text = item.OrganizationName;
			holder.Date.Text = DateConverter.Convert (item.Date);
			holder.Amount.Text = item.PaymentAmount.ToString ();
			holder.Address.Text = item.Address;
			holder.Points.Text = Converters.ConvertPoints (item.Score);

			if (item.Score < 0) {
				view.FindViewById<BaseTextView> (Resource.Id.txtPoints).SetTextColor (new Color (242, 142, 45));
			} else {
				view.FindViewById<BaseTextView> (Resource.Id.txtPoints).SetTextColor (new Color (140, 189, 58));
			}

			if (position == 0) {
				view.FindViewById<View> (Resource.Id.imgTopLine).Visibility = ViewStates.Invisible;
			} else {
				view.FindViewById<View> (Resource.Id.imgTopLine).Visibility = ViewStates.Visible;
			}

			if (position == Count - 1) {
				view.FindViewById<View> (Resource.Id.imgBottomLine).Visibility = ViewStates.Invisible;
			} else {
				view.FindViewById<View> (Resource.Id.imgBottomLine).Visibility = ViewStates.Visible;
			}

			if (string.IsNullOrEmpty (item.Address)) {
				view.FindViewById<View> (Resource.Id.imgAddress).Visibility = ViewStates.Gone;
			} else {
				view.FindViewById<View> (Resource.Id.imgAddress).Visibility = ViewStates.Visible;
			}

			return view;
		}

		public override int Count {
			get { return _itemSource.Count; }
		}

		public override long GetItemId (int position)
		{
			return Convert.ToInt32 (_itemSource [position].OrganizationId);
		}

		public override TransactionInfo this [int position] {
			get { return _itemSource [position]; }
		}
	}
}