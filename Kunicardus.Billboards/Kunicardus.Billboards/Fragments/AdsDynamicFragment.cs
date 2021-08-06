using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Kunicardus.Billboards.Fragments
{
    public class AdsDynamicFragment : Android.Support.V4.App.Fragment
    {
		/*returns view dynamicaly*/
		private Func<LayoutInflater, ViewGroup, Bundle, View> _view;
        public AdsDynamicFragment(Func<LayoutInflater, ViewGroup, Bundle, View> view)
		{
			_view = view;
		}
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);
			return _view (inflater, container, savedInstanceState);		
		}
    }
}