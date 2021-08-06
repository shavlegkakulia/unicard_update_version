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
using Kunicardus.Billboards.Core.Helpers;
using Android.Graphics;
using Kunicardus.Billboards.Core;
using System.Threading.Tasks;
using Kunicardus.Billboards.Plugins;

namespace Kunicardus.Billboards.Adapters
{
    public class AdsHistoryAdapter : BaseAdapter<HistoryModel>
    {
        Activity _context;
        List<HistoryModel> _adsList;

        public AdsHistoryAdapter(Activity context, List<HistoryModel> adsList)
        {
            _context = context;
            _adsList = adsList;
        }

        public override HistoryModel this [int position]
        {
            get { return _adsList[position]; }
        }

        public override int Count
        {
            get { return _adsList.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
                view = _context.LayoutInflater.Inflate(Resource.Layout.HistoryItemLayout, null);

            var item = _adsList[position];
            TextView txtPoints = view.FindViewById<TextView>(Resource.Id.txtPoints);
            TextView txtDate = view.FindViewById<TextView>(Resource.Id.txtDate);
            TextView txtTime = view.FindViewById<TextView>(Resource.Id.txtTime);

            txtPoints.Text = item.Score.ToString();
            txtDate.Text = item.Date.ToGeoString();
            txtTime.Text = item.Date.ToShortTimeString();

            return view;
        }
    }
}