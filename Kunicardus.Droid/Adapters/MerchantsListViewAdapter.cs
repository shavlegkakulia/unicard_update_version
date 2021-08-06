using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Kuni.Core.Models.DB;
using Kunicardus.Droid.ViewHolders;

namespace Kunicardus.Droid.Adapters
{
	public class MerchantsListViewAdapter : BaseAdapter<MerchantInfo>
	{
		List<MerchantInfo> _model;
		Activity _context;

		public MerchantsListViewAdapter (List<MerchantInfo> model, Activity context)
		{
			_model = model;
			_context = context;
		}

		public override MerchantInfo this [int position] {
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
			View View = convertView;
			if (View == null) {
				var inflater = _context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				View = inflater.Inflate (Resource.Layout.MerchantsListViewItem, null);

				var holder = new MerchantsViewHolder ();
				////holder.Address = View.FindViewById<TextView>(Resource.Id.address);
				////holder.Distance = View.FindViewById<TextView>(Resource.Id.distance);
				////holder.DistanceUnit = View.FindViewById<TextView>(Resource.Id.distanceu);
				////holder.AmountText = View.FindViewById<TextView>(Resource.Id.amountText);
				////holder.MinPayText = View.FindViewById<TextView>(Resource.Id.minPayText);
				////holder.Name = View.FindViewById<TextView>(Resource.Id.debitorName);
				////holder.OverdueDays = View.FindViewById<TextView>(Resource.Id.overdueDays);
				////holder.PaymentDate = View.FindViewById<TextView>(Resource.Id.paymentDate);
				////holder.TextView1 = View.FindViewById<TextView>(Resource.Id.textView1);
				////holder.TextView2 = View.FindViewById<TextView>(Resource.Id.textView2);
				////holder.Percentage = View.FindViewById<TextView>(Resource.Id.percentage);

				// View.Tag = holder;
			}
			return View;
		}
	}
}