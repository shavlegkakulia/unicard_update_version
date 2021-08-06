using System;
using Android.Widget;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Content;
using Kunicardus.Billboards.Core.Models;
using Android;
using Kunicardus.Billboards.ViewHolders;

namespace Kunicardus.Billboards.Adapters
{
	public class MenuAdapter:BaseAdapter<MenuModel>
	{
		List<MenuModel> _model;
		Activity _context;

		public MenuAdapter (Activity context, List<MenuModel> model)
		{
			_model = model;
			_context = context;
		}

		public override MenuModel this [int position] {
			get { return _model [position]; }
		}

		public override int Count {
			get { return _model.Count; }
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView;
			if (view == null) {
				var inflater = _context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				view = inflater.Inflate (Resource.Layout.MenuItem, null);

				var holder = new MenuItemViewHolder ();

				holder.Title = view.FindViewById<BaseTextView> (Resource.Id.txtTitle);
				holder.Icon = view.FindViewById<ImageView> (Resource.Id.imgIcon);
				view.Tag = holder;
			}
			var item = _model [position];
			var tag = (MenuItemViewHolder)view.Tag;

			tag.Title.Text = item.Name;
			var img = _context.Resources.GetIdentifier (item.IconName, "drawable", _context.PackageName);
			tag.Icon.SetImageResource (img);

			return view;
		}
	}
}

