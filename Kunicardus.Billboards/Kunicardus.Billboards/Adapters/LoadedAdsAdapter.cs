using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Kunicardus.Billboards.Core.Models;
using Android.Graphics;
using System.Threading.Tasks;
using Kunicardus.Billboards.Plugins;
using Kunicardus.Billboards.Core.Helpers;

namespace Kunicardus.Billboards.Adapters
{
    public class LoadedAdsAdapter : BaseAdapter<AdsModel>
    {
        Activity _context;
        List<AdsModel> _adsList;

        public LoadedAdsAdapter(Activity context, List<AdsModel> adsList)
        {
            _context = context;
            _adsList = adsList;
        }

        public override AdsModel this[int position]
        {
            get { return _adsList[position]; }
        }

        public override int Count
        {
            get { return _adsList.Count; }
        }

        public override long GetItemId(int position)
        {
            return _adsList[position].AdvertismentId;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
                view = _context.LayoutInflater.Inflate(Resource.Layout.AdsItemLayout, null);

            var item = _adsList[position];
            TextView txtName = view.FindViewById<TextView>(Resource.Id.name);
            TextView txtDate = view.FindViewById<TextView>(Resource.Id.txtDate);
            TextView txtTime = view.FindViewById<TextView>(Resource.Id.txtTime);
            ImageView imgLogo = view.FindViewById<ImageView>(Resource.Id.imgLogo);

            txtName.Text = item.MerchantName;
            txtDate.Text = item.PassDate.ToGeoString();
            txtTime.Text = item.PassDate.ToShortTimeString();

            var byteArray = Convert.FromBase64String(item.MerchantLogo);
            using (Bitmap bmp = BitmapFactory.DecodeByteArray(byteArray, 0, byteArray.Length))
            {
                imgLogo.SetImageBitmap(bmp);
            }
            return view;
        }

        public void UpdateList(List<AdsModel> ads)
        {
            _adsList = ads;
        }
    }
}