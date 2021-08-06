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
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System.Threading.Tasks;
using Android.Graphics;
using Kunicardus.Billboards.Core.DbModels;
using Kunicardus.Billboards;

namespace Kunicardus.Billboards.Adapters
{
	public class InfoWindowAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
	{
		LayoutInflater _inflater;
		Billboard _billboard;
        private Kunicardus.Billboards.Fragments.BillboardsFragment billboardsFragment;
        private Dictionary<Marker, Billboard> markersDictionary;

		public InfoWindowAdapter (Kunicardus.Billboards.Fragments.BillboardsFragment billboardsFragment, Dictionary<Marker, Billboard> markersDictionary)
		{
			// TODO: Complete member initialization
            this.billboardsFragment = billboardsFragment;
			this.markersDictionary = markersDictionary;
		}

		public View GetInfoContents (Marker marker)
		{
			return RenderVIew (marker);
		}

		public View GetInfoWindow (Marker marker)
		{
			return RenderVIew (marker);
		}

		//public void SetImageBitmap (Bitmap bitmap)
		//{
		//	_imageBitmap = bitmap;
		//}

		private View RenderVIew (Marker marker)
		{
            View view = billboardsFragment.GetLayoutInflater(null).Inflate(Resource.Layout.MapInfoWindowLayout, null);

            Billboard item = markersDictionary.Where(x => x.Key.Id == marker.Id).FirstOrDefault().Value;

			ImageView logo = view.FindViewById<ImageView> (Resource.Id.imgLogo);
			TextView name = view.FindViewById<TextView> (Resource.Id.name);

            var byteArray = Convert.FromBase64String(item.MerchantLogo);
            using (Bitmap bmp = BitmapFactory.DecodeByteArray(byteArray, 0, byteArray.Length))
            {
                logo.SetImageBitmap(bmp);
            }

			if (item != null) {
                name.Text = item.MerchantName;
			}

			return view;
		}
	}
}