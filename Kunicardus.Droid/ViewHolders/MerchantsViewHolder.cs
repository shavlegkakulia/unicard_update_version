using Android.Widget;

namespace Kunicardus.Droid.ViewHolders
{
	public class MerchantsViewHolder //: RecyclerView.ViewHolder
	{
		public TextView MerchantName { get; set; }

		public TextView Address { get; set; }

		public TextView Distance{ get; set; }

		public TextView DistanceUnit { get; set; }

		public ImageView Image { get; set; }
	}
}