using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Kuni.Core.Models.DB;
using Android.Graphics;
using Android.Gms.Maps.Model;

namespace Kunicardus.Droid.Adapters
{
    public class InfoWindowAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
    {
        private Fragments.MerchantsFragment merchantsFragment;

        private IEnumerable<MerchantInfo> _merchants;
        private Bitmap _imageBitmap;
        LayoutInflater _inflater;

        public InfoWindowAdapter(Fragments.MerchantsFragment merchantsFragment, IEnumerable<MerchantInfo> merchants, LayoutInflater inflater)
        {
            // TODO: Complete member initialization
            this.merchantsFragment = merchantsFragment;
            _merchants = merchants;
            _inflater = inflater;
        }

        public View GetInfoContents(Marker marker)
        {
            return RenderView(marker);
        }

        public View GetInfoWindow(Marker marker)
        {
            return RenderView(marker);
        }

        public void SetImageBitmap(Bitmap bitmap)
        {
            _imageBitmap = bitmap;
        }

        private View RenderView(Marker marker)
        {
            if (marker.Snippet == null || _inflater == null)
            {
                return null;
            }

            View View = _inflater.Inflate(Resource.Layout.MapInfoWindowLayout, null);
            if (_merchants != null)
            {

                MerchantInfo item = _merchants.FirstOrDefault(x => x.MerchantId == marker.Snippet);

                ImageView logo = View.FindViewById<ImageView>(Resource.Id.imgLogo);
                TextView name = View.FindViewById<TextView>(Resource.Id.name);
                TextView address = View.FindViewById<TextView>(Resource.Id.address);
                TextView points = View.FindViewById<TextView>(Resource.Id.points);

                if (_imageBitmap != null)
                {
                    logo.SetImageBitmap(_imageBitmap);
                }

                string unitDesc, score;

                if (item != null)
                {
                    name.Text = item.MerchantName;
                    address.Text = item.Address;

                    unitDesc = item.UnitDescription;
                    score = item.UnitScore;
                    points.Text = string.Format("{0} - {1} ქულა", unitDesc, score);
                }
                else {
                    points.Visibility = ViewStates.Invisible;
                }


            }
            return View;
        }
    }
}