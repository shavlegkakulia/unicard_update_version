using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using Kuni.Core.Models.DB;
using Android.Graphics;
//using MvvmCross.Binding.Droid.Views;
//using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Kunicardus.Droid.Adapters
{
    public class NewsListViewAdapter : MvxAdapter
    {

        Context _context;

        public NewsListViewAdapter(Context context, IMvxAndroidBindingContext bindingContext) : base(context, bindingContext)
        {
            _context = context;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var View = GetView(position, convertView, parent, ItemTemplateId);
            if (View == null)
            {
                var inflater = _context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
                View = inflater.Inflate(Resource.Layout.NewsListItemView, null);
            }

            var source = (List<NewsInfo>)ItemsSource;
            var item = source[position];

            if (item.IsRead)
            {
                View.FindViewById<TextView>(Resource.Id.name).SetTextColor(Color.ParseColor("#929191"));
            }
            else
            {
                View.FindViewById<TextView>(Resource.Id.name).SetTextColor(Color.ParseColor("#000000"));
            }

            return View;
        }
    }
}